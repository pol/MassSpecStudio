using Hydra.Core.Domain;
using Hydra.Modules.Peptides.Views;
using MassSpecStudio.Core.Events;
using Microsoft.Practices.Prism.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hydra.Test.Modules.Peptides.Views
{
	[TestClass]
	public class PeptideViewModelTest
	{
		private IEventAggregator eventAggregator;
		private PeptideViewModel viewModel;
		private bool selectionFired;

		[TestInitialize]
		public void TestInitialize()
		{
			eventAggregator = new EventAggregator();
			viewModel = new PeptideViewModel(new Peptide() { Sequence = "SAMPLE" }, eventAggregator);
		}

		[TestMethod]
		public void Constructor()
		{
			Assert.AreEqual("SAMPLE", viewModel.Sequence);
			Assert.AreEqual(5, viewModel.BIonDoublyCharged.Count);
			Assert.AreEqual(5, viewModel.BIonSinglyCharged.Count);
			Assert.AreEqual(5, viewModel.YIonDoublyCharged.Count);
			Assert.AreEqual(5, viewModel.YIonSinglyCharged.Count);
			Assert.AreEqual(0, viewModel.FragmentIons.Count);
		}

		[TestMethod]
		public void SelectedFragmentIon()
		{
			FragmentIon fragmentIon = new FragmentIon();
			eventAggregator.GetEvent<ObjectSelectionEvent>().Subscribe(OnFragmentIon);
			viewModel.SelectedFragmentIon = fragmentIon;

			Assert.AreEqual(fragmentIon, viewModel.SelectedFragmentIon);
			Assert.AreEqual(true, selectionFired);
		}

		private void OnFragmentIon(object value)
		{
			selectionFired = true;
		}
	}
}
