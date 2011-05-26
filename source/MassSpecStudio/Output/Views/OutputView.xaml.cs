using System.ComponentModel.Composition;
using AvalonDock;
using MassSpecStudio.Core.Events;
using Microsoft.Practices.Prism.Events;

namespace MassSpecStudio.Modules.Output.Views
{
	/// <summary>
	/// Interaction logic for OutputView.xaml
	/// </summary>
	[Export(typeof(OutputView))]
	public partial class OutputView : DockableContent
	{
		[ImportingConstructor]
		public OutputView(IEventAggregator eventAggregator, OutputViewModel viewModel)
		{
			eventAggregator.GetEvent<OutputEvent>().Subscribe(OnOutput, ThreadOption.UIThread);
			DataContext = viewModel;
			InitializeComponent();
		}

		private void OnOutput(string value)
		{
			Show();
			outputScroller.ScrollToEnd();
		}
	}
}
