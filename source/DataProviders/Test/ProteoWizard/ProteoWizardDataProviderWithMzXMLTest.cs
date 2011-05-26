using MassSpecStudio.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProteoWizard.MassSpecStudio.DataProvider;

namespace MassSpecStudio.DataProvider.Test.ProteoWizard
{
	[TestClass]
	public class ProteoWizardDataProviderWithMzXMLTest : DataProviderBaseTest
	{
		public ProteoWizardDataProviderWithMzXMLTest()
			: base(Properties.Settings.Default.mzXMLTestFile1, Properties.Settings.Default.mzXMLTestFile2)
		{
		}

		public override IDataProvider GetNewDataProvider()
		{
			return new ProteoWizardDataProvider(MockEventAggregator.Object);
		}

		[TestInitialize]
		public override void TestInitialize()
		{
			base.TestInitialize();
		}

		[TestCleanup]
		public override void TestCleanup()
		{
			base.TestCleanup();
		}

		[TestMethod]
		public override void GetSampleList()
		{
			base.GetSampleList();
		}

		[TestMethod]
		public override void GetTotalIonChromatogram()
		{
			base.GetTotalIonChromatogram();
		}

		[TestMethod]
		public override void GetExtractedIonChromatogram()
		{
			base.GetExtractedIonChromatogram();
		}

		[TestMethod]
		public override void GetSpectrum()
		{
			base.GetSpectrum();
		}

		[TestMethod]
		public override void GetAveragedSpectrum()
		{
			base.GetAveragedSpectrum();
		}

		[TestMethod]
		public override void GetMsMsSpectrum()
		{
			base.GetMsMsSpectrum();
		}
	}
}
