using System.Collections.Generic;
using MassSpecStudio.Core;
using MassSpecStudio.Core.Domain;
using pwiz.CLI.msdata;
using Domain = MassSpecStudio.Core.Domain;

namespace ProteoWizard.MassSpecStudio.DataProvider
{
	public class SpectrumExtractor
	{
		private readonly Run run;
		private readonly SpectrumCache spectrumCache;
		private RtToTimePointConverter rtToTimePointConverter;

		public SpectrumExtractor(Run run, SpectrumCache spectrumCache)
		{
			this.run = run;
			this.spectrumCache = spectrumCache;
		}

		public RtToTimePointConverter RtToTimePointConverter
		{
			get
			{
				if (rtToTimePointConverter == null)
				{
					rtToTimePointConverter = new RtToTimePointConverter(run, spectrumCache);
				}
				return rtToTimePointConverter;
			}
		}

		public Domain.ISpectrum GetSpectrum(double rt, TimeUnit timeUnits)
		{
			return GetSpectrum(rt, timeUnits, 0, double.MaxValue);
		}

		public Domain.ISpectrum GetSpectrum(double rt, TimeUnit timeUnits, double mzLower, double mzUpper)
		{
			return GetSpectrum(RtToTimePointConverter.Convert(rt, timeUnits), timeUnits, mzLower, mzUpper);
		}

		public Domain.ISpectrum GetSpectrum(int timePoint, TimeUnit timeUnits)
		{
			return GetSpectrum(timePoint, timeUnits, 0, double.MaxValue);
		}

		public Domain.ISpectrum GetSpectrum(int timePoint, TimeUnit timeUnits, double mzLower, double mzUpper)
		{
			if (spectrumCache.IsInCache(timePoint))
			{
				Domain.ISpectrum spectrum = spectrumCache.Read(timePoint);
				if (mzLower == 0 && mzUpper == double.MaxValue)
				{
					return spectrum;
				}
				else
				{
					IList<double> xVals = spectrum.XValues;
					IList<double> yVals = spectrum.YValues;
					BinaryData xValues = new BinaryData();
					BinaryData yValues = new BinaryData();

					for (int i = 0; i < xVals.Count; i++)
					{
						if (xVals[i] >= mzLower && xVals[i] <= mzUpper)
						{
							xValues.Add(xVals[i]);
							yValues.Add(yVals[i]);
						}
					}

					return new BinarySpectrum(spectrum.StartRT, xValues, yValues);
				}
			}
			else
			{
				pwiz.CLI.msdata.Spectrum spectrumFromFile = run.spectrumList.spectrum(timePoint, true);
				string level = (string)spectrumFromFile.cvParam(pwiz.CLI.CVID.MS_ms_level).value;
				if (level == "1")
				{
					double rt = RtToTimePointConverter.CalculateRT(spectrumFromFile, timeUnits);

					BinaryDataArray mzArray = spectrumFromFile.getMZArray();
					BinaryDataArray intensityArray = spectrumFromFile.getIntensityArray();
					BinaryData mzData = mzArray.data;
					BinaryData intensityData = intensityArray.data;

					FilterMassList(mzData, intensityData, mzLower, mzUpper);

					BinarySpectrum spectrum = new BinarySpectrum(rt, mzData, intensityData);
					if (spectrum.Count == mzData.Count)
					{
						AddSpectrumToCacheIfNotPresent(timePoint, spectrum);
					}

					return spectrum;
				}
			}

			return new Domain.Spectrum(0, new List<XYPoint>());
		}

		private void FilterMassList(IList<double> mzData, IList<double> intensityData, double mzLower, double mzUpper)
		{
			if (mzLower != 0 && mzUpper != double.MaxValue)
			{
				for (int i = 0; i < mzData.Count; i++)
				{
					if (mzData[i] < mzLower || mzData[i] > mzUpper)
					{
						mzData.RemoveAt(i);
						intensityData.RemoveAt(i);
						i--;
					}
				}
			}
		}

		private void AddSpectrumToCacheIfNotPresent(int timePoint, BinarySpectrum spectrum)
		{
			if (!spectrumCache.IsInCache(timePoint))
			{
				spectrumCache.Add(timePoint, spectrum);
			}
		}
	}
}
