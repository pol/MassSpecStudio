using System;
using System.Diagnostics;
using MassSpecStudio.Core;
using MassSpecStudio.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProteoWizard.MassSpecStudio.DataProvider;
using pwiz.CLI.msdata;
using Domain = MassSpecStudio.Core.Domain;

namespace MassSpecStudio.DataProvider.Test.ProteoWizard
{
	[TestClass]
	public class AveragedSpectrumExtractorTest
	{
		private const int MinimumMillisecondsItShouldTakeToExtractSpectrum = 200;
		private const int MaximumMillisecondsItShouldTakeToExtractSpectrum = 1500000;
		private const int MinimumMillisecondsItShouldTakeToPullSpectrumFromCache = 0;
		private const int MaximumMillisecondsItShouldTakeToPullSpectrumFromCache = 100000;
		private MSDataFile dataFile;
		private SpectrumCache spectrumCache;
		private AveragedSpectrumExtractor averagedSpectrumExtractor;
		private Stopwatch timer;

		[TestInitialize]
		public void MyTestInitialize()
		{
			dataFile = new MSDataFile(Properties.Settings.Default.mzXMLTestFile1);
			spectrumCache = new SpectrumCache();
			averagedSpectrumExtractor = new AveragedSpectrumExtractor(dataFile.run, spectrumCache);

			timer = new Stopwatch();
		}

		[TestCleanup]
		public void MyTestCleanup()
		{
			dataFile.Dispose();
			dataFile = null;
		}

		[TestMethod]
		public void GetAveragedSpectrum()
		{
			timer.Start();
			Domain.ISpectrum spectrum = averagedSpectrumExtractor.GetAveragedSpectrum(1.5, 1.6, TimeUnit.Seconds);
			timer.Stop();

			XYDataHelper.AssertSpectrum(spectrum, 1.50005, 1.60005, 40383, timer, MinimumMillisecondsItShouldTakeToExtractSpectrum, MaximumMillisecondsItShouldTakeToExtractSpectrum);
			XYDataHelper.AssertValue(spectrum, 300, 308.08392, 2);
		}

		[TestMethod]
		public void GetAveragedSpectrumFromCache()
		{
			averagedSpectrumExtractor.GetAveragedSpectrum(1.5, 1.6, TimeUnit.Seconds);

			timer.Reset();
			timer.Start();
			Domain.ISpectrum spectrum = averagedSpectrumExtractor.GetAveragedSpectrum(1.5, 1.6, TimeUnit.Seconds);
			timer.Stop();

			XYDataHelper.AssertSpectrum(spectrum, 1.50005, 1.60005, 40383, timer, MinimumMillisecondsItShouldTakeToPullSpectrumFromCache, MaximumMillisecondsItShouldTakeToPullSpectrumFromCache);
			XYDataHelper.AssertValue(spectrum, 300, 308.08392, 2);
		}

		[TestMethod]
		public void GetAveragedSpectrumForSpecificMassRange()
		{
			timer.Start();
			Domain.ISpectrum spectrum = averagedSpectrumExtractor.GetAveragedSpectrum(1.5, 1.6, TimeUnit.Seconds, 100, 800);
			timer.Stop();

			XYDataHelper.AssertSpectrum(spectrum, 1.50005, 1.60005, 24526, timer, MinimumMillisecondsItShouldTakeToExtractSpectrum, MaximumMillisecondsItShouldTakeToExtractSpectrum);
			XYDataHelper.AssertValue(spectrum, 300, 308.08392, 2);
		}

		[TestMethod]
		public void GetAveragedSpectrumForSpecificMassRangeFromCache()
		{
			averagedSpectrumExtractor.GetAveragedSpectrum(1.5, 1.6, TimeUnit.Seconds, 100, 800);

			timer.Start();
			Domain.ISpectrum spectrum = averagedSpectrumExtractor.GetAveragedSpectrum(1.5, 1.6, TimeUnit.Seconds, 100, 800);
			timer.Stop();

			XYDataHelper.AssertSpectrum(spectrum, 1.50005, 1.60005, 24526, timer, MinimumMillisecondsItShouldTakeToPullSpectrumFromCache, MaximumMillisecondsItShouldTakeToPullSpectrumFromCache);
			XYDataHelper.AssertValue(spectrum, 300, 308.08392, 2);
		}

		[TestMethod]
		public void GetMSMSSpectrum()
		{
			dataFile = new MSDataFile(Properties.Settings.Default.mzXMLTestFile2);
			spectrumCache = new SpectrumCache();
			averagedSpectrumExtractor = new AveragedSpectrumExtractor(dataFile.run, spectrumCache);
			Domain.ISpectrum msmsSpectrum = averagedSpectrumExtractor.GetMSMSSpectrum(7.8, 1, 592.3, 0.5);

			Assert.AreEqual(42279, msmsSpectrum.Count);
			Assert.AreEqual(37, Math.Round(msmsSpectrum.GetXYPair(63).YValue, 5));
		}
	}
}
