using System;
using System.Collections.Generic;
using MassSpecStudio.Core;
using MassSpecStudio.Core.Domain;
using pwiz.CLI.msdata;

namespace ProteoWizard.MassSpecStudio.DataProvider
{
	public class RtToTimePointConverter
	{
		private const int NumberOfSecondsInAMinute = 60;
		private const double Threshold = 0.01;
		private readonly TicGenerator ticGenerator;
		private readonly Run run;

		public RtToTimePointConverter(Run run, SpectrumCache spectrumCache)
		{
			this.run = run;
			ticGenerator = new TicGenerator(run, spectrumCache);
		}

		public int Convert(double rt, TimeUnit timeUnits)
		{
			IXYData tic = ticGenerator.Generate(timeUnits);
			IList<double> mzValues = tic.XValues;

			int timePoint = 0;
			double timePointDelta = double.MaxValue;
			double delta = double.MaxValue;
			for (int i = 0; i < mzValues.Count; i++)
			{
				delta = Math.Abs(mzValues[i] - rt);
				if (delta < Threshold && timePointDelta > delta)
				{
					timePointDelta = delta;
					timePoint = i;
				}
				else
					if (timePoint > 0)
					{
						return timePoint;
					}
			}
			throw new RtDoesNotExistException();
		}

		public int ConvertForMSMS(double rt, TimeUnit timeUnits)
		{
			pwiz.CLI.msdata.SpectrumList spectrumList = run.spectrumList;
			int timePoints = spectrumList.size();

			for (int i = 0; i < timePoints; i++)
			{
				pwiz.CLI.msdata.Spectrum spectrumFromFile = run.spectrumList.spectrum(i, true);
				string level = (string)spectrumFromFile.cvParam(pwiz.CLI.CVID.MS_ms_level).value;
				if (level == "1")
				{
					double calculatedRt = CalculateRT(spectrumFromFile, timeUnits);
					if (calculatedRt >= rt)
					{
						return i;
					}
				}
			}
			throw new RtDoesNotExistException();
		}

		public double CalculateRT(pwiz.CLI.msdata.Spectrum spectrum, TimeUnit timeUnits)
		{
			double rt = Math.Round((double)spectrum.scanList.scans[0].cvParam(pwiz.CLI.CVID.MS_scan_start_time).value, 5);

			if (timeUnits == TimeUnit.Seconds)
			{
				rt = rt / NumberOfSecondsInAMinute;
			}

			if (timeUnits == TimeUnit.Hours)
			{
				rt = rt * NumberOfSecondsInAMinute;
			}

			return rt;
		}
	}
}
