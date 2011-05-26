using Hydra.Modules.Project.Views;
using MassSpecStudio.Core.Domain;
using MassSpecStudio.Core.Events;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Hydra.Test.Modules.Project.Views
{
	[TestClass]
	public class ProjectExplorerViewModelTest
	{
		private ProjectBase project;
		private Mock<IRegionManager> mockRegionManager;
		private EventAggregator eventAggregator;
		private ProjectExplorerViewModel viewModel;
		private ProjectViewModel projectViewModel;
		private bool objectSelectionEventFired;

		[TestInitialize]
		public void TestInitialize()
		{
			project = TestHelper.CreateTestProject();
			mockRegionManager = new Mock<IRegionManager>();
			eventAggregator = new EventAggregator();

			viewModel = new ProjectExplorerViewModel(mockRegionManager.Object, eventAggregator);
			projectViewModel = new ProjectViewModel(project);
			viewModel.Project.Add(projectViewModel);
		}

		[TestMethod]
		public void Properties()
		{
			Assert.AreEqual(1, viewModel.Project.Count);
			Assert.IsNull(viewModel.SelectedItem);

			viewModel.SelectedItem = viewModel.Project[0];

			Assert.AreEqual(projectViewModel, viewModel.SelectedItem);
		}

		[TestMethod]
		public void OnSelectionChanged()
		{
			eventAggregator.GetEvent<ObjectSelectionEvent>().Subscribe(OnObjectSelectionEvent);

			viewModel.SelectedItem = viewModel.Project[0];
			viewModel.OnSelectionChanged();

			Assert.AreEqual(true, objectSelectionEventFired);
		}

		[TestMethod]
		public void OnLoadProject()
		{
			LoadProjectDataEvent loadProjectDataEvent = new LoadProjectDataEvent();
			ProjectViewModel newProjectViewModel = new ProjectViewModel(project);
			eventAggregator.GetEvent<LoadProjectDataEvent>().Publish(newProjectViewModel);

			Assert.AreEqual(newProjectViewModel, viewModel.Project[0]);
		}

		private void OnObjectSelectionEvent(object value)
		{
			objectSelectionEventFired = true;
		}
	}
}
