using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace MassSpecStudio.Modules.StatusBar.Views
{
	/// <summary>
	/// Interaction logic for StatusBarView.xaml
	/// </summary>
	[Export]
	public partial class StatusBarView : UserControl
	{
		[ImportingConstructor]
		public StatusBarView(StatusBarViewModel viewModel)
		{
			this.DataContext = viewModel;
			InitializeComponent();
		}
	}
}
