using MassSpecStudio.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProteoWizard.MassSpecStudio.DataProvider;
using pwiz.CLI.msdata;

namespace MassSpecStudio.DataProvider.Test.ProteoWizard
{
	[TestClass]
	public class RtToTimePointConverterTest
	{
		private const int MinimumMillisecondsItShouldTakeToGenerateTic = 10000;
		private const int MaximumMillisecondsItShouldTakeToGenerateTic = 20000;
		private const int MinimumMillisecondsItShouldTakeToPullTicFromCache = 0;
		private const int MaximumMillisecondsItShouldTakeToPullTicFromCache = 250;
		private RtToTimePointConverter timePointToRtConverter;
		private SpectrumCache spectrumCache;
		private MSDataFile dataFile;

		[TestInitialize]
		public void MyTestInitialize()
		{
			dataFile = new MSDataFile(Properties.Settings.Default.mzXMLTestFile1);
			spectrumCache = new SpectrumCache();
			timePointToRtConverter = new RtToTimePointConverter(dataFile.run, spectrumCache);
		}

		[TestCleanup]
		public void MyTestCleanup()
		{
			dataFile.Dispose();
			dataFile = null;
		}

		[TestMethod]
		public void Convert()
		{
			Assert.AreEqual(100, timePointToRtConverter.Convert(1.68335, TimeUnit.Seconds));
		}

		[TestMethod]
		[ExpectedException(typeof(RtDoesNotExistException))]
		public void ConvertInvalidRt()
		{
			timePointToRtConverter.Convert(100.5, TimeUnit.Seconds);
		}

		[TestMethod]
		public void ConvertForMSMS()
		{
			SetupMSMSData();
			Assert.AreEqual(483, timePointToRtConverter.ConvertForMSMS(7.3, TimeUnit.Seconds));
		}

		[TestMethod]
		[ExpectedException(typeof(RtDoesNotExistException))]
		public void ConvertForMSMSInvalidRt()
		{
			Assert.AreEqual(1, timePointToRtConverter.ConvertForMSMS(100.5, TimeUnit.Seconds));
		}

		[TestMethod]
		public void CalculateRT()
		{
			Assert.AreEqual(7.3832, timePointToRtConverter.CalculateRT(dataFile.run.spectrumList.spectrum(442), TimeUnit.Seconds));
		}

		private void SetupMSMSData()
		{
			dataFile = new MSDataFile(Properties.Settings.Default.mzXMLTestFile2);
			spectrumCache = new SpectrumCache();
			timePointToRtConverter = new RtToTimePointConverter(dataFile.run, spectrumCache);
		}
	}
}
