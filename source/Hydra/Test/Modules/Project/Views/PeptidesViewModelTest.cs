using System.Linq;
using Hydra.Core.Domain;
using Hydra.Core.Provider;
using Hydra.DataProvider;
using Hydra.Modules.Project.Views;
using MassSpecStudio.Core.Domain;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Hydra.Test.Modules.Project.Views
{
	[TestClass]
	public class PeptidesViewModelTest
	{
		private Mock<IServiceLocator> mockServiceLocator;
		private ProjectBase project;
		private PeptidesViewModel viewModel;
		private Experiment experiment;

		[TestInitialize]
		public void TestInitialize()
		{
			mockServiceLocator = new Mock<IServiceLocator>();
			project = TestHelper.CreateTestProject();
			experiment = project.Experiments.First() as Experiment;
			viewModel = new PeptidesViewModel(project.Experiments.First() as Experiment, mockServiceLocator.Object);
			Assert.AreEqual(2, viewModel.Children.Count);
		}

		[TestMethod]
		public void Construction()
		{
			Assert.AreEqual(2, viewModel.Children.Count);
			Assert.AreEqual(viewModel, viewModel.Children[0].Parent);
			Assert.AreEqual("SAMPLE", viewModel.Children[0].Name);
			Assert.AreEqual(viewModel, viewModel.Children[1].Parent);
			Assert.AreEqual("SAMPLES", viewModel.Children[1].Name);
		}

		[TestMethod]
		public void GetProperties()
		{
			Assert.AreEqual(@"c:\temp\testExperiment\Data\Peptides.xml", viewModel.Location);
			Assert.AreEqual("peptides", viewModel.Name);
		}

		[TestMethod]
		public void AddFromFile()
		{
			mockServiceLocator.Setup(mock => mock.GetInstance<IPeptideDataProvider>()).Returns(new CsvPeptideDataProvider());
			viewModel.AddPeptides(Properties.Settings.Default.PeptideTestFile1);

			Assert.AreEqual(9, viewModel.Children.Count);
			Assert.AreEqual(9, experiment.Peptides.PeptideCollection.Count);
			Assert.AreEqual(viewModel, viewModel.Children[3].Parent);
			Assert.AreEqual("QGFLVF", viewModel.Children[3].Name);
			Assert.AreEqual(true, viewModel.IsExpanded);
		}

		[TestMethod]
		public void AddPeptide()
		{
			viewModel.AddPeptide.Execute(viewModel);

			Assert.AreEqual(3, viewModel.Children.Count);
			Assert.AreEqual(3, experiment.Peptides.PeptideCollection.Count);
			Assert.AreEqual("A", viewModel.Children[2].Name);
		}

		[TestMethod]
		public void Remove()
		{
			viewModel.Remove.Execute(viewModel.Children[1]);

			Assert.AreEqual(1, viewModel.Children.Count);
			Assert.AreEqual(1, experiment.Peptides.PeptideCollection.Count);
		}
	}
}
