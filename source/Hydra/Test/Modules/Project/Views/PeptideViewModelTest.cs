using Hydra.Core;
using Hydra.Core.Domain;
using Hydra.Modules.Project.Views;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hydra.Test.Modules.Project.Views
{
	[TestClass]
	public class PeptideViewModelTest
	{
		[TestMethod]
		public void Properties()
		{
			Peptide peptide = TestHelper.CreatePeptide("SAMPLE", 1);
			
			PeptideViewModel viewModel = new PeptideViewModel(peptide);
			Assert.AreEqual("SAMPLE", viewModel.Sequence);
			Assert.AreEqual(1, viewModel.Start);
			Assert.AreEqual(2, viewModel.Stop);
			Assert.AreEqual(3, viewModel.MZ);
			Assert.AreEqual(4, viewModel.ChargeState);
			Assert.AreEqual("5", viewModel.Notes);
			Assert.AreEqual(6, viewModel.Period);
			Assert.AreEqual(XicPeakPickerOption.MostIntenseWithinEntireXic, viewModel.PeakPicking);
			Assert.AreEqual(7, viewModel.RT);
			Assert.AreEqual(8, viewModel.RTVariance);
			Assert.AreEqual(9, viewModel.Adjust);
			Assert.AreEqual(10, viewModel.SelectionWidth);
			Assert.AreEqual(11, viewModel.NumberOfPeaks);
			Assert.AreEqual(12, viewModel.Threshold);
			Assert.AreEqual(13, viewModel.RTPadding);
		}
	}
}
