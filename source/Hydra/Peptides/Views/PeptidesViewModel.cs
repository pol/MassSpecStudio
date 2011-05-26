using System.Collections.Generic;
using System.Linq;
using Hydra.Core.Domain;
using Hydra.Core.Events;
using MassSpecStudio.Core;
using MassSpecStudio.Core.Extensions;
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
			_peptides = (from data in run.RunResults
						 select data.Peptide).DistinctBy(item => item.Sequence).ToList();
		}

		public IList<Peptide> Peptides
		{
			get { return _peptides; }
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
	}
}
