using System.Linq;
using Hydra.Core.Domain;
using Hydra.Modules.Project.Views;
using Hydra.Modules.Project.WizardViews;
using MassSpecStudio.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hydra.Test.Modules.Project.WizardViews
{
	[TestClass]
	public class ImportPeptidesViewModelTest
	{
		private ProjectBase project;
		private ExperimentViewModel viewModel;
		private ImportPeptidesViewModel importPeptidesViewModel;

		[TestInitialize]
		public void TestInitialize()
		{
			project = TestHelper.CreateTestProject();
			viewModel = new ExperimentViewModel(project.Experiments.First() as Experiment);
			importPeptidesViewModel = new ImportPeptidesViewModel(viewModel);
		}

		[TestMethod]
		public void Title()
		{
			Assert.AreEqual("Import peptides", importPeptidesViewModel.Title);
		}

		[TestMethod]
		public void ViewModel()
		{
			Assert.AreEqual(viewModel.Samples, importPeptidesViewModel.ViewModel);
		}

		[TestMethod]
		public void SelectedFileName()
		{
			importPeptidesViewModel.SelectedFileName = string.Empty;
			importPeptidesViewModel.RecentDataLocations = new string[0];
			Assert.AreEqual(string.Empty, importPeptidesViewModel.SelectedFileName);
			Assert.AreEqual(false, importPeptidesViewModel.CanNext);
			Assert.AreEqual(0, importPeptidesViewModel.RecentDataLocations.Length);

			importPeptidesViewModel.SelectedFileName = @"c:\temp";

			Assert.AreEqual(@"c:\temp", importPeptidesViewModel.SelectedFileName);
			Assert.AreEqual(true, importPeptidesViewModel.CanNext);
			Assert.AreEqual(1, importPeptidesViewModel.RecentDataLocations.Length);
			Assert.AreEqual(@"c:\temp", importPeptidesViewModel.RecentDataLocations[0]);
		}

		[TestMethod]
		public void RecentDataLocations()
		{
			importPeptidesViewModel.RecentDataLocations = new string[1] { "test" };

			Assert.AreEqual(1, importPeptidesViewModel.RecentDataLocations.Length);
		}

		[TestMethod]
		public void NextButtonText()
		{
			Assert.AreEqual("Finish", importPeptidesViewModel.NextButtonText);
		}

		[TestMethod]
		public void ViewType()
		{
			Assert.AreEqual(typeof(ImportPeptidesView), importPeptidesViewModel.ViewType);
		}

		[TestMethod]
		public void OnNext()
		{
			importPeptidesViewModel.SelectedFileName = @"Data\testPeptides.xml";
			Assert.AreEqual(null, ((Experiment)viewModel.Data).PeptideFilePendingImport);
			importPeptidesViewModel.OnNext();
			Assert.AreEqual(@"Data\testPeptides.xml", ((Experiment)viewModel.Data).PeptideFilePendingImport);
		}
	}
}
