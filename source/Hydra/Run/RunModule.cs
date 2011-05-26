using System.ComponentModel.Composition;
using AvalonDock;
using Hydra.Core.Events;
using Hydra.Modules.Run.Views;
using MassSpecStudio.Core;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;

namespace Hydra.Modules.Run
{
	[ModuleExport(typeof(RunModule))]
	[ExportMetadata("Title", "Run")]
	[ExportMetadata("Type", "Module")]
	[ExportMetadata("Author", "Hydra")]
	[ExportMetadata("Description", "This module allows viewing of run information and the generation of tic, xic and spectra from the run.")]
	public class RunModule : IModule
	{
		private readonly IRegionManager _regionManager;
		private readonly IEventAggregator _eventAggregator;

		[ImportingConstructor]
		public RunModule(IRegionManager regionManager, IEventAggregator eventAggregator)
		{
			_regionManager = regionManager;
			_eventAggregator = eventAggregator;

			eventAggregator.GetEvent<FileClickedEvent>().Subscribe(OnFileClicked, ThreadOption.PublisherThread, true, FilterClicks);
		}

		public void Initialize()
		{
		}

		public void OnFileClicked(object value)
		{
			RunViewModel viewModel = new RunViewModel(value as Core.Domain.Run);
			ManagedContent view = _regionManager.FindExistingView(Regions.DocumentRegion.ToString(), typeof(RunView), viewModel.XYData.Title);
			if (view == null)
			{
				view = new RunView(_eventAggregator, _regionManager, viewModel, "TIC", "Time");
				_regionManager.AddToRegion(Regions.DocumentRegion.ToString(), view);
			}
			view.Show();
			view.Activate();

			_eventAggregator.GetEvent<ShowPeptidesEvent>().Publish(value);
		}

		private bool FilterClicks(object value)
		{
			return value is Core.Domain.Run;
		}
	}
}
