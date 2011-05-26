using System.Windows;
using System.Windows.Forms;

namespace MassSpecStudio.Modules.Project.Views
{
	/// <summary>
	/// Interaction logic for NewExperimentView.xaml
	/// </summary>
	public partial class NewProjectView : Window
	{
		public NewProjectView(NewProjectViewModel viewModel)
		{
			this.DataContext = viewModel;
			InitializeComponent();
		}

		public NewProjectViewModel ViewModel
		{
			get { return (NewProjectViewModel)DataContext; }
		}

		private void OnClose(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void OnBrowse(object sender, RoutedEventArgs e)
		{
			FolderBrowserDialog dialog = new FolderBrowserDialog();
			dialog.SelectedPath = Properties.Settings.Default.LastBrowseLocation;

			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				Properties.Settings.Default.LastBrowseLocation = dialog.SelectedPath;
				ViewModel.Location = dialog.SelectedPath;
				Properties.Settings.Default.Save();
			}
		}

		private void OnOK(object sender, RoutedEventArgs e)
		{
			if (ViewModel.IsCreateProjectValid())
			{
				Visibility = System.Windows.Visibility.Hidden;
				if (ViewModel.CreateProject())
				{
					Close();
				}
				else
				{
					Visibility = System.Windows.Visibility.Visible;
				}
			}
		}
	}
}
