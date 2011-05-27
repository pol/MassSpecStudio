using System;
using System.ComponentModel;
using System.Windows;
using AvalonDock;
using MassSpecStudio.UI.Controls;

namespace Hydra.Modules.Validation.Views
{
	/// <summary>
	/// Interaction logic for ManualValidationView.xaml
	/// </summary>
	public partial class ManualValidationView : DocumentContent
	{
		private ManualValidationViewModel viewModel;

		public ManualValidationView(ManualValidationViewModel viewModel)
		{
			SetViewModel(viewModel);
			IsActiveContentChanged += new EventHandler(ManualValidationView_IsActiveContentChanged);

			InitializeComponent();
		}

		public void SetViewModel(ManualValidationViewModel newViewModel)
		{
			viewModel = newViewModel;
			viewModel.UpdateGraphs += HandleUpdateGraphs;
			viewModel.Reprocess += HandleReprocessing;
			DataContext = viewModel;
		}

		private void ManualValidationView_IsActiveContentChanged(object sender, EventArgs e)
		{
			if (IsActiveDocument)
			{
				viewModel.RefreshPropertiesPanel();
			}
		}

		private void HandleUpdateGraphs(object sender, EventArgs e)
		{
			xicSpectrumView.UpdateGraphs();
		}

		private void HandleReprocessing(object sender, EventArgs e)
		{
			ProgressDialog progressDialog = new ProgressDialog("Processing");
			progressDialog.Owner = Application.Current.MainWindow;
			progressDialog.RunWorkerThread(DoWork);
		}

		private void DoWork(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = (BackgroundWorker)sender;

			viewModel.ReprocessSelectedResult(worker);
		}
	}
}
