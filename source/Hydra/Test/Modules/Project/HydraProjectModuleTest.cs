using System.Collections.ObjectModel;
using System.Linq;
using Hydra.Core;
using Hydra.Core.Domain;
using Hydra.Modules.Project;
using Hydra.Modules.Project.Views;
using MassSpecStudio.Core;
using MassSpecStudio.Core.Domain;
using MassSpecStudio.Core.Events;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Hydra.Test.Modules.Project
{
	[TestClass]
	public class HydraProjectModuleTest
	{
		private HydraProjectModule hydraProjectModule;
		private IEventAggregator eventAggregator;
		private Mock<IRegionManager> mockRegionManager;
		private Mock<IDocumentManager> mockDocumentManager;
		private Mock<IServiceLocator> mockServiceLocator;
		private Mock<IRegion> mockRegion;
		private bool loadProjectDataEventFired;

		[TestInitialize]
		public void TestInitialize()
		{
			eventAggregator = new EventAggregator();
			mockRegion = new Mock<IRegion>();
			mockRegionManager = new Mock<IRegionManager>();
			mockDocumentManager = new Mock<IDocumentManager>();
			mockServiceLocator = new Mock<IServiceLocator>();
			hydraProjectModule = new HydraProjectModule(mockRegionManager.Object, eventAggregator, mockDocumentManager.Object, mockServiceLocator.Object);
		}

		[TestMethod]
		public void OnLoadProject()
		{
			TestHelper.MockRegionManagerDisplay(Regions.LeftRegion.ToString(), typeof(ProjectExplorerView), mockServiceLocator, mockRegion, mockRegionManager);

			ProjectBase project = new ProjectBase("temp", @"c:\");
			project.Experiments.Add(new Experiment("testExperiment", project, new HydraExperimentType(mockServiceLocator.Object)));

			eventAggregator.GetEvent<LoadProjectDataEvent>().Subscribe(OnLoadProjectDataEvent);
			eventAggregator.GetEvent<LoadProjectEvent>().Publish(project);

			Assert.AreEqual(project, ((ProjectViewModel)DocumentCache.ProjectViewModel).Data as ProjectBase);
			Assert.AreEqual(project.Experiments.First(), DocumentCache.Experiment);
			Assert.IsTrue(loadProjectDataEventFired);
		}

		[TestMethod]
		public void OnProjectClosed()
		{
			MockRegionManagerRemove(Regions.LeftRegion.ToString());

			eventAggregator.GetEvent<ProjectClosedEvent>().Publish(null);

			mockRegion.VerifyAll();
		}

		private void OnLoadProjectDataEvent(object vaue)
		{
			loadProjectDataEventFired = true;
		}

		private void MockRegionManagerRemove(string regionName)
		{
			mockRegion.Setup(mock => mock.Views).Returns(new ViewsCollection(new ObservableCollection<ItemMetadata>(), null));
			mockRegionManager.Setup(mock => mock.Regions.ContainsRegionWithName(regionName)).Returns(true);
			mockRegionManager.Setup(mock => mock.Regions[regionName]).Returns(mockRegion.Object);
		}
	}
}
