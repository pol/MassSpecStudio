using System.ComponentModel.Composition;
using AvalonDock;
using Hydra.Core.Domain;
using Hydra.Core.Events;
using Hydra.Modules.Validation.Views;
using Hydra.Processing.Algorithm;
using MassSpecStudio.Core;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;

namespace Hydra.Modules.Validation
{
	[ModuleExport(typeof(ValidationModule))]
	[ExportMetadata("Title", "Validation")]
	[ExportMetadata("Type", "Module")]
	[ExportMetadata("Author", "Hydra")]
	[ExportMetadata("Description", "This module allows manual review of results.")]
	public class ValidationModule : IModule
	{
		private readonly IRegionManager _regionManager;
		private readonly IEventAggregator _eventAggregator;
		private readonly LabelAmountAlgorithm labelAmountAlgorithm;

		[ImportingConstructor]
		public ValidationModule(IRegionManager regionManager, IEventAggregator eventAggregator, LabelAmountAlgorithm labelAmountAlgorithm)
		{
			_regionManager = regionManager;
			_eventAggregator = eventAggregator;
			this.labelAmountAlgorithm = labelAmountAlgorithm;

			_eventAggregator.GetEvent<LoadManualValidationEvent>().Subscribe(OnView);
		}

		public void Initialize()
		{
		}

		public void OnView(Result value)
		{
			ManualValidationViewModel viewModel = new ManualValidationViewModel(value, _eventAggregator, labelAmountAlgorithm);
			ManagedContent view = _regionManager.FindExistingView(Regions.DocumentRegion.ToString(), typeof(ManualValidationView), "Manual Results Validation (TODO:)");
			if (view == null)
			{
				view = new ManualValidationView(viewModel);
				_regionManager.AddToRegion(Regions.DocumentRegion.ToString(), view);
			}
			else
			{
				((ManualValidationView)view).SetViewModel(viewModel);
			}
			view.Show();
			view.Activate();
		}
	}
}
