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
	public class SpectrumExtractorTest
	{
		private const int MinimumMillisecondsItShouldTakeToExtractSpectrum = 0;
		private const int MaximumMillisecondsItShouldTakeToExtractSpectrum = 1500;
		private const int MinimumMillisecondsItShouldTakeToPullSpectrumFromCache = 0;
		private const int MaximumMillisecondsItShouldTakeToPullSpectrumFromCache = 500;

		private const int MinimumMillisecondsItShouldTakeToExtractSpectrumViaRt = 5000;
		private const int MaximumMillisecondsItShouldTakeToExtractSpectrumViaRt = 25000;
		private const int MinimumMillisecondsItShouldTakeToPullSpectrumFromCacheViaRt = 0;
		private const int MaximumMillisecondsItShouldTakeToPullSpectrumFromCacheViaRt = 500;

		private MSDataFile dataFile;
		private SpectrumCache spectrumCache;
		private SpectrumExtractor spectrumExtractor;
		private Stopwatch timer;

		[TestInitialize]
		public void MyTestInitialize()
		{
			dataFile = new MSDataFile(Properties.Settings.Default.mzXMLTestFile1);
			spectrumCache = new SpectrumCache();
			spectrumExtractor = new SpectrumExtractor(dataFile.run, spectrumCache);

			timer = new Stopwatch();
		}

		[TestCleanup]
		public void MyTestCleanup()
		{
			dataFile.Dispose();
			dataFile = null;
		}

		[TestMethod]
		public void GetSpectrum()
		{
			timer.Start();
			Domain.ISpectrum spectrum = spectrumExtractor.GetSpectrum(100, TimeUnit.Seconds);
			timer.Stop();

			XYDataHelper.AssertSpectrum(spectrum, 1.68335, 12674, timer, MinimumMillisecondsItShouldTakeToExtractSpectrum, MaximumMillisecondsItShouldTakeToExtractSpectrum);
		}

		[TestMethod]
		public void GetSpectrumFromCache()
		{
			spectrumExtractor.GetSpectrum(100, TimeUnit.Seconds);

			timer.Reset();
			timer.Start();
			Domain.ISpectrum spectrum = spectrumExtractor.GetSpectrum(100, TimeUnit.Seconds);
			timer.Stop();

			XYDataHelper.AssertSpectrum(spectrum, 1.68335, 12674, timer, MinimumMillisecondsItShouldTakeToPullSpectrumFromCache, MaximumMillisecondsItShouldTakeToPullSpectrumFromCache);
		}

		[TestMethod]
		public void GetSpectrumForSpecificMassRange()
		{
			timer.Start();
			Domain.ISpectrum spectrum = spectrumExtractor.GetSpectrum(100, TimeUnit.Seconds, 100, 800);
			timer.Stop();

			XYDataHelper.AssertSpectrum(spectrum, 1.68335, 9645, timer, MinimumMillisecondsItShouldTakeToExtractSpectrum, MaximumMillisecondsItShouldTakeToExtractSpectrum);
		}

		[TestMethod]
		public void GetSpectrumUsingRt()
		{
			timer.Start();
			Domain.ISpectrum spectrum = spectrumExtractor.GetSpectrum(1.68335, TimeUnit.Seconds);
			timer.Stop();

			XYDataHelper.AssertSpectrum(spectrum, 1.68335, 12674, timer, MinimumMillisecondsItShouldTakeToExtractSpectrumViaRt, MaximumMillisecondsItShouldTakeToExtractSpectrumViaRt);
		}

		[TestMethod]
		public void GetSpectrumUsingRtFromCache()
		{
			spectrumExtractor.GetSpectrum(1.68335, TimeUnit.Seconds);

			timer.Reset();
			timer.Start();
			Domain.ISpectrum spectrum = spectrumExtractor.GetSpectrum(1.68335, TimeUnit.Seconds);
			timer.Stop();

			XYDataHelper.AssertSpectrum(spectrum, 1.68335, 12674, timer, MinimumMillisecondsItShouldTakeToPullSpectrumFromCacheViaRt, MaximumMillisecondsItShouldTakeToPullSpectrumFromCacheViaRt);
		}

		[TestMethod]
		public void GetSpectrumUsingRtForSpecificMassRange()
		{
			timer.Start();
			Domain.ISpectrum spectrum = spectrumExtractor.GetSpectrum(1.68335, TimeUnit.Seconds, 100, 800);
			timer.Stop();

			XYDataHelper.AssertSpectrum(spectrum, 1.68335, 9645, timer, MinimumMillisecondsItShouldTakeToExtractSpectrumViaRt, MaximumMillisecondsItShouldTakeToExtractSpectrumViaRt);
		}
	}
}
