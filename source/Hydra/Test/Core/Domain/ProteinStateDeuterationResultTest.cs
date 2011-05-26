using System.Collections.Generic;
using System.IO;
using Hydra.Core.Domain;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ProteoWizard.MSStudio.DataProvider;

namespace Hydra.Test.Core.Domain
{
	[TestClass]
	public class ProteinStateDeuterationResultTest
	{
		private Mock<IServiceLocator> mockServiceLocator;
		private Mock<IEventAggregator> mockEventAggregator;
		private Run run;
		private Experiment experiment;
		private ProteinStateDeuterationResult proteinStateDeuterationResult;

		[TestInitialize]
		public void TestInitialize()
		{
			mockServiceLocator = new Mock<IServiceLocator>();
			mockEventAggregator = new Mock<IEventAggregator>();
			experiment = TestHelper.GetTestExperiment(mockServiceLocator);
			run = new Run(Path.GetFileName(Properties.Settings.Default.mzXMLTestFile1), Properties.Settings.Default.mzXMLTestFile1, new ProteinState(experiment), new Labeling(experiment), experiment, new ProteoWizardDataProvider(mockEventAggregator.Object));
			proteinStateDeuterationResult = new ProteinStateDeuterationResult();
		}

		[TestMethod]
		public void BlankContructor()
		{
			Assert.AreEqual(0, proteinStateDeuterationResult.DeuterationValues.Count);
			Assert.AreEqual(0, proteinStateDeuterationResult.DeuterationDistributedDeuterationValues.Count);
			Assert.AreEqual(0, proteinStateDeuterationResult.CentroidMassValues.Count);
			Assert.AreEqual(0, proteinStateDeuterationResult.ReplicateResults.Count);
		}

		[TestMethod]
		public void Constructor()
		{
			RunResult runResult1 = CreateRunResult(1, 2, 3, 4, true);
			RunResult runResult2 = CreateRunResult(5, 6, 7, 8, true);

			proteinStateDeuterationResult = new ProteinStateDeuterationResult(new List<RunResult>() { runResult1, runResult2 }, new ProteinState(experiment));

			Assert.AreEqual(5, proteinStateDeuterationResult.TheoreticalCentroidMass);
			Assert.AreEqual(2, proteinStateDeuterationResult.DeuterationValues.Count);
			Assert.AreEqual(2, proteinStateDeuterationResult.DeuterationValues[0]);
			Assert.AreEqual(6, proteinStateDeuterationResult.DeuterationValues[1]);
			Assert.AreEqual(2, proteinStateDeuterationResult.DeuterationDistributedDeuterationValues.Count);
			Assert.AreEqual(3, proteinStateDeuterationResult.DeuterationDistributedDeuterationValues[0]);
			Assert.AreEqual(7, proteinStateDeuterationResult.DeuterationDistributedDeuterationValues[1]);
			Assert.AreEqual(2, proteinStateDeuterationResult.CentroidMassValues.Count);
			Assert.AreEqual(4, proteinStateDeuterationResult.CentroidMassValues[0]);
			Assert.AreEqual(8, proteinStateDeuterationResult.CentroidMassValues[1]);
			Assert.AreEqual(2, proteinStateDeuterationResult.NValue);
		}

		[TestMethod]
		public void ContructorWithNoReplicates()
		{
			proteinStateDeuterationResult = new ProteinStateDeuterationResult(new List<RunResult>(), new ProteinState(experiment));

			Assert.AreEqual(0, proteinStateDeuterationResult.TheoreticalCentroidMass);
			Assert.AreEqual(0, proteinStateDeuterationResult.DeuterationValues.Count);
			Assert.AreEqual(0, proteinStateDeuterationResult.DeuterationDistributedDeuterationValues.Count);
			Assert.AreEqual(0, proteinStateDeuterationResult.CentroidMassValues.Count);
			Assert.AreEqual(0, proteinStateDeuterationResult.NValue);
		}

		[TestMethod]
		public void ContructorWithNoResultsUsedInCalculation()
		{
			RunResult runResult1 = CreateRunResult(1, 2, 3, 4, false);
			RunResult runResult2 = CreateRunResult(5, 6, 7, 8, false);

			proteinStateDeuterationResult = new ProteinStateDeuterationResult(new List<RunResult>() { runResult1, runResult2 }, new ProteinState(experiment));

			Assert.AreEqual(5, proteinStateDeuterationResult.TheoreticalCentroidMass);
			Assert.AreEqual(0, proteinStateDeuterationResult.DeuterationValues.Count);
			Assert.AreEqual(0, proteinStateDeuterationResult.DeuterationDistributedDeuterationValues.Count);
			Assert.AreEqual(0, proteinStateDeuterationResult.CentroidMassValues.Count);
			Assert.AreEqual(0, proteinStateDeuterationResult.NValue);
		}

		[TestMethod]
		public void GetProperties()
		{
			proteinStateDeuterationResult.AmountDeut = 1;
			proteinStateDeuterationResult.AmountDeuterationFromDeuterationDistribution = 2;
			proteinStateDeuterationResult.AmountDeuterationFromDeuterationDistributionStandardDeviation = 3;
			proteinStateDeuterationResult.AmountDeuterationStandardDeviation = 4;
			proteinStateDeuterationResult.CentroidMass = 5;
			proteinStateDeuterationResult.CentroidMassStandardDeviation = 6;
			proteinStateDeuterationResult.NValue = 7;
			proteinStateDeuterationResult.TheoreticalCentroidMass = 8;

			Assert.AreEqual(1, proteinStateDeuterationResult.AmountDeut);
			Assert.AreEqual(2, proteinStateDeuterationResult.AmountDeuterationFromDeuterationDistribution);
			Assert.AreEqual(3, proteinStateDeuterationResult.AmountDeuterationFromDeuterationDistributionStandardDeviation);
			Assert.AreEqual(4, proteinStateDeuterationResult.AmountDeuterationStandardDeviation);
			Assert.AreEqual(5, proteinStateDeuterationResult.CentroidMass);
			Assert.AreEqual(6, proteinStateDeuterationResult.CentroidMassStandardDeviation);
			Assert.AreEqual(7, proteinStateDeuterationResult.NValue);
			Assert.AreEqual(8, proteinStateDeuterationResult.TheoreticalCentroidMass);
		}

		private RunResult CreateRunResult(double theoreticalAverageMass, double amountDeut, double amountDeutFromDeutDist, double averageMass, bool isUsedInCalculations)
		{
			RunResult runResult = new RunResult(new Peptide(), run);
			runResult.TheoreticalAverageMass = theoreticalAverageMass;
			runResult.AmountDeut = amountDeut;
			runResult.AmountDeutFromDeutDist = amountDeutFromDeutDist;
			runResult.AverageMass = averageMass;
			runResult.IsUsedInCalculations = isUsedInCalculations;
			return runResult;
		}
	}
}
