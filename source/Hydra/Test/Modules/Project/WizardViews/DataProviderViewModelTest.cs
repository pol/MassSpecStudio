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
	public class DataProviderViewModelTest
	{
		private DataProviderViewModel viewModel;
		private ExperimentViewModel experimentViewModel;
		private Experiment experiment;
		private Mock<IEventAggregator> mockEventAggregator;
		private Mock<IServiceLocator> mockServiceLocator;

		[TestInitialize]
		public void TestInitialize()
		{
			mockEventAggregator = new Mock<IEventAggregator>();
			mockServiceLocator = new Mock<IServiceLocator>();
			mockServiceLocator.Setup(mock => mock.GetAllInstances<IDataProvider>()).Returns(new List<IDataProvider>() { new ProteoWizardDataProvider(mockEventAggregator.Object) });

			ProjectBase project = TestHelper.CreateTestProject();
			experiment = project.Experiments.First() as Experiment;
			experimentViewModel = new ExperimentViewModel(experiment);
			viewModel = new DataProviderViewModel(experimentViewModel, mockServiceLocator.Object);
		}

		[TestMethod]
		public void GetProperties()
		{
			Assert.AreEqual("Select Data Provider", viewModel.Title);
			Assert.AreEqual(experimentViewModel, viewModel.ViewModel);
			Assert.AreEqual(typeof(DataProviderView), viewModel.ViewType);
		}

		[TestMethod]
		public void DataProviders()
		{
			Assert.AreEqual(1, viewModel.DataProviders.Count);
			Assert.IsInstanceOfType(viewModel.DataProviders[0], typeof(ProteoWizardDataProvider));
		}

		[TestMethod]
		public void SelectDataProvider()
		{
			Assert.AreEqual(viewModel.DataProviders[0], viewModel.SelectedDataProvider);
		}

		[TestMethod]
		public void OnNext()
		{
			Assert.AreEqual(null, experiment.DataProvider);
			Assert.AreEqual(true, viewModel.OnNext());
			Assert.AreEqual(viewModel.DataProviders[0], experiment.DataProvider);
			Assert.AreEqual(viewModel.DataProviders[0].TypeId, experiment.DataProviderType);
		}
	}
}
