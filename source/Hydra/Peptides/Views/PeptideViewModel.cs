using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using MassSpecStudio.Core;
using MassSpecStudio.Core.Events;
using Microsoft.Practices.Prism.Events;
using ProteinCalc;

namespace Hydra.Modules.Peptides.Views
{
	[Export(typeof(PeptideViewModel))]
	public class PeptideViewModel : ViewModelBase
	{
		private PeptideIon _peptideIon;
		private Hydra.Core.Domain.Peptide _peptide;
		private PeptideIonFragmentor _peptideIonFragmentor;
		private Core.Domain.FragmentIon _selectedFragmentIon;
		private IEventAggregator _eventAggregator;

		public PeptideViewModel(Hydra.Core.Domain.Peptide peptide, IEventAggregator eventAggregator)
		{
			_peptide = peptide;
			_peptideIon = new PeptideIon(peptide.Sequence, 2);
			_peptideIonFragmentor = new PeptideIonFragmentor();
			_eventAggregator = eventAggregator;
		}

		public string Sequence
		{
			get
			{
				return _peptide.Sequence;
			}

			set
			{
				_peptide.Sequence = value;
				_peptideIon = new PeptideIon(value, 2);

				NotifyPropertyChanged(() => Sequence);
				NotifyPropertyChanged(() => BIonSinglyCharged);
				NotifyPropertyChanged(() => YIonSinglyCharged);
				NotifyPropertyChanged(() => BIonDoublyCharged);
				NotifyPropertyChanged(() => YIonDoublyCharged);
			}
		}

		public IList<IonViewModel> BIonSinglyCharged
		{
			get
			{
				return IonViewModel.CreateList(_peptideIonFragmentor.GetBIonList(_peptideIon, 1), _peptide.FragmentIonList, _peptide);
			}
		}

		public IList<IonViewModel> BIonDoublyCharged
		{
			get
			{
				return IonViewModel.CreateList(_peptideIonFragmentor.GetBIonList(_peptideIon, 2), _peptide.FragmentIonList, _peptide);
			}
		}

		public IList<IonViewModel> YIonSinglyCharged
		{
			get
			{
				return IonViewModel.CreateList(_peptideIonFragmentor.GetYIonList(_peptideIon, 1), _peptide.FragmentIonList, _peptide);
			}
		}

		public IList<IonViewModel> YIonDoublyCharged
		{
			get
			{
				return IonViewModel.CreateList(_peptideIonFragmentor.GetYIonList(_peptideIon, 2), _peptide.FragmentIonList, _peptide);
			}
		}

		public Core.Domain.FragmentIon SelectedFragmentIon
		{
			get
			{
				return _selectedFragmentIon;
			}

			set
			{
				if (value != null && value != _selectedFragmentIon)
				{
					_selectedFragmentIon = value;
					_eventAggregator.GetEvent<ObjectSelectionEvent>().Publish(value);
					NotifyPropertyChanged(() => SelectedFragmentIon);
				}
			}
		}

		public ObservableCollection<Core.Domain.FragmentIon> FragmentIons
		{
			get { return _peptide.FragmentIonList; }
		}
	}
}
