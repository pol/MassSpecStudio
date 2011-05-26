using System.Collections.Generic;
using MassSpecStudio.Core;
using MassSpecStudio.Core.Domain;
using pwiz.CLI.msdata;
using Domain = MassSpecStudio.Core.Domain;

namespace ProteoWizard.MassSpecStudio.DataProvider
{
	public class TicGenerator
	{
		private readonly Run run;
		private readonly SpectrumCache spectrumCache;
		private readonly TicCache ticCache;
		private readonly SpectrumExtractor spectrumExtractor;

		public TicGenerator(Run run, SpectrumCache spectrumCache)
		{
			this.run = run;
			this.spectrumCache = spectrumCache;
			spectrumExtractor = new SpectrumExtractor(run, spectrumCache);
			ticCache = new TicCache();
		}

		public IXYData Generate(TimeUnit timeUnits)
		{
			if (DoesFileNativelySupportTicExtraction())
			{
				return ExtractTicNativelyFromFile();
			}

			return ExtractTicByManuallyLookingAtEachSpectrumInTheFile(timeUnits);
		}

		private bool DoesFileNativelySupportTicExtraction()
		{
			return run.chromatogramList != null;
		}

		private IXYData ExtractTicNativelyFromFile()
		{
			Chromatogram chromatogram = run.chromatogramList.chromatogram(0, true);

			IList<double> xVals = chromatogram.binaryDataArrays[0].data;
			IList<double> yVals = chromatogram.binaryDataArrays[1].data;

			BinaryData xValues = new BinaryData();
			BinaryData yValues = new BinaryData();

			for (int i = 0; i < xVals.Count; i++)
			{
				xValues.Add(xVals[i]);
				yValues.Add(yVals[i]);
			}

			return new XYBinaryData(xValues, yValues);
		}

		private IXYData ExtractTicByManuallyLookingAtEachSpectrumInTheFile(TimeUnit timeUnits)
		{
			if (ticCache.IsInCache())
			{
				return ticCache.Read();
			}
			else
			{
				BinaryData xVals = new BinaryData();
				BinaryData yVals = new BinaryData();

				pwiz.CLI.msdata.SpectrumList spectrumList = run.spectrumList;
				int timePoints = spectrumList.size();

				for (int i = 0; i < timePoints; i++)
				{
					pwiz.CLI.msdata.Spectrum s = spectrumList.spectrum(i);
					Scan scan = null;

					if (s.scanList.scans.Count > 0)
					{
						scan = s.scanList.scans[0];
					}

					CVParam param;

					param = s.cvParam(pwiz.CLI.CVID.MS_ms_level);
					int msLevel = !param.empty() ? (int)param.value : 0;

					if (msLevel <= 1)
					{
						param = scan != null ? scan.cvParam(pwiz.CLI.CVID.MS_scan_start_time) : new CVParam();
						double scanTime = !param.empty() ? (double)param.value : 0;

						// TODO: Duplicated with CalculateRT, reuse RtToTimePointConverter.
						if (timeUnits == TimeUnit.Seconds)
						{
							xVals.Add(scanTime / 60);
						}

						param = s.cvParam(pwiz.CLI.CVID.MS_total_ion_current);
						yVals.Add(!param.empty() ? (double)param.value : 0);
					}
				}

				IXYData tic = new XYBinaryData(xVals, yVals);
				ticCache.Set(tic);
				return tic;
			}
		}

		private bool SpectrumIsSurveryScan(Domain.Spectrum spectrum)
		{
			return spectrum.Count > 0;
		}
	}
}
