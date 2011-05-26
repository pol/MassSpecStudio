using System.Linq;
using Hydra.Core.Domain;
using Hydra.Modules.Project.Views;
using MassSpecStudio.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hydra.Test.Modules.Project.Views
{
	[TestClass]
	public class ResultsViewModelTest
	{
		[TestMethod]
		public void Name()
		{
			ProjectBase project = TestHelper.CreateTestProject();
			Experiment experiment = project.Experiments.First() as Experiment;
			experiment.Results.Add(new Result("result", experiment));

			ResultsViewModel viewModel = new ResultsViewModel(experiment);
			Assert.AreEqual("Results", viewModel.Name);
			Assert.AreEqual(1, viewModel.Children.Count);
			Assert.AreEqual("result", viewModel.Children[0].Name);
		}
	}
}
