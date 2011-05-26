using System.Linq;
using Hydra.Core.Domain;
using Hydra.Modules.Project.Views;
using MassSpecStudio.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hydra.Test.Modules.Project.Views
{
	[TestClass]
	public class ProteinStateViewModelTest
	{
		[TestMethod]
		public void NameAndConstruction()
		{
			ProjectBase project = TestHelper.CreateTestProject();
			Experiment experiment = project.Experiments.First() as Experiment;

			ProteinStateViewModel viewModel = new ProteinStateViewModel(experiment.ProteinStates.Last(), experiment);

			Assert.AreEqual(2, viewModel.Children.Count);
			Assert.AreEqual("1(20)", viewModel.Children[0].Name);
			Assert.AreEqual(viewModel, viewModel.Children[0].Parent);
			Assert.AreEqual("2(30)", viewModel.Children[1].Name);
			Assert.AreEqual(viewModel, viewModel.Children[1].Parent);

			Assert.AreEqual("Protein state 2", viewModel.Name);
		}
	}
}
