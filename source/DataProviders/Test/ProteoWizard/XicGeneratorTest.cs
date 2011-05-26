using System;
using System.Diagnostics;
using MassSpecStudio.Core;
using MassSpecStudio.Core.Domain;
using MassSpecStudio.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProteoWizard.MassSpecStudio.DataProvider;
using pwiz.CLI.msdata;

namespace MassSpecStudio.DataProvider.Test.ProteoWizard
{
	[TestClass()]
	public class XicGeneratorTest
	{
		private const int MinimumMillisecondsItShouldTakeToGenerateXic = 5000;
		private const int MaximumMillisecondsItShouldTakeToGenerateXic = 25000;
		private const int MinimumMillisecondsItShouldTakeToPullXicFromCache = 0;
		private const int MaximumMillisecondsItShouldTakeToPullXicFromCache = 1500;
		private XicGenerator xicGenerator;
		private SpectrumCache spectrumCache;
		private Stopwatch timer;
		private MSDataFile dataFile;

		[TestInitialize()]
		public void MyTestInitialize()
		{
			dataFile = new MSDataFile(Properties.Settings.Default.mzXMLTestFile1);
			spectrumCache = new SpectrumCache();
			xicGenerator = new XicGenerator(dataFile.run, spectrumCache);

			timer = new Stopwatch();
		}

		[TestCleanup()]
		public void MyTestCleanup()
		{
			dataFile.Dispose();
			dataFile = null;
		}

		[TestMethod]
		public void Generate()
		{
			timer.Start();
			IXYData xic = xicGenerator.Generate(592.3, 592.3, 0.01, TimeUnit.Seconds);
			timer.Stop();

			XYDataHelper.AssertChromatogram(xic, 1500, timer, MinimumMillisecondsItShouldTakeToGenerateXic, MaximumMillisecondsItShouldTakeToGenerateXic);
			XYDataHelper.AssertValue(xic, 454, 7.5832, 142);
		}

		[TestMethod]
		public void GenerateFromCache()
		{
			xicGenerator.Generate(592.3, 592.3, 0.01, TimeUnit.Seconds);

			timer.Start();
			IXYData xic = xicGenerator.Generate(592.3, 592.3, 0.01, TimeUnit.Seconds);
			timer.Stop();

			XYDataHelper.AssertChromatogram(xic, 1500, timer, MinimumMillisecondsItShouldTakeToPullXicFromCache, MaximumMillisecondsItShouldTakeToPullXicFromCache);
			XYDataHelper.AssertValue(xic, 454, 7.5832, 142);
		}

		[TestMethod]
		public void GenerateABlankXic()
		{
			timer.Start();
			IXYData xic = xicGenerator.Generate(200, 200, 0.0001, TimeUnit.Seconds);
			timer.Stop();

			XYDataHelper.AssertChromatogram(xic, 1500, timer, MinimumMillisecondsItShouldTakeToGenerateXic, MaximumMillisecondsItShouldTakeToGenerateXic);
			XYDataHelper.AssertValue(xic, 454, 7.5832, 0);
		}

		[TestMethod]
		public void GenerateFromMSMS()
		{
			dataFile = new MSDataFile(Properties.Settings.Default.mzXMLTestFile2);
			spectrumCache = new SpectrumCache();
			xicGenerator = new XicGenerator(dataFile.run, spectrumCache);

			IXYData xic = xicGenerator.Generate(592.3, 592.3, 0.01, TimeUnit.Seconds);

			Assert.AreEqual(239, xic.XValues.Count);
			Assert.AreEqual(7.6804, Math.Round(xic.GetXYPair(168).XValue, 10));
			Assert.AreEqual(519, xic.GetXYPair(168).YValue);
		}
	}
}
