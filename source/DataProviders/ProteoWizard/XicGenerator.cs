using System;
using System.Collections.Generic;
using MassSpecStudio.Core;
using MassSpecStudio.Core.DataProvider;
using MassSpecStudio.Core.Domain;
using pwiz.CLI.msdata;
using Domain = MassSpecStudio.Core.Domain;

namespace ProteoWizard.MassSpecStudio.DataProvider
{
	public class XicGenerator
	{
		private readonly Run run;
		private readonly SpectrumCache spectrumCache;
		private readonly SpectrumExtractor spectrumExtractor;

		public XicGenerator(Run run, SpectrumCache spectrumCache)
		{
			this.run = run;
			this.spectrumCache = spectrumCache;
			spectrumExtractor = new SpectrumExtractor(run, spectrumCache);
		}

		public IXYData Generate(double mass1, double mass2, double mzTolerance, TimeUnit timeUnits)
		{
			if (mass1 != mass2)
			{
				throw new ArgumentException("This method only supports getting an XIC for one mass.  Mass1 must be equal to Mass2.");
			}

			pwiz.CLI.msdata.SpectrumList spectrumList = run.spectrumList;

			int timePoints = spectrumList.size();

			List<double> msLevelXVals = new List<double>();
			List<double> msLevelYVals = new List<double>();

			for (int i = 0; i < timePoints; i++)
			{
				IList<double> mzValues = new List<double>();
				IList<double> intensityValues = new List<double>();
				double rt;

				if (spectrumCache.IsInCache(i))
				{
					SpectrumCacheItem cacheItem = spectrumCache.ReadRawCacheItem(i);
					mzValues = cacheItem.Spectrum.XValues;
					intensityValues = cacheItem.Spectrum.YValues;
					rt = cacheItem.Spectrum.StartRT;
				}
				else
				{
					Domain.ISpectrum spectrum = spectrumExtractor.GetSpectrum(i, timeUnits);
					mzValues = spectrum.XValues;
					intensityValues = spectrum.YValues;
					rt = spectrum.StartRT;
				}

				XicHelper.GetIntensities(mzValues, intensityValues, rt, mass1, mzTolerance, msLevelXVals, msLevelYVals);
			}

			IXYData xyData = new XYData(msLevelXVals.ToArray(), msLevelYVals.ToArray());
			xyData.TimeUnit = TimeUnit.Minutes;

			return xyData;
		}
	}
}
