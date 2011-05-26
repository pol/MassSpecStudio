using System.ComponentModel.Composition;
using AvalonDock;

namespace MassSpecStudio.Modules.Properties.Views
{
	/// <summary>
	/// Interaction logic for PropertiesView.xaml
	/// </summary>
	[Export]
	public partial class PropertiesView : DockableContent
	{
		[ImportingConstructor]
		public PropertiesView(PropertiesViewModel viewModel)
		{
			this.DataContext = viewModel;
			InitializeComponent();
		}
	}
}
