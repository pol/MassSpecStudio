using System.Linq;
using Hydra.Core.Domain;
using Hydra.Modules.Project.Views;
using MassSpecStudio.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hydra.Test.Modules.Project.Views
{
	[TestClass]
	public class ProjectViewModelTest
	{
		private ProjectViewModel viewModel;

		[TestInitialize]
		public void TestInitialize()
		{
			ProjectBase project = TestHelper.CreateTestProject();
			Experiment experiment = project.Experiments.First() as Experiment;
			experiment.Results.Add(new Result("result", experiment));

			viewModel = new ProjectViewModel(project);
		}

		[TestMethod]
		public void Construction()
		{
			Assert.AreEqual(1, viewModel.Children.Count);
			Assert.AreEqual(3, viewModel.Children[0].Children.Count);
			SamplesViewModel samplesViewModel = viewModel.Children[0].Children[0] as SamplesViewModel;
			Assert.AreEqual(2, samplesViewModel.Children.Count);
			Assert.AreEqual(2, samplesViewModel.Children[0].Children.Count);
			Assert.AreEqual(2, samplesViewModel.Children[0].Children[0].Children.Count);

			PeptidesViewModel peptidesViewModel = viewModel.Children[0].Children[1] as PeptidesViewModel;
			Assert.AreEqual(2, peptidesViewModel.Children.Count);

			ResultsViewModel resultsViewModel = viewModel.Children[0].Children[2] as ResultsViewModel;
			Assert.AreEqual(1, resultsViewModel.Children.Count);
		}

		[TestMethod]
		public void Properties()
		{
			Assert.AreEqual("temp", viewModel.Name);
			Assert.AreEqual(@"c:\temp\temp.mssproj", viewModel.Location);
		}
	}
}
