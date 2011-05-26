using System.Collections.Generic;
using Hydra.Core.Domain;
using Hydra.Modules.Results.Views;
using MassSpecStudio.Core;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ProteoWizard.MassSpecStudio.DataProvider;

namespace Hydra.Test.Modules.Results.Views
{
	[TestClass]
	public class ResultsViewModelTest
	{
		private ResultsViewModel viewModel;
		private IEventAggregator eventAggregator;
		private Mock<IServiceLocator> mockServiceLocator;
		private Result result;

		[TestInitialize]
		public void TestInitialize()
		{
			eventAggregator = new EventAggregator();
			mockServiceLocator = new Mock<IServiceLocator>();
			mockServiceLocator.Setup(mock => mock.GetAllInstances<IDataProvider>()).Returns(new List<IDataProvider>() { new ProteoWizardDataProvider(eventAggregator) });

			Experiment experiment = Experiment.Open(Properties.Settings.Default.TestExperiment1, new MassSpecStudio.Core.Domain.ProjectBase("testProject1", Properties.Settings.Default.ProjectRootDirectory), mockServiceLocator.Object);
			result = experiment.Results[0];
			viewModel = new ResultsViewModel(result, eventAggregator);
		}

		[TestMethod]
		public void Constructor()
		{
			Assert.AreEqual("Result (2/10/2011 05:46:05 PM)", viewModel.Title);
			Assert.AreEqual(10, viewModel.DeuterationResults.Count);
		}

		////[TestMethod]
		////public void OnPeptideSelected()
		////{
		////    eventAggregator.GetEvent<PeptideSelectedEvent>().Publish(result.RunResults[0].Peptide);
		////    Assert.AreEqual(result.RunResults[0].Peptide.Sequence, viewModel.SelectedDeuterationResult);
		////    Assert.AreEqual(4, viewModel.DeuterationResults.Count);
		////}

		[TestMethod]
		public void SaveExcel()
		{
			// TODO: Re-enable the commented code above
		}
	}
}
