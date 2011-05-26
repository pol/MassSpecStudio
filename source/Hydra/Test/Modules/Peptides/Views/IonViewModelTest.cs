using System.Collections.Generic;
using Hydra.Modules.Peptides.Views;
using Microsoft.Practices.Prism.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProteinCalc;

namespace Hydra.Test.Modules.Peptides.Views
{
	[TestClass]
	public class IonViewModelTest
	{
		private IonViewModel viewModel;
		private IList<B_Ion> ionsB;
		private IList<Y_Ion> ionsY;
		private Hydra.Core.Domain.Peptide peptide;
		private System.Collections.ObjectModel.ObservableCollection<Hydra.Core.Domain.FragmentIon> fragmentIons;

		[TestInitialize]
		public void TestInitialize()
		{
			IEventAggregator eventAggregator = new EventAggregator();
			peptide = new Hydra.Core.Domain.Peptide() { Sequence = "SAMPLE", MonoIsotopicMass = 200 };
			PeptideViewModel peptideViewModel = new PeptideViewModel(peptide, eventAggregator);
			ionsB = new List<B_Ion>();
			ionsB.Add(peptideViewModel.BIonSinglyCharged[0].Ion as B_Ion);
			ionsB.Add(peptideViewModel.BIonSinglyCharged[1].Ion as B_Ion);
			ionsY = new List<Y_Ion>();
			ionsY.Add(peptideViewModel.YIonSinglyCharged[0].Ion as Y_Ion);
			ionsY.Add(peptideViewModel.YIonSinglyCharged[1].Ion as Y_Ion);
			ionsY.Add(peptideViewModel.YIonSinglyCharged[2].Ion as Y_Ion);

			IList<Hydra.Core.Domain.FragmentIon> fragmentLists = new List<Hydra.Core.Domain.FragmentIon>();
			fragmentLists.Add(new Hydra.Core.Domain.FragmentIon() { MZ = 100, FragmentIonType = Hydra.Core.FragmentIonType.BFragment });
			fragmentLists.Add(new Hydra.Core.Domain.FragmentIon() { MZ = 88.039853443828, FragmentIonType = Hydra.Core.FragmentIonType.BFragment });
			fragmentLists.Add(new Hydra.Core.Domain.FragmentIon() { MZ = 148.06098281631802, FragmentIonType = Hydra.Core.FragmentIonType.BFragment });
			fragmentIons = new System.Collections.ObjectModel.ObservableCollection<Hydra.Core.Domain.FragmentIon>(fragmentLists);

			viewModel = new IonViewModel();
		}

		[TestMethod]
		public void CreateListBIons()
		{
			// Can't create B ions with a monoisotopic.
			IList<IonViewModel> ionViewModels = IonViewModel.CreateList(ionsB, fragmentIons, peptide);

			Assert.AreEqual(2, ionViewModels.Count);
			Assert.AreEqual(true, ionViewModels[0].IsChecked);
			Assert.AreEqual(false, ionViewModels[1].IsChecked);
		}

		[TestMethod]
		public void CreateListYIons()
		{
			// Can't create B ions with a monoisotopic.
			IList<IonViewModel> ionViewModels = IonViewModel.CreateList(ionsY, fragmentIons, peptide);

			Assert.AreEqual(3, ionViewModels.Count);
			Assert.AreEqual(true, ionViewModels[0].IsChecked);
			Assert.AreEqual(false, ionViewModels[1].IsChecked);
			Assert.AreEqual(false, ionViewModels[2].IsChecked);
		}

		[TestMethod]
		public void IsChecked()
		{
			IList<IonViewModel> ionViewModels = IonViewModel.CreateList(ionsY, fragmentIons, peptide);

			Assert.AreEqual(false, ionViewModels[1].IsChecked);
			ionViewModels[1].IsChecked = true;
			Assert.AreEqual(true, ionViewModels[1].IsChecked);
		}

		[TestMethod]
		public void IsUnchecked()
		{
			IList<IonViewModel> ionViewModels = IonViewModel.CreateList(ionsY, fragmentIons, peptide);

			Assert.AreEqual(true, ionViewModels[0].IsChecked);
			ionViewModels[0].IsChecked = false;
			Assert.AreEqual(false, ionViewModels[0].IsChecked);
		}
	}
}
