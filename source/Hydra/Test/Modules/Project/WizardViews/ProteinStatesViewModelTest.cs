using System.Linq;
using Hydra.Core.Domain;
using Hydra.Modules.Project.Views;
using Hydra.Modules.Project.WizardViews;
using MassSpecStudio.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hydra.Test.Modules.Project.WizardViews
{
	[TestClass]
	public class ProteinStatesViewModelTest
	{
		private ProteinStatesViewModel viewModel;
		private ExperimentViewModel experimentViewModel;

		[TestInitialize]
		public void TestInitialize()
		{
			ProjectBase project = TestHelper.CreateTestProject();
			Experiment experiment = project.Experiments.First() as Experiment;
			experimentViewModel = new ExperimentViewModel(experiment);
			viewModel = new ProteinStatesViewModel(experimentViewModel);
		}

		[TestMethod]
		public void Properties()
		{
			Assert.AreEqual("Add Protein States", viewModel.Title);
			Assert.AreEqual(experimentViewModel.Samples, viewModel.ViewModel);
			Assert.AreEqual(typeof(ProteinStatesView), viewModel.ViewType);
		}

		[TestMethod]
		public void SelectedItem()
		{
			Assert.IsNull(viewModel.SelectedItem);
			Assert.AreEqual(false, viewModel.Remove.CanExecute(null));
			viewModel.SelectedItem = viewModel.ViewModel.Children.First();
			Assert.AreEqual(true, viewModel.Remove.CanExecute(null));
			Assert.IsNotNull(viewModel.SelectedItem);
		}

		[TestMethod]
		public void CanNext()
		{
			Assert.AreEqual(true, viewModel.CanNext);
			viewModel.ViewModel.Children.Clear();
			Assert.AreEqual(false, viewModel.CanNext);
		}

		[TestMethod]
		public void CanBack()
		{
			Assert.AreEqual(false, viewModel.CanBack);
		}

		[TestMethod]
		public void Add()
		{
			Assert.AreEqual(2, viewModel.ViewModel.Children.Count);
			Assert.IsNull(viewModel.SelectedItem);
			viewModel.Add.Execute(null);
			Assert.AreEqual(3, viewModel.ViewModel.Children.Count);
			Assert.AreEqual("Protein state 3", viewModel.ViewModel.Children.Last().Name);
			Assert.AreEqual(viewModel.ViewModel.Children.Last(), viewModel.SelectedItem);
		}

		[TestMethod]
		public void Remove()
		{
			Add();
			Assert.AreEqual(3, viewModel.ViewModel.Children.Count);
			Assert.AreEqual(viewModel.ViewModel.Children.Last(), viewModel.SelectedItem);
			viewModel.Remove.Execute(viewModel.ViewModel.Children.First());
			Assert.AreEqual(2, viewModel.ViewModel.Children.Count);
			Assert.AreEqual(viewModel.ViewModel.Children.Last(), viewModel.SelectedItem);
		}
	}
}