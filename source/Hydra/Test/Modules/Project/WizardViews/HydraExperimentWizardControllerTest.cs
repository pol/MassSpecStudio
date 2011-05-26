using System;
using System.Collections.Generic;
using System.Linq;
using Hydra.Core.Domain;
using Hydra.Modules.Project.WizardViews;
using MassSpecStudio.Core;
using MassSpecStudio.Core.Domain;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ProteoWizard.MassSpecStudio.DataProvider;

namespace Hydra.Test.Modules.Project.WizardViews
{
	[TestClass]
	public class HydraExperimentWizardControllerTest
	{
		private Mock<IRegionManager> mockRegionManager;
		private Mock<IServiceLocator> mockServiceLocator;
		private Mock<IEventAggregator> mockEventAggregator;
		private Mock<IRegion> mockRegion;
		private HydraExperimentWizardController controller;
		private bool finishedEventFired;

		[TestInitialize]
		public void TestInitialize()
		{
			mockEventAggregator = new Mock<IEventAggregator>();
			mockServiceLocator = new Mock<IServiceLocator>();
			mockRegionManager = new Mock<IRegionManager>();
			mockRegion = new Mock<IRegion>();
			mockServiceLocator.Setup(mock => mock.GetAllInstances<IDataProvider>()).Returns(new List<IDataProvider>() { new ProteoWizardDataProvider(mockEventAggregator.Object) });

			ProjectBase project = TestHelper.CreateTestProject();
			Experiment experiment = project.Experiments.First() as Experiment;
			controller = new HydraExperimentWizardController(mockRegionManager.Object, mockServiceLocator.Object, experiment);
			controller.FinishedEvent += HandleFinishedEvent;
			TestHelper.MockRegionManagerDisplay("WizardContentRegion", typeof(ProteinStatesViewModel), mockServiceLocator, mockRegion, mockRegionManager);
			TestHelper.MockRegionManagerDisplay("WizardContentRegion", typeof(LabellingsViewModel), mockServiceLocator, mockRegion, mockRegionManager);
			TestHelper.MockRegionManagerDisplay("WizardContentRegion", typeof(DataProviderViewModel), mockServiceLocator, mockRegion, mockRegionManager);
			TestHelper.MockRegionManagerDisplay("WizardContentRegion", typeof(RunsViewModel), mockServiceLocator, mockRegion, mockRegionManager);
			TestHelper.MockRegionManagerDisplay("WizardContentRegion", typeof(ImportPeptidesViewModel), mockServiceLocator, mockRegion, mockRegionManager);
		}

		[TestMethod]
		public void Next()
		{
			Assert.IsInstanceOfType(controller.CurrentWizardViewModel, typeof(ProteinStatesViewModel));
			controller.NextCommand.Execute(string.Empty);
			Assert.IsInstanceOfType(controller.CurrentWizardViewModel, typeof(LabellingsViewModel));
		}

		[TestMethod]
		public void NextOnLastPage()
		{
			controller.NextCommand.Execute(string.Empty);
			controller.NextCommand.Execute(string.Empty);
			controller.NextCommand.Execute(string.Empty);
			controller.NextCommand.Execute(string.Empty);
			Assert.IsFalse(finishedEventFired);
			Assert.IsInstanceOfType(controller.CurrentWizardViewModel, typeof(ImportPeptidesViewModel));
			controller.NextCommand.Execute(string.Empty);
			Assert.IsTrue(finishedEventFired);
			Assert.IsInstanceOfType(controller.CurrentWizardViewModel, typeof(ImportPeptidesViewModel));
		}

		[TestMethod]
		public void Back()
		{
			Next();
			Assert.IsInstanceOfType(controller.CurrentWizardViewModel, typeof(LabellingsViewModel));
			controller.BackCommand.Execute(string.Empty);
			Assert.IsInstanceOfType(controller.CurrentWizardViewModel, typeof(ProteinStatesViewModel));
		}

		[TestMethod]
		public void BackOnFirstPage()
		{
			Assert.IsInstanceOfType(controller.CurrentWizardViewModel, typeof(ProteinStatesViewModel));
			controller.BackCommand.Execute(string.Empty);
			Assert.IsInstanceOfType(controller.CurrentWizardViewModel, typeof(ProteinStatesViewModel));
		}

		[TestMethod]
		public void Workflow()
		{
			Assert.IsFalse(finishedEventFired);
			Assert.IsInstanceOfType(controller.CurrentWizardViewModel, typeof(ProteinStatesViewModel));

			ExecuteNext(typeof(LabellingsViewModel), false);
			ExecuteNext(typeof(DataProviderViewModel), false);
			ExecuteNext(typeof(RunsViewModel), false);
			ExecuteNext(typeof(ImportPeptidesViewModel), false);
			ExecuteNext(typeof(ImportPeptidesViewModel), true);
		}

		private void ExecuteNext(Type viewModel, bool isFinishedEventFired)
		{
			controller.NextCommand.Execute(string.Empty);
			Assert.AreEqual(isFinishedEventFired, finishedEventFired);
			Assert.IsInstanceOfType(controller.CurrentWizardViewModel, viewModel);
		}

		private void HandleFinishedEvent(object sender, EventArgs args)
		{
			finishedEventFired = true;
		}
	}
}
