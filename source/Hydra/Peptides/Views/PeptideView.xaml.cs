using System.ComponentModel.Composition;
using AvalonDock;

namespace Hydra.Modules.Peptides.Views
{
	/// <summary>
	/// Interaction logic for PeptideProperties.xaml
	/// </summary>
	[Export(typeof(PeptideView))]
	public partial class PeptideView : DocumentContent
	{
		[ImportingConstructor]
		public PeptideView(PeptideViewModel viewModel)
		{
			DataContext = viewModel;
			InitializeComponent();
		}
	}
}
