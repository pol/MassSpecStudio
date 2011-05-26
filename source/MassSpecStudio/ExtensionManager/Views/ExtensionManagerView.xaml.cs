using System.ComponentModel.Composition;
using System.Windows;

namespace MassSpecStudio.Modules.ExtensionManager.Views
{
	/// <summary>
	/// Interaction logic for ExtensionManagerView.xaml
	/// </summary>
	[Export]
	public partial class ExtensionManagerView : Window
	{
		[ImportingConstructor]
		public ExtensionManagerView(ExtensionManagerViewModel viewModel)
		{
			this.DataContext = viewModel;
			InitializeComponent();
		}

		private void OnClose(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void OnInstall(object sender, RoutedEventArgs e)
		{
			InstallDialogView dialog = new InstallDialogView();
			dialog.Owner = this;
			dialog.ShowDialog();
		}
	}
}
