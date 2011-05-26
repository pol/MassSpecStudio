using System.IO;
using Hydra.Core.Domain;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ProteoWizard.MassSpecStudio.DataProvider;

namespace Hydra.Test.Core.Domain
{
	[TestClass]
	public class RunResultTest
	{
		private Mock<IServiceLocator> mockServiceLocator;
		private Mock<IEventAggregator> mockEventAggregator;
		private Run run;
		private Experiment experiment;

		[TestInitialize]
		public void TestInitialize()
		{
			mockServiceLocator = new Mock<IServiceLocator>();
			mockEventAggregator = new Mock<IEventAggregator>();
			experiment = TestHelper.GetTestExperiment(mockServiceLocator);
			run = new Run(Path.GetFileName(Properties.Settings.Default.mzXMLTestFile1), Properties.Settings.Default.mzXMLTestFile1, new ProteinState(experiment), new Labeling(experiment), experiment, new ProteoWizardDataProvider(mockEventAggregator.Object));
		}

		[TestMethod]
		public void BlankConstructor()
		{
			RunResult runResult = new RunResult();
			Assert.AreEqual(0, runResult.IsotopicPeakList.Count);
			Assert.AreEqual(0, runResult.TheoreticalIsotopicPeakList.Count);
		}

		[TestMethod]
		public void PeptideConstructor()
		{
			RunResult runResult = new RunResult(new Peptide("SAM") { PeaksInCalculation = 1, DeuteriumDistributionThreshold = 2 }, run);
			Assert.AreEqual("SAM", runResult.Peptide.Sequence);
			Assert.AreEqual(run, runResult.Run);
			Assert.AreEqual(1, runResult.ActualPeaksInCalculation);
			Assert.AreEqual(2, runResult.ActualDeutDistThreshold);
			Assert.AreEqual(false, runResult.IsResultBasedOnFragment);
		}

		[TestMethod]
		public void FragmentConstructor()
		{
			RunResult runResult = new RunResult(new FragmentIon("S", new Peptide("SAM")) { PeaksInCalculation = 1, DeutDistThreshold = 2 }, run);
			Assert.AreEqual("S", runResult.FragmentIon.Sequence);
			Assert.AreEqual("SAM", runResult.Peptide.Sequence);
			Assert.AreEqual(run, runResult.Run);
			Assert.AreEqual(1, runResult.ActualPeaksInCalculation);
			Assert.AreEqual(2, runResult.ActualDeutDistThreshold);
			Assert.AreEqual(true, runResult.IsResultBasedOnFragment);
		}

		[TestMethod]
		public void GetProperties()
		{
			RunResult runResult = new RunResult();
			runResult.ActualDeutDistThreshold = 1;
			runResult.ActualPeaksInCalculation = 2;
			runResult.AverageMass = 3;
			runResult.TheoreticalAverageMass = 4;
			runResult.CentroidMR = 5;
			runResult.TheoreticalCentroidMR = 6;
			runResult.AmideHydrogenTotal = 7;
			runResult.AmountDeut = 8;
			runResult.IsUsedInCalculations = true;
			runResult.IsResultBasedOnFragment = true;
			runResult.FragmentIon = new FragmentIon();

			Assert.AreEqual(1, runResult.ActualDeutDistThreshold);
			Assert.AreEqual(2, runResult.ActualPeaksInCalculation);
			Assert.AreEqual(3, runResult.AverageMass);
			Assert.AreEqual(4, runResult.TheoreticalAverageMass);
			Assert.AreEqual(5, runResult.CentroidMR);
			Assert.AreEqual(6, runResult.TheoreticalCentroidMR);
			Assert.AreEqual(7, runResult.AmideHydrogenTotal);
			Assert.AreEqual(8, runResult.AmountDeut);
			Assert.AreEqual(true, runResult.IsResultBasedOnFragment);
			Assert.IsNotNull(runResult.FragmentIon);
			Assert.AreEqual(true, runResult.IsUsedInCalculations);
		}
	}
}
