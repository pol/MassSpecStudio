using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hydra.Test.Modules.Peptides.Views
{
	[TestClass]
	public class PeptidesViewModelTest
	{
		////private PeptidesViewModel viewModel;
		////private IEventAggregator eventAggregator;
		////private Mock<IServiceLocator> mockServiceLocator;
		////private Result result;
		////private bool selectedPeptideFired;

		////[TestInitialize]
		////public void TestInitialize()
		////{
		////    eventAggregator = new EventAggregator();
		////    mockServiceLocator = new Mock<IServiceLocator>();
		////    mockServiceLocator.Setup(mock => mock.GetAllInstances<IDataProvider>()).Returns(new List<IDataProvider>() { new ProteoWizardDataProvider(eventAggregator) });

		////    Experiment experiment = Experiment.Open(Properties.Settings.Default.TestExperiment1, new MassSpecStudio.Core.Domain.ProjectBase("testProject1", Properties.Settings.Default.ProjectRootDirectory), mockServiceLocator.Object);
		////    result = experiment.Results[0];
		////    viewModel = new PeptidesViewModel(result, eventAggregator);
		////}

		////[TestMethod]
		////public void Constructor()
		////{
		////    Assert.AreEqual(5, viewModel.Peptides.Count);
		////}

		////[TestMethod]
		////public void SelectedPeptide()
		////{
		////    eventAggregator.GetEvent<PeptideSelectedEvent>().Subscribe(OnSelectedPeptide);
		////    viewModel.SelectedPeptide = viewModel.Peptides[0];
		////    Assert.AreEqual(true, selectedPeptideFired);
		////}

		////private void OnSelectedPeptide(Peptide value)
		////{
		////    selectedPeptideFired = true;
		////}
		//// TODO: Reenable commented code above.
	}
}
