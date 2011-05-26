using System.Linq;
using Hydra.Core.Domain;
using Hydra.Modules.Project.Views;
using MassSpecStudio.Core.Domain;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Hydra.Test.Modules.Project.Views
{
	[TestClass]
	public class LabelingViewModelTest
	{
		private Experiment experiment;
		private Mock<IServiceLocator> mockServiceLocator;
		private LabelingViewModel viewModel;

		[TestInitialize]
		public void TestInitialize()
		{
			mockServiceLocator = new Mock<IServiceLocator>();
			ProjectBase project = TestHelper.CreateTestProject();
			experiment = project.Experiments.First() as Experiment;
			viewModel = new LabelingViewModel(experiment.Labeling.First(), experiment.ProteinStates.First(), experiment, mockServiceLocator.Object);
		}

		[TestMethod]
		public void Construction()
		{
			Assert.AreEqual(2, viewModel.Children.Count);
			Assert.AreEqual("file07 - run1 - apo-CaM 50%0025D2O trial1.wiff.mzXML", viewModel.Children[0].Name);
			Assert.AreEqual(viewModel, viewModel.Children[0].Parent);
			Assert.AreEqual("file25 - 20l apo trial1-s1 (MSMS).mzXML", viewModel.Children[1].Name);
			Assert.AreEqual(viewModel, viewModel.Children[1].Parent);
		}

		[TestMethod]
		public void GetSetProperties()
		{
			Assert.AreEqual("1(20)", viewModel.Name);
			Assert.AreEqual(20, viewModel.LabelingPercent);
			Assert.AreEqual(1, viewModel.LabelingTime);

			viewModel.LabelingPercent = 2;
			viewModel.LabelingTime = 3;

			Assert.AreEqual(2, viewModel.LabelingPercent);
			Assert.AreEqual(3, viewModel.LabelingTime);
		}

		[TestMethod]
		public void RemoveRunData()
		{
			Assert.AreEqual(2, viewModel.Children.Count);
			Assert.AreEqual(4, experiment.Runs.Count);
			viewModel.RemoveRunData.Execute(viewModel.Children[0]);
			Assert.AreEqual(1, viewModel.Children.Count);
			Assert.AreEqual(3, experiment.Runs.Count);
		}
	}
}
