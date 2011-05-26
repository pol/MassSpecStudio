using Hydra.Modules.Run.Views;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hydra.Test.Modules.Runs.Views
{
	[TestClass]
	public class SpectrumSelectionDialogViewModelTest
	{
		private SpectrumSelectionDialogViewModel viewModel;

		[TestInitialize]
		public void TestInitialize()
		{
			viewModel = new SpectrumSelectionDialogViewModel();
		}

		[TestMethod]
		public void Constructor()
		{
			Assert.AreEqual(-5, viewModel.MZLowerOffset);
			Assert.AreEqual(5, viewModel.MZUpperOffset);
		}

		[TestMethod]
		public void Properties()
		{
			viewModel.MonoisotopicMass = 1;
			viewModel.MZLowerOffset = 2;
			viewModel.MZUpperOffset = 3;
			viewModel.StartMass = 4;
			viewModel.StopMass = 5;

			Assert.AreEqual(1, viewModel.MonoisotopicMass);
			Assert.AreEqual(2, viewModel.MZLowerOffset);
			Assert.AreEqual(3, viewModel.MZUpperOffset);
			Assert.AreEqual(4, viewModel.StartMass);
			Assert.AreEqual(5, viewModel.StopMass);
		}
	}
}
