using System.ComponentModel.Composition;
using System.Windows;

namespace Hydra.Processing.Algorithm.Views
{
	/// <summary>
	/// Interaction logic for ProcessingDialogView.xaml
	/// </summary>
	[Export(typeof(ProcessingDialogView))]
	public partial class ProcessingDialogView : Window
	{
		private readonly ProcessingDialogViewModel _viewModel;

		[ImportingConstructor]
		public ProcessingDialogView(ProcessingDialogViewModel viewModel)
		{
			_viewModel = viewModel;
			DataContext = viewModel;
			InitializeComponent();
		}

		public ProcessingDialogViewModel ViewModel
		{
			get { return _viewModel; }
		}

		public bool StartProcessing { get; set; }

		private void OnProcess(object sender, RoutedEventArgs e)
		{
			StartProcessing = true;
			Close();
		}

		private void OnCancel(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
