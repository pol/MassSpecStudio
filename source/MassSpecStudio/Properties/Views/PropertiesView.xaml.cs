using System.ComponentModel.Composition;
using AvalonDock;
using MassSpecStudio.Core.Events;
using Microsoft.Practices.Prism.Events;

namespace MassSpecStudio.Modules.Properties.Views
{
	/// <summary>
	/// Interaction logic for PropertiesView.xaml
	/// </summary>
	[Export]
	public partial class PropertiesView : DockableContent
	{
		[ImportingConstructor]
		public PropertiesView(PropertiesViewModel viewModel, IEventAggregator eventAggregator)
		{
			this.DataContext = viewModel;
			InitializeComponent();

			eventAggregator.GetEvent<ProjectClosedEvent>().Subscribe(OnClosed);
		}

		private void OnClosed(object value)
		{
			Close();
		}
	}
}
