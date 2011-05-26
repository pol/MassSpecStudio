using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace MassSpecStudio.Modules.ExtensionManager.Views
{
	/// <summary>
	/// Interaction logic for InstallDialogView.xaml
	/// </summary>
	public partial class InstallDialogView : Window
	{
		private string baseDirectory;

		public InstallDialogView()
		{
			baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
			InitializeComponent();
		}

		private void OnInstallModules(object sender, RoutedEventArgs e)
		{
			GetFileToInstall("Modules");
		}

		private void OnInstallProcessingAlgorithms(object sender, RoutedEventArgs e)
		{
			GetFileToInstall("Processing");
		}

		private void OnInstallDataProvider(object sender, RoutedEventArgs e)
		{
			GetFileToInstall("DataProviders");
		}

		private void GetFileToInstall(string subDirectory)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Filter = "Dll Files (*.dll)|*.dll";

			if (dialog.ShowDialog() == true)
			{
				File.Copy(Path.Combine(baseDirectory, subDirectory), dialog.FileName);
				restartMessage.Visibility = Visibility.Visible;
			}
		}

		private void OnClose(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
