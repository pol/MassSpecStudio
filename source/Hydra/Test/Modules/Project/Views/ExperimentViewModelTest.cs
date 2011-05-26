using System.Linq;
using Hydra.Core.Domain;
using Hydra.Modules.Project.Views;
using MassSpecStudio.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hydra.Test.Modules.Project.Views
{
	[TestClass]
	public class ExperimentViewModelTest
	{
		private Experiment experiment;
		private ExperimentViewModel viewModel;

		[TestInitialize]
		public void TestInitialize()
		{
			ProjectBase project = TestHelper.CreateTestProject();
			experiment = project.Experiments.First() as Experiment;
			viewModel = new ExperimentViewModel(experiment);
		}

		[TestMethod]
		public void Properties()
		{
			Assert.AreEqual(@"c:\temp\testExperiment\testExperiment.mssexp", viewModel.Location);
			Assert.AreEqual(0, viewModel.Children.Count);
			Assert.AreEqual(2, viewModel.Samples.Children.Count);
			Assert.AreEqual(2, viewModel.Peptides.Children.Count);
			Assert.AreEqual(0, viewModel.Results.Children.Count);
			Assert.AreEqual("Experiment", viewModel.Name);
		}
	}
}
