using System.Collections.Generic;
using System.Linq;
using Hydra.Core.Domain;
using Hydra.Modules.Project.Views;
using Hydra.Modules.Project.WizardViews;
using MassSpecStudio.Core;
using MassSpecStudio.Core.Domain;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ProteoWizard.MassSpecStudio.DataProvider;

namespace Hydra.Test.Modules.Project.WizardViews
{
	[TestClass]
	public class RunsViewModelTest
	{
		private RunsViewModel viewModel;
		private ExperimentViewModel experimentViewModel;
		private Mock<IEventAggregator> mockEventAggregator;

		[TestInitialize]
		public void TestInitialize()
		{
			mockEventAggregator = new Mock<IEventAggregator>();
			ProjectBase project = TestHelper.CreateTestProject();
			Experiment experiment = project.Experiments.First() as Experiment;
			experiment.DataProvider = new ProteoWizardDataProvider(mockEventAggregator.Object);
			experimentViewModel = new ExperimentViewModel(experiment);
			viewModel = new RunsViewModel(experimentViewModel);
		}

		[TestMethod]
		public void GetProperties()
		{
			Assert.AreEqual("Add Runs", viewModel.Title);
			Assert.AreEqual(experimentViewModel.Samples, viewModel.ViewModel);
			Assert.AreEqual(typeof(RunsView), viewModel.ViewType);
		}

		[TestMethod]
		public void SelectedDataPath()
		{
			viewModel.SelectedDataPath = Properties.Settings.Default.TestRunDataDirectory;
			Assert.AreEqual(Properties.Settings.Default.TestRunDataDirectory, viewModel.SelectedDataPath);
			Assert.AreEqual(2, viewModel.Files.Length);
		}

		[TestMethod]
		public void RecentBrowseLocations()
		{
			viewModel.RecentBrowseLocations = new string[] { @"c:\temp" };
			Assert.AreEqual(1, viewModel.RecentBrowseLocations.Length);
			Assert.AreEqual(@"c:\temp", viewModel.RecentBrowseLocations[0]);
		}

		[TestMethod]
		public void Files()
		{
			viewModel.SelectedDataPath = Properties.Settings.Default.TestRunDataDirectory;
			Assert.AreEqual(2, viewModel.Files.Length);
			Assert.AreEqual(Properties.Settings.Default.mzXMLTestFile1, viewModel.Files[0]);
			Assert.AreEqual(Properties.Settings.Default.mzXMLTestFile2, viewModel.Files[1]);
		}

		[TestMethod]
		public void SelectedItem()
		{
			Assert.IsNull(viewModel.SelectedItem);
			Assert.IsFalse(viewModel.RemoveRunData.CanExecute(viewModel.SelectedItem));
			viewModel.SelectedItem = GetFirstRun();
			Assert.IsNotNull(viewModel.SelectedItem);
			Assert.IsTrue(viewModel.RemoveRunData.CanExecute(viewModel.SelectedItem));
		}

		[TestMethod]
		public void CanAddRunData()
		{
			List<string> selectedRun = new List<string>() { @"c:\temp\test.mzxml" };
			viewModel.SelectedData = selectedRun[0];
			Assert.IsFalse(viewModel.AddRunData.CanExecute(null));
			Assert.IsFalse(viewModel.AddRunData.CanExecute(selectedRun));
			viewModel.SelectedItem = GetFirstRun();
			Assert.IsFalse(viewModel.AddRunData.CanExecute(null));
			Assert.IsFalse(viewModel.AddRunData.CanExecute(selectedRun));
			viewModel.SelectedItem = GetFirstLabel();
			Assert.IsTrue(viewModel.AddRunData.CanExecute(null));
			Assert.IsTrue(viewModel.AddRunData.CanExecute(selectedRun));
		}

		[TestMethod]
		public void AddRunData()
		{
			Mock<IServiceLocator> mockServiceLocator = new Mock<IServiceLocator>();
			IEventAggregator eventAggregator = new EventAggregator();
			mockServiceLocator.Setup(mock => mock.GetAllInstances<IDataProvider>()).Returns(new List<IDataProvider>() { new ProteoWizardDataProvider(eventAggregator) });
			List<string> selectedRuns = new List<string> { Properties.Settings.Default.mzXMLTestFile1, Properties.Settings.Default.mzXMLTestFile2 };
			Experiment experiment = experimentViewModel.Data as Experiment;
			experiment.DataProvider = new ProteoWizardDataProvider(eventAggregator);
			LabelingViewModel labelingViewModel = new LabelingViewModel(experimentViewModel.Samples.Children[0].Children[0].Data as Labeling, experimentViewModel.Samples.Children[0].Data as ProteinState, experiment, mockServiceLocator.Object);
			labelingViewModel.Parent = experimentViewModel.Samples.Children[0];
			viewModel.SelectedItem = labelingViewModel;
			viewModel.SelectedDataPath = string.Empty;
			Assert.AreEqual(2, labelingViewModel.Children.Count);

			viewModel.AddRunData.Execute(selectedRuns);

			Assert.AreEqual(4, labelingViewModel.Children.Count);
			Assert.AreEqual(Properties.Settings.Default.mzXMLTestFile1, labelingViewModel.Children[2].Location);
			Assert.AreEqual(Properties.Settings.Default.mzXMLTestFile2, labelingViewModel.Children[3].Location);
		}

		[TestMethod]
		public void AddRunDataWhenFileDoesNotExist()
		{
			List<string> selectedRuns = new List<string> { @"c:\temp\wrong.mzxml" };
			LabelingViewModel firstLabelViewModel = GetFirstLabel();
			viewModel.SelectedItem = firstLabelViewModel;
			Assert.AreEqual(2, firstLabelViewModel.Children.Count);

			viewModel.AddRunData.Execute(selectedRuns);

			Assert.AreEqual(2, firstLabelViewModel.Children.Count);
		}

		[TestMethod]
		public void CanRemoveRunData()
		{
			Assert.IsFalse(viewModel.RemoveRunData.CanExecute(null));
			Assert.IsFalse(viewModel.RemoveRunData.CanExecute(GetFirstLabel()));
			Assert.IsTrue(viewModel.RemoveRunData.CanExecute(GetFirstRun()));
		}

		[TestMethod]
		public void RemoveRunData()
		{
			LabelingViewModel firstLabelingViewModel = GetFirstLabel();
			Assert.AreEqual(2, firstLabelingViewModel.Children.Count);
			viewModel.RemoveRunData.Execute(GetFirstRun());
			Assert.AreEqual(1, firstLabelingViewModel.Children.Count);
		}

		private RunViewModel GetFirstRun()
		{
			return viewModel.ViewModel.Children.First().Children.First().Children.First();
		}

		private LabelingViewModel GetFirstLabel()
		{
			return viewModel.ViewModel.Children.First().Children.First();
		}
	}
}
