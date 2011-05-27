using System.ComponentModel.Composition;
using AvalonDock;
using Microsoft.Win32;

namespace Hydra.Modules.Results.Views
{
	/// <summary>
	/// Interaction logic for ResultsView.xaml
	/// </summary>
	[Export(typeof(ResultsView))]
	public partial class ResultsView : DocumentContent
	{
		private ResultsViewModel viewModel;

		[ImportingConstructor]
		public ResultsView(ResultsViewModel viewModel)
		{
			DataContext = viewModel;
			this.viewModel = viewModel;
			InitializeComponent();

			IsActiveContentChanged += new System.EventHandler(ResultsView_IsActiveContentChanged);
		}

		private void ResultsView_IsActiveContentChanged(object sender, System.EventArgs e)
		{
			if (IsActiveDocument)
			{
				viewModel.Refresh();
			}
		}

		private void OnExport(object sender, System.Windows.RoutedEventArgs e)
		{
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";

			if (dialog.ShowDialog() == true)
			{
				viewModel.SaveExcel(dialog.FileName);
			}
		}
	}
}
