using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace Hydra.Modules.Project.WizardViews
{
	/// <summary>
	/// Interaction logic for ImportPeptidesView.xaml
	/// </summary>
	[Export]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	public partial class ImportPeptidesView : UserControl
	{
		public ImportPeptidesView()
		{
			InitializeComponent();
		}

		public ImportPeptidesViewModel ViewModel
		{
			get { return DataContext as ImportPeptidesViewModel; }
		}

		private void OnBrowse(object sender, RoutedEventArgs e)
		{
			System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
			dialog.FileName = ViewModel.SelectedFileName;
			dialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";

			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				ViewModel.SelectedFileName = dialog.FileName;
			}
		}
	}
}
