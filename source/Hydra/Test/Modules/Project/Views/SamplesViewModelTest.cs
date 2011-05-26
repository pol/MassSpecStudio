using System.Linq;
using Hydra.Core.Domain;
using Hydra.Modules.Project.Views;
using MassSpecStudio.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hydra.Test.Modules.Project.Views
{
	[TestClass]
	public class SamplesViewModelTest
	{
		private ProjectBase project;
		private ProjectViewModel viewModel;
		private SamplesViewModel samplesViewModel;
		private Experiment experiment;

		[TestInitialize]
		public void TestInitialize()
		{
			project = TestHelper.CreateTestProject();
			viewModel = new ProjectViewModel(project);
			samplesViewModel = viewModel.Children.First().Children.First() as SamplesViewModel;
			experiment = (Experiment)project.Experiments.First();
		}

		[TestMethod]
		public void Properties()
		{
			Assert.AreEqual("testExperiment", samplesViewModel.Name);
			samplesViewModel.Name = "test";
			Assert.AreEqual("test", samplesViewModel.Name);
			Assert.AreEqual("test", experiment.Name);
		}

		[TestMethod]
		public void AddProteinState()
		{
			Assert.AreEqual(2, samplesViewModel.Children.Count);

			samplesViewModel.AddProteinState.Execute(null);

			Assert.AreEqual(3, samplesViewModel.Children.Count);
			Assert.AreEqual(samplesViewModel, samplesViewModel.Children[2].Parent);
			Assert.AreEqual(true, samplesViewModel.Children[2].IsExpanded);
			Assert.AreEqual("Protein state 3", samplesViewModel.Children[2].Name);
			Assert.AreEqual(3, experiment.ProteinStates.Count);
			Assert.AreEqual("Protein state 3", experiment.ProteinStates[2].Name);
		}

		[TestMethod]
		public void RemoveProteinState()
		{
			Assert.AreEqual(2, samplesViewModel.Children.Count);
			Assert.AreEqual(2, samplesViewModel.Children.First().Children.First().Children.Count);
			Assert.AreEqual(4, experiment.Runs.Count);

			samplesViewModel.RemoveProteinState.Execute(samplesViewModel.Children.First());

			Assert.AreEqual(1, samplesViewModel.Children.Count);
			Assert.AreEqual(1, experiment.ProteinStates.Count);
			Assert.AreEqual(1, experiment.Runs.Count);
		}

		[TestMethod]
		public void AddLabeling()
		{
			ProteinStateViewModel proteinStateViewModel1 = samplesViewModel.Children[0];
			ProteinStateViewModel proteinStateViewModel2 = samplesViewModel.Children[1];
			Assert.AreEqual(2, proteinStateViewModel1.Children.Count);
			Assert.AreEqual(2, proteinStateViewModel2.Children.Count);

			samplesViewModel.AddLabeling.Execute(null);

			Assert.AreEqual(3, proteinStateViewModel1.Children.Count);
			Assert.AreEqual(3, proteinStateViewModel2.Children.Count);
			Assert.AreEqual(proteinStateViewModel1, proteinStateViewModel1.Children[2].Parent);
			Assert.AreEqual(false, proteinStateViewModel1.Children[2].IsExpanded);
			Assert.AreEqual("3(40)", proteinStateViewModel1.Children[2].Name);
			Assert.AreEqual(3, experiment.Labeling.Count);
			Assert.AreEqual(40, experiment.Labeling[2].LabelingPercent);
			Assert.AreEqual(3, experiment.Labeling[2].LabelingTime);
		}

		[TestMethod]
		public void RemoveLabeling()
		{
			ProteinStateViewModel proteinStateViewModel1 = samplesViewModel.Children[0];
			ProteinStateViewModel proteinStateViewModel2 = samplesViewModel.Children[1];
			Assert.AreEqual(2, proteinStateViewModel1.Children.Count);
			Assert.AreEqual(2, proteinStateViewModel2.Children.Count);
			Assert.AreEqual(2, proteinStateViewModel1.Children.First().Children.Count);
			Assert.AreEqual(4, experiment.Runs.Count);

			samplesViewModel.RemoveLabeling.Execute(proteinStateViewModel1.Children.First());

			Assert.AreEqual(1, proteinStateViewModel1.Children.Count);
			Assert.AreEqual(1, proteinStateViewModel2.Children.Count);
			Assert.AreEqual(1, experiment.Labeling.Count);
			Assert.AreEqual(1, experiment.Runs.Count);
		}
	}
}
