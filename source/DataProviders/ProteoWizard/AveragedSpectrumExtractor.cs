using System.Collections.Generic;
using MassSpecStudio.Core;
using MassSpecStudio.Core.DataProvider;
using MassSpecStudio.Core.Domain;
using pwiz.CLI;
using pwiz.CLI.msdata;
using Domain = MassSpecStudio.Core.Domain;

namespace ProteoWizard.MassSpecStudio.DataProvider
{
	public class AveragedSpectrumExtractor
	{
		private const int NumberOfSecondsInAMinute = 60;
		private readonly Run run;
		private readonly SpectrumCache spectrumCache;
		private readonly SpectrumExtractor spectrumExtractor;
		private readonly RtToTimePointConverter rtToTimePointConverter;

		public AveragedSpectrumExtractor(Run run, SpectrumCache spectrumCache)
		{
			this.run = run;
			this.spectrumCache = spectrumCache;
			rtToTimePointConverter = new RtToTimePointConverter(run, spectrumCache);
			spectrumExtractor = new SpectrumExtractor(run, spectrumCache);
		}

		public Domain.ISpectrum GetAveragedSpectrum(double startRt, double endRt, TimeUnit timeUnits)
		{
			return GetAveragedSpectrum(startRt, endRt, timeUnits, 0, double.MaxValue);
		}

		public Domain.ISpectrum GetAveragedSpectrum(double startRt, double endRt, TimeUnit timeUnits, double mzLower, double mzUpper)
		{
			List<int> scanList = GetScanList(startRt, endRt);
			SpectrumList spectrumList = run.spectrumList;
			int timePoints = spectrumList.size();

			List<Domain.ISpectrum> msValuesList = new List<Domain.ISpectrum>();

			List<int> tempScanList = new List<int>();

			if (scanList == null)
			{
				// If scan list is empty then take entire chromatographic range.
				for (int i = 0; i < timePoints; i++)
				{
					tempScanList.Add(i);
				}
			}
			else
			{
				tempScanList = scanList;
			}

			for (int i = 0; i < tempScanList.Count; i++)
			{
				msValuesList.Add(spectrumExtractor.GetSpectrum(tempScanList[i], timeUnits, mzLower, mzUpper));
			}

			// Merge scans
			return SpectrumHelper.CreateAveragedSpectrum(msValuesList);
		}

		public Domain.ISpectrum GetMSMSSpectrum(double targetRT, double rtWidth, double mz, double mzVar)
		{
			// TODO: Get indexees of scans between the two rts. SpectrumExtractor.CaculateRT
			List<int> scanList = GetScanListForMSMS(targetRT - (0.5 * rtWidth), targetRT + (0.5 * rtWidth));

			// TODO: loop thru scan range and pull spectra from level 2 with the appropriate precusor + variant.
			List<List<XYPoint>> msmsSpectraList = GetMSMSDataArraysFromMSScans(scanList, mz - mzVar, mz + mzVar);
			List<Domain.ISpectrum> spectraList = ConvertXYPointsToSpectra(msmsSpectraList);

			return SpectrumHelper.CreateAveragedSpectrum(spectraList);
		}

		private List<int> GetScanListForMSMS(double startRt, double endRt)
		{
			int startTimePoint = rtToTimePointConverter.ConvertForMSMS(startRt, TimeUnit.Seconds);
			int endTimePoint = rtToTimePointConverter.ConvertForMSMS(endRt, TimeUnit.Seconds);

			List<int> scanlist = new List<int>();
			for (int i = startTimePoint; i <= endTimePoint; i++)
			{
				scanlist.Add(i);
			}
			return scanlist;
		}

		private List<int> GetScanList(double startRt, double endRt)
		{
			int startTimePoint = rtToTimePointConverter.Convert(startRt, TimeUnit.Seconds);
			int endTimePoint = rtToTimePointConverter.Convert(endRt, TimeUnit.Seconds);

			List<int> scanlist = new List<int>();
			for (int i = startTimePoint; i <= endTimePoint; i++)
			{
				scanlist.Add(i);
			}
			return scanlist;
		}

		private List<Domain.ISpectrum> ConvertXYPointsToSpectra(List<List<XYPoint>> msmsSpectraList)
		{
			List<Domain.ISpectrum> spectra = new List<Domain.ISpectrum>();
			foreach (List<XYPoint> msmsSpectrum in msmsSpectraList)
			{
				spectra.Add(new Domain.Spectrum(1, msmsSpectrum));
			}
			return spectra;
		}

		private List<List<XYPoint>> GetMSMSDataArraysFromMSScans(List<int> scanList, double mzlower, double mzupper)
		{
			List<List<XYPoint>> msValuesList = new List<List<XYPoint>>(500);
			SpectrumList spectrumList = run.spectrumList;
			int timePoints = spectrumList.size();

			List<List<XYPoint>> msmsValuesList = new List<List<XYPoint>>(500);
			List<int> tempScanList = new List<int>();

			if (scanList == null)
			{
				for (int i = 0; i < timePoints; i++)
				{
					tempScanList.Add(i);
				}
			}
			else
			{
				tempScanList = scanList;
			}

			for (int i = 0; i < tempScanList.Count; i++)
			{
				pwiz.CLI.msdata.Spectrum spectrum = spectrumList.spectrum(tempScanList[i], true);

				string level = (string)spectrum.cvParam(CVID.MS_ms_level).value;
				bool foundSelectedIon = false;
				if (level == "2")
				{
					PrecursorList precursorlist = spectrum.precursors;

					foreach (Precursor pc in precursorlist)
					{
						foreach (SelectedIon si in pc.selectedIons)
						{
							double selectedIonMZ = (double)si.cvParam(CVID.MS_selected_ion_m_z).value;
							if (selectedIonMZ >= mzlower && selectedIonMZ <= mzupper)
							{
								foundSelectedIon = true;
							}
						}
					}
				}
				if (foundSelectedIon)
				{
					BinaryDataArray mzArray = spectrum.getMZArray();
					BinaryDataArray intensityArray = spectrum.getIntensityArray();
					BinaryData mzData = mzArray.data;
					BinaryData intensityData = intensityArray.data;

					List<XYPoint> msmsValues = new List<XYPoint>(1000);

					for (int j = 0; j < mzData.Count; j++)
					{
						XYPoint pair = new XYPoint(mzData[j], intensityData[j]);
						pair.NumberOfDuplicates = 0;
						msmsValues.Add(pair);
					}
					msmsValuesList.Add(msmsValues);
				}
			}
			return msmsValuesList;
		}
	}
}
