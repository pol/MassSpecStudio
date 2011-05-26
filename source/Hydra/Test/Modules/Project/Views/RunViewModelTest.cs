using System.Linq;
using Hydra.Core.Domain;
using Hydra.Modules.Project.Views;
using MassSpecStudio.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hydra.Test.Modules.Project.Views
{
	[TestClass]
	public class RunViewModelTest
	{
		private ProjectBase project;
		private RunViewModel viewModel;

		[TestInitialize]
		public void TestInitialize()
		{
			project = TestHelper.CreateTestProject();
			viewModel = new RunViewModel(((Experiment)project.Experiments.First()).Runs.First());
		}

		[TestMethod]
		public void Name()
		{
			Assert.AreEqual("file07 - run1 - apo-CaM 50%0025D2O trial1.wiff.mzXML", viewModel.Name);
		}

		[TestMethod]
		public void Location()
		{
			Assert.AreEqual("file07 - run1 - apo-CaM 50%0025D2O trial1.wiff.mzXML", viewModel.Name);
		}

		[TestMethod]
		public void Remove()
		{
			Experiment experiment = project.Experiments.First() as Experiment;
			LabelingViewModel labelingViewModel = new LabelingViewModel(experiment.Labeling.First(), experiment.ProteinStates.First(), experiment);
			Assert.AreEqual(2, labelingViewModel.Children.Count);

			viewModel = labelingViewModel.Children.First();
			viewModel.Remove.Execute(viewModel);

			Assert.AreEqual(1, labelingViewModel.Children.Count);
		}
	}
}
