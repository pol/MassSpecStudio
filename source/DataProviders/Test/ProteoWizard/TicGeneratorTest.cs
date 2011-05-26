using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using MassSpecStudio.Core;
using MassSpecStudio.Core.Domain;
using MassSpecStudio.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProteoWizard.MassSpecStudio.DataProvider;
using pwiz.CLI.msdata;

namespace MassSpecStudio.DataProvider.Test.ProteoWizard
{
	[TestClass]
	public class TicGeneratorTest
	{
		private const int MinimumMillisecondsItShouldTakeToGenerateTic = 5000;
		private const int MaximumMillisecondsItShouldTakeToGenerateTic = 30000;
		private const int MinimumMillisecondsItShouldTakeToPullTicFromCache = 0;
		private const int MaximumMillisecondsItShouldTakeToPullTicFromCache = 250;
		private TicGenerator ticGenerator;
		private SpectrumCache spectrumCache;
		private Stopwatch timer;
		private MSDataFile dataFile;

		[TestInitialize]
		public void MyTestInitialize()
		{
			dataFile = new MSDataFile(Properties.Settings.Default.mzXMLTestFile1);
			spectrumCache = new SpectrumCache();
			ticGenerator = new TicGenerator(dataFile.run, spectrumCache);

			timer = new Stopwatch();
		}

		[TestCleanup]
		public void MyTestCleanup()
		{
			dataFile.Dispose();
			dataFile = null;
		}

		[TestMethod]
		public void Generate()
		{
			timer.Start();
			IXYData tic = ticGenerator.Generate(TimeUnit.Seconds);
			timer.Stop();

			XYDataHelper.AssertChromatogram(tic, 1500, timer, MinimumMillisecondsItShouldTakeToGenerateTic, MaximumMillisecondsItShouldTakeToGenerateTic);
			XYDataHelper.AssertValue(tic, 0, 0.0167333333, 60847);
			XYDataHelper.AssertValue(tic, 500, 8.3498333333, 35000);
			XYDataHelper.AssertValue(tic, 1000, 16.6828333333, 6321);
		}

		[TestMethod]
		public void GenerateFromCache()
		{
			IXYData uncachedTic = ticGenerator.Generate(TimeUnit.Seconds);

			timer.Reset();
			timer.Start();
			IXYData cachedTic = ticGenerator.Generate(TimeUnit.Seconds);

			XYDataHelper.AssertChromatogram(cachedTic, 1500, timer, MinimumMillisecondsItShouldTakeToPullTicFromCache, MaximumMillisecondsItShouldTakeToPullTicFromCache);

			IList<double> cachedXValues = cachedTic.XValues;
			IList<double> cachedYValues = cachedTic.YValues;
			IList<double> uncachedXValues = uncachedTic.XValues;
			IList<double> uncachedYValues = uncachedTic.YValues;
			for (int i = 0; i < uncachedXValues.Count; i++)
			{
				Assert.AreEqual(cachedXValues[i], uncachedXValues[i]);
				Assert.AreEqual(cachedYValues[i], uncachedYValues[i]);
			}
		}
	}
}
