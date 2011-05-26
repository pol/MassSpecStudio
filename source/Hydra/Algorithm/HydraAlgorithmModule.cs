using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows;
using Hydra.Core;
using Hydra.Processing.Algorithm.Views;
using MassSpecStudio.Core.Events;
using MassSpecStudio.Core.Processing;
using MassSpecStudio.UI.Controls;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.ServiceLocation;

namespace Hydra.Processing.Algorithm
{
	[ModuleExport(typeof(HydraAlgorithmModule))]
	[ExportMetadata("Title", "Hydra Processing Algorithms")]
	[ExportMetadata("Type", "Processing")]
	[ExportMetadata("Author", "Hydra")]
	[ExportMetadata("Description", "This module provides algorithms two algorithms (Label Amount Calculator and MSMS Fragment Analyzer) for analyzing HDX data.")]
	public class HydraAlgorithmModule : IModule
	{
		private readonly IEventAggregator _eventAggregator;
		private readonly IServiceLocator _serviceLocator;
		private IAlgorithm _algorithm;

		[ImportingConstructor]
		public HydraAlgorithmModule(IEventAggregator eventAggregator, IServiceLocator serviceLocator)
		{
			// Example for service locator: IChromatographicPeakDetection peak = serviceLocator.GetInstance<IChromatographicPeakDetection>() as IChromatographicPeakDetection;
			_eventAggregator = eventAggregator;
			_serviceLocator = serviceLocator;
			_eventAggregator.GetEvent<ProcessingDialogEvent>().Subscribe(ShowProcessingDialog);
		}

		public void Initialize()
		{
		}

		public void ShowProcessingDialog(object value)
		{
			_eventAggregator.GetEvent<StatusUpdateEvent>().Publish("Ready");
			ProcessingDialogView dialog = new ProcessingDialogView(_serviceLocator.GetInstance<ProcessingDialogViewModel>());
			dialog.Owner = Application.Current.MainWindow;
			dialog.ShowDialog();

			if (dialog.StartProcessing)
			{
				_eventAggregator.GetEvent<StatusUpdateEvent>().Publish("Processing...");
				_algorithm = dialog.ViewModel.SelectedAlgorithm;
				ProgressDialog progressDialog = new ProgressDialog("Processing");
				progressDialog.Owner = Application.Current.MainWindow;
				progressDialog.RunWorkerThread(DoWork);
			}
		}

		private void DoWork(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = (BackgroundWorker)sender;

			_algorithm.Execute(worker, DocumentCache.Experiment);
		}
	}
}
