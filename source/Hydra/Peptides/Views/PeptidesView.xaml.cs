using System.ComponentModel.Composition;
using AvalonDock;
using Hydra.Core.Domain;
using MassSpecStudio.Core.Events;
using Microsoft.Practices.Prism.Events;

namespace Hydra.Modules.Peptides.Views
{
	/// <summary>
	/// Interaction logic for PeptidesView.xaml
	/// </summary>
	[Export(typeof(PeptidesView))]
	public partial class PeptidesView : DockableContent
	{
		[ImportingConstructor]
		public PeptidesView(PeptidesViewModel viewModel, IEventAggregator eventAggregator)
		{
			DataContext = viewModel;
			InitializeComponent();
			eventAggregator.GetEvent<ProjectClosedEvent>().Subscribe(OnClosed);
		}

		public void Refresh(Result result)
		{
			((PeptidesViewModel)DataContext).Load(result);
		}

		private void OnClosed(object value)
		{
			Close();
		}
	}
}
