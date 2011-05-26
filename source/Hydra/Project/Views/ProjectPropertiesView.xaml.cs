using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Forms;
using AvalonDock;

namespace Hydra.Modules.Project.Views
{
	/// <summary>
	/// Interaction logic for ProjectPropertiesView.xaml
	/// </summary>
	[Export]
	public partial class ProjectPropertiesView : DocumentContent
	{
		[ImportingConstructor]
		public ProjectPropertiesView(ProjectPropertiesViewModel viewModel)
		{
			DataContext = viewModel;
			InitializeComponent();
		}

		public ProjectPropertiesViewModel ViewModel
		{
			get
			{
				return (ProjectPropertiesViewModel)DataContext;
			}
		}

		private void OnSelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			if (e.NewValue != e.OldValue)
			{
				ViewModel.SelectedItem = e.NewValue;
				ViewModel.OnSelectionChanged();
			}
		}

		private void OnBrowse(object sender, RoutedEventArgs e)
		{
			FolderBrowserDialog dialog = new FolderBrowserDialog();
			dialog.SelectedPath = Properties.Settings.Default.LastBrowseDataPath;

			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				ViewModel.SelectedDataPath = dialog.SelectedPath;
			}
		}
	}
}
