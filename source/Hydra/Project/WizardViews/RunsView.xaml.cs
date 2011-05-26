using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace Hydra.Modules.Project.WizardViews
{
	/// <summary>
	/// Interaction logic for RunsView.xaml
	/// </summary>
	[Export]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	public partial class RunsView : UserControl
	{
		public RunsView()
		{
			InitializeComponent();
		}

		public RunsViewModel ViewModel
		{
			get { return DataContext as RunsViewModel; }
		}

		private void OnSelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			if (e.NewValue != e.OldValue && ViewModel != null)
			{
				ViewModel.SelectedItem = e.NewValue;
			}
		}

		private void OnBrowse(object sender, RoutedEventArgs e)
		{
			System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
			dialog.SelectedPath = Properties.Settings.Default.LastBrowseDataPath;

			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				ViewModel.SelectedDataPath = dialog.SelectedPath;
			}
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			ViewModel.Load();
		}
	}
}
