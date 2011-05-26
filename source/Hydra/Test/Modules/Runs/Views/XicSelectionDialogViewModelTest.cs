using Hydra.Modules.Run.Views;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hydra.Test.Modules.Runs.Views
{
	[TestClass]
	public class XicSelectionDialogViewModelTest
	{
		private XicSelectionDialogViewModel viewModel;

		[TestInitialize]
		public void TestInitialize()
		{
			viewModel = new XicSelectionDialogViewModel();
		}

		[TestMethod]
		public void Constructor()
		{
			Assert.AreEqual(300, viewModel.Mass);
			Assert.AreEqual(0.01, viewModel.MzTolerance);
		}

		[TestMethod]
		public void Properties()
		{
			viewModel.Mass = 1;
			viewModel.MzTolerance = 2;

			Assert.AreEqual(1, viewModel.Mass);
			Assert.AreEqual(2, viewModel.MzTolerance);
		}
	}
}
