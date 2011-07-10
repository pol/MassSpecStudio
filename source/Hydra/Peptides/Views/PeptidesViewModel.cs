using System.Collections.Generic;
using System.Linq;
using Hydra.Core.Domain;
using Hydra.Core.Events;
using MassSpecStudio.Core;
using Microsoft.Practices.Prism.Events;

namespace Hydra.Modules.Peptides.Views
{
	public class PeptidesViewModel : ViewModelBase
	{
		private readonly IEventAggregator _eventAggregator;
		private IList<Peptide> _peptides;
		private Peptide _selectedPeptide;

		public PeptidesViewModel(Core.Domain.Run run)
		{
		}

		public PeptidesViewModel(Core.Domain.Result run, IEventAggregator eventAggregator)
		{
			_eventAggregator = eventAggregator;
			Load(run);
		}

		public IList<Peptide> Peptides
		{
			get
			{
				return _peptides;
			}

			private set
			{
				_peptides = value;
				NotifyPropertyChanged(() => Peptides);
			}
		}

		public Peptide SelectedPeptide
		{
			get
			{
				return _selectedPeptide;
			}

			set
			{
				_selectedPeptide = value;
				NotifyPropertyChanged(() => SelectedPeptide);
				_eventAggregator.GetEvent<PeptideSelectedEvent>().Publish(_selectedPeptide);
			}
		}

		public void Load(Core.Domain.Result run)
		{
			IList<Peptide> allPeptides = (from data in run.RunResults
										  select data.Peptide).ToList();

			Peptides = new List<Peptide>();
			foreach (Peptide peptide in allPeptides)
			{
				if (!Peptides.Any(item => item.RT == peptide.RT && item.MonoIsotopicMass == peptide.MonoIsotopicMass && item.ChargeState == peptide.ChargeState))
				{
					Peptides.Add(peptide);
				}
			}
		}
	}
}
