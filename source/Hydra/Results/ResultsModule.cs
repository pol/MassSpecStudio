using System.ComponentModel.Composition;
using AvalonDock;
using Hydra.Core.Domain;
using Hydra.Core.Events;
using Hydra.Modules.Results.Views;
using MassSpecStudio.Core;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;

namespace Hydra.Modules.Results
{
	[ModuleExport(typeof(ResultsModule))]
	[ExportMetadata("Title", "Results")]
	[ExportMetadata("Type", "Module")]
	[ExportMetadata("Author", "Hydra")]
	[ExportMetadata("Description", "This module provides a user interface for results viewing.")]
	public class ResultsModule : IModule
	{
		private readonly IRegionManager _regionManager;
		private readonly IEventAggregator _eventAggregator;

		[ImportingConstructor]
		public ResultsModule(IRegionManager regionManager, IEventAggregator eventAggregator)
		{
			_regionManager = regionManager;
			_eventAggregator = eventAggregator;

			eventAggregator.GetEvent<ViewResultsEvent>().Subscribe(OnViewResults, ThreadOption.UIThread);
			eventAggregator.GetEvent<FileClickedEvent>().Subscribe(OnFileClicked, ThreadOption.PublisherThread, true, FilterClicks);
		}

		public void Initialize()
		{
		}

		public void OnFileClicked(object value)
		{
			OnViewResults(value as Result);
		}

		private bool FilterClicks(object value)
		{
			return value is Result;
		}

		private void OnViewResults(Result value)
		{
			ManagedContent view = _regionManager.FindExistingView(Regions.DocumentRegion.ToString(), typeof(ResultsView), value.Name);
			if (view == null)
			{
				view = new ResultsView(new ResultsViewModel(value, _eventAggregator));
				_regionManager.AddToRegion(Regions.DocumentRegion.ToString(), view);
			}
			view.Show();
			view.Activate();
		}
	}
}
