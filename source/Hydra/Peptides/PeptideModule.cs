using System.ComponentModel.Composition;
using AvalonDock;
using Hydra.Core.Domain;
using Hydra.Core.Events;
using Hydra.Modules.Peptides.Views;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;

namespace Hydra.Modules.Peptides
{
	[ModuleExport(typeof(PeptideModule))]
	[ExportMetadata("Title", "Peptide")]
	[ExportMetadata("Type", "Module")]
	[ExportMetadata("Author", "Hydra")]
	[ExportMetadata("Description", "This modules allows viewing and selection of peptides.")]
	public class PeptideModule : IModule
	{
		private readonly IRegionManager _regionManager;
		private readonly IEventAggregator _eventAggregator;

		[ImportingConstructor]
		public PeptideModule(IRegionManager regionManager, IEventAggregator eventAggregator)
		{
			_regionManager = regionManager;
			_eventAggregator = eventAggregator;

			eventAggregator.GetEvent<FileClickedEvent>().Subscribe(OnFileClicked, ThreadOption.PublisherThread, true, FilterClicks);
			eventAggregator.GetEvent<ShowPeptidesEvent>().Subscribe(OnShowPeptides);
			eventAggregator.GetEvent<LoadManualValidationEvent>().Subscribe(OnShowPeptidesForValidation);
			eventAggregator.GetEvent<DisplayPeptidesWindowEvent>().Subscribe(OnDisplayPeptidesWindow);
		}

		public void Initialize()
		{
		}

		public void OnFileClicked(object value)
		{
			ManagedContent peptideView = _regionManager.FindExistingView("DocumentRegion", typeof(PeptideView), (value as Peptide).Sequence);
			if (peptideView == null)
			{
				peptideView = new PeptideView(new PeptideViewModel(value as Peptide, _eventAggregator));
				_regionManager.AddToRegion("DocumentRegion", peptideView);
			}
			peptideView.Show();
			peptideView.Activate();
		}

		public void OnShowPeptides(object value)
		{
			ManagedContent peptidesView = _regionManager.FindExistingView("BottomRegion", typeof(PeptidesView));
			if (peptidesView == null)
			{
				peptidesView = new PeptidesView(new PeptidesViewModel(value as Core.Domain.Run));
				_regionManager.AddToRegion("BottomRegion", peptidesView);
			}
			peptidesView.Show();
			peptidesView.Activate();
		}

		public void OnShowPeptidesForValidation(Result value)
		{
			ManagedContent peptidesView = _regionManager.FindExistingView("BottomRegion", typeof(PeptidesView));
			if (peptidesView == null)
			{
				peptidesView = new PeptidesView(new PeptidesViewModel(value, _eventAggregator));
				_regionManager.AddToRegion("BottomRegion", peptidesView);
			}
			peptidesView.Show();
			peptidesView.Activate();
		}

		public void OnDisplayPeptidesWindow(object value)
		{
			ManagedContent peptidesView = _regionManager.FindExistingView("BottomRegion", typeof(PeptidesView));
			if (peptidesView != null)
			{
				peptidesView.Show();
				peptidesView.Activate();
			}
		}

		private bool FilterClicks(object value)
		{
			return value is Peptide;
		}
	}
}
