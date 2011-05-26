using System.ComponentModel.Composition;
using AvalonDock;

namespace Hydra.Modules.Peptides.Views
{
	/// <summary>
	/// Interaction logic for PeptidesView.xaml
	/// </summary>
	[Export(typeof(PeptidesView))]
	public partial class PeptidesView : DockableContent
	{
		[ImportingConstructor]
		public PeptidesView(PeptidesViewModel viewModel)
		{
			DataContext = viewModel;
			InitializeComponent();
		}
	}
}
