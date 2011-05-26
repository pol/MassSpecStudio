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
	public class MsMsFragmentAlgorithmTest : AlgorithmTestBase
	{
		private MsMsFragmentAlgorithm msmsFragmentAnalyzer;

		[TestInitialize]
		public void MyTestInitialize()
		{
			Initialize();

			msmsFragmentAnalyzer = new MsMsFragmentAlgorithm(
				MockEventAggregator.Object,
				new MsMsSpectrumSelection(MockEventAggregator.Object, MockRegionManager.Object),
				new SavitzkyGolaySmoothing(MockEventAggregator.Object, MockRegionManager.Object),
				new MsMsFragmentAnalyzer(MockEventAggregator.Object),
				new DeuterationResultGenerator(MockEventAggregator.Object),
				MockServiceLocator.Object);
		}

		[TestMethod]
		public void Execute()
		{
			DocumentManager documentManager = new DocumentManager(MockEventAggregator.Object, MockServiceLocator.Object);
			documentManager.Open(Properties.Settings.Default.TestProject2);
			Experiment experiment = documentManager.ProjectFile.Experiments.First() as Experiment;
			BackgroundWorker worker = MockBackgroundWorker.Object;
			worker.WorkerReportsProgress = true;
			msmsFragmentAnalyzer.Execute(worker, experiment);

			Assert.AreEqual(1, experiment.Results.Count);
			Assert.AreEqual(3, experiment.Results.Last().DeuterationResults.Count);
			AssertResult(experiment.Results.Last().DeuterationResults[0].ReplicateResults, -1.14279);
			AssertResult(experiment.Results.Last().DeuterationResults[1].ReplicateResults, -1.33839);
			AssertResult(experiment.Results.Last().DeuterationResults[2].ReplicateResults, -1.18787);
		}

		private void AssertResult(IList<RunResult> results, double amountDeut)
		{
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual(amountDeut, Math.Round(results[0].AmountDeut, 5));
		}
	}
}
