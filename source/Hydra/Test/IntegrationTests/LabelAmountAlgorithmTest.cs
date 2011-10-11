using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DeconTools.MassSpecStudio.Processing.Steps;
using Hydra.Core.Domain;
using Hydra.Processing.Algorithm;
using Hydra.Processing.Algorithm.Steps;
using MassSpecStudio.Core;
using MassSpecStudio.Processing.Steps;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hydra.Test.IntegrationTests
{
	[TestClass]
	public class LabelAmountAlgorithmTest : AlgorithmTestBase
	{
		private LabelAmountAlgorithm labelAmountCalculator;

		[TestInitialize]
		public void MyTestInitialize()
		{
			Initialize();

			IsotopicProfileFinder finder = new IsotopicProfileFinder(MockEventAggregator.Object);
			finder.PeakWidthMaximum = 0.5;
			finder.MSPeakSelectionOption = Hydra.Core.MSPeakSelectionOption.MostIntenseWithinMzVariation;

			labelAmountCalculator = new LabelAmountAlgorithm(
				MockEventAggregator.Object,
				new XicSelection(MockEventAggregator.Object, MockRegionManager.Object),
				new ChromatographicPeakDetection(MockEventAggregator.Object),
				new XicPeakPicker(MockEventAggregator.Object),
				new SpectrumSelection(MockEventAggregator.Object, MockRegionManager.Object),
				new SpectralPeakDetection(MockEventAggregator.Object),
				finder,
				new LabelAmountCalculator(MockEventAggregator.Object),
				new SavitzkyGolaySmoothing(MockEventAggregator.Object, MockRegionManager.Object),
				new SavitzkyGolaySmoothing(MockEventAggregator.Object, MockRegionManager.Object),
				new DeuterationResultGenerator(MockEventAggregator.Object),
				MockServiceLocator.Object);
		}

		[TestMethod]
		public void Execute()
		{
			DocumentManager documentManager = new DocumentManager(MockEventAggregator.Object, MockServiceLocator.Object);
			documentManager.Open(Properties.Settings.Default.TestProject1);
			Experiment experiment = documentManager.ProjectFile.Experiments.First() as Experiment;
			BackgroundWorker worker = MockBackgroundWorker.Object;
			worker.WorkerReportsProgress = true;
			labelAmountCalculator.Execute(worker, experiment);

			Assert.AreEqual(2, experiment.Results.Count);
			Assert.AreEqual(10, experiment.Results.Last().DeuterationResults.Count);
			AssertResult(experiment.Results.Last().DeuterationResults[0].ReplicateResults, 3.01881, 4.90037);
			AssertResult(experiment.Results.Last().DeuterationResults[1].ReplicateResults, 2.56598, 2.65459);
			AssertResult(experiment.Results.Last().DeuterationResults[2].ReplicateResults, 0.51985, 0.46396);
			AssertResult(experiment.Results.Last().DeuterationResults[3].ReplicateResults, 0.46805, 0.47907);
			AssertResult(experiment.Results.Last().DeuterationResults[4].ReplicateResults, 3.63919, 3.58074);
			AssertResult(experiment.Results.Last().DeuterationResults[5].ReplicateResults, 3.5442, 3.51598);
			AssertResult(experiment.Results.Last().DeuterationResults[6].ReplicateResults, 1.09245, 1.22908);
			AssertResult(experiment.Results.Last().DeuterationResults[7].ReplicateResults, 1.32974, 0.85082);
			AssertResult(experiment.Results.Last().DeuterationResults[9].ReplicateResults, 1.09374, 0.63204);
		}

		[TestMethod]
		[Ignore]
		public void ExecuteWithNoPeptides()
		{
		}

		[TestMethod]
		[Ignore]
		public void ExecuteWithNoProteinStates()
		{
		}

		[TestMethod]
		[Ignore]
		public void ExecuteWithNoLabels()
		{
		}

		[TestMethod]
		[Ignore]
		public void ExecuteWithNoRuns()
		{
		}

		[TestMethod]
		[Ignore]
		public void ExecuteWithNoRunsInAProteinState()
		{
		}

		[TestMethod]
		[Ignore]
		public void ExecuteWhenNoXicFound()
		{
		}

		private void AssertResult(IList<RunResult> results, double amountDeut1, double amountDeut2)
		{
			Assert.AreEqual(2, results.Count);
			Assert.AreEqual(amountDeut1, Math.Round(results[0].AmountDeut, 5));
			Assert.AreEqual(amountDeut2, Math.Round(results[1].AmountDeut, 5));
		}
	}
}
