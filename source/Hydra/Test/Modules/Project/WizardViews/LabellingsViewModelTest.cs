using System.Linq;
using Hydra.Core.Domain;
using Hydra.Modules.Project.Views;
using Hydra.Modules.Project.WizardViews;
using MassSpecStudio.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hydra.Test.Modules.Project.WizardViews
{
	[TestClass]
	public class LabellingsViewModelTest
	{
		private LabellingsViewModel viewModel;
		private ExperimentViewModel experimentViewModel;

		[TestInitialize]
		public void TestInitialize()
		{
			ProjectBase project = TestHelper.CreateTestProject();
			Experiment experiment = project.Experiments.First() as Experiment;
			experimentViewModel = new ExperimentViewModel(experiment);
			viewModel = new LabellingsViewModel(experimentViewModel);
		}

		[TestMethod]
		public void Properties()
		{
			Assert.AreEqual("Add Labelings", viewModel.Title);
			Assert.AreEqual(experimentViewModel.Samples, viewModel.ViewModel);
			Assert.AreEqual(experimentViewModel.Samples.Children.First(), viewModel.FirstProteinState);
			Assert.AreEqual(typeof(LabellingsView), viewModel.ViewType);
		}

		[TestMethod]
		public void SelectedItem()
		{
			Assert.IsNull(viewModel.SelectedItem);
			Assert.AreEqual(false, viewModel.Remove.CanExecute(null));
			viewModel.SelectedItem = viewModel.FirstProteinState.Children.First();
			Assert.AreEqual(true, viewModel.Remove.CanExecute(null));
			Assert.IsNotNull(viewModel.SelectedItem);
		}

		[TestMethod]
		public void CanNext()
		{
			Assert.AreEqual(true, viewModel.CanNext);
			viewModel.FirstProteinState.Children.Clear();
			Assert.AreEqual(false, viewModel.CanNext);
		}

		[TestMethod]
		public void Add()
		{
			Assert.AreEqual(2, viewModel.FirstProteinState.Children.Count);
			Assert.IsNull(viewModel.SelectedItem);
			viewModel.Add.Execute(null);
			Assert.AreEqual(3, viewModel.FirstProteinState.Children.Count);
			Assert.AreEqual("3(40)", viewModel.FirstProteinState.Children.Last().Name);
			Assert.AreEqual(viewModel.FirstProteinState.Children.Last(), viewModel.SelectedItem);
		}

		[TestMethod]
		public void Remove()
		{
			Add();
			Assert.AreEqual(3, viewModel.FirstProteinState.Children.Count);
			Assert.AreEqual(viewModel.FirstProteinState.Children.Last(), viewModel.SelectedItem);
			viewModel.Remove.Execute(viewModel.FirstProteinState.Children.First());
			Assert.AreEqual(2, viewModel.FirstProteinState.Children.Count);
			Assert.AreEqual(viewModel.FirstProteinState.Children.Last(), viewModel.SelectedItem);
		}
	}
}
