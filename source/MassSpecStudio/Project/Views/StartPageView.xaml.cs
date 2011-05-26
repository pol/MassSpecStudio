using System.ComponentModel.Composition;
using System.Windows.Forms;
using AvalonDock;
using MassSpecStudio.Core.EventArg;

namespace MassSpecStudio.Modules.Project.Views
{
	/// <summary>
	/// Interaction logic for StartPageView.xaml
	/// </summary>
	[Export]
	public partial class StartPageView : DocumentContent
	{
		private readonly StartPageViewModel _viewModel;

		[ImportingConstructor]
		public StartPageView(StartPageViewModel viewModel)
		{
			_viewModel = viewModel;
			viewModel.ErrorMessageEvent += ShowErrorMessage;
			DataContext = viewModel;
			InitializeComponent();
		}

		private void ShowErrorMessage(object sender, MessageEventArgs args)
		{
			MessageBox.Show(args.Message, "Error", MessageBoxButtons.OK);
		}

		private void OnOpen(object sender, System.Windows.RoutedEventArgs e)
		{
			OpenFileDialog openDialog = new OpenFileDialog();
			openDialog.Filter = "(*.mssproj)|*.mssproj";
			DialogResult result = openDialog.ShowDialog();

			if (result == System.Windows.Forms.DialogResult.OK)
			{
				_viewModel.Open(openDialog.FileName);
			}
		}

		private void OnOpenRecent(object sender, System.Windows.RoutedEventArgs e)
		{
			_viewModel.Open(((System.Windows.Controls.Button)sender).Tag.ToString());
		}
	}
}
