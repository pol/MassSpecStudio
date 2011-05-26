using System.Windows;
using AvalonDock;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

namespace Hydra.Modules.Run.Views
{
	/// <summary>
	/// Interaction logic for XicSelectionDialog.xaml
	/// </summary>
	public partial class XicSelectionDialog : Window
	{
		private readonly XicSelectionDialogViewModel _viewModel;
		private readonly IRegionManager _regionManager;
		private readonly Core.Domain.Run _run;
		private readonly IEventAggregator _eventAggregator;

		public XicSelectionDialog(IEventAggregator eventAggregator, IRegionManager regionManager, Core.Domain.Run run)
		{
			_regionManager = regionManager;
			_run = run;
			_eventAggregator = eventAggregator;

			_viewModel = new XicSelectionDialogViewModel();
			DataContext = _viewModel;
			InitializeComponent();
		}

		private void OnRetrieve(object sender, RoutedEventArgs e)
		{
			RunViewModel viewModel = new RunViewModel(_run, _viewModel.Mass, _viewModel.MzTolerance);
			ManagedContent view = _regionManager.FindExistingView("DocumentRegion", typeof(RunView), viewModel.XYData.Title);
			if (view == null)
			{
				view = new RunView(_eventAggregator, _regionManager, viewModel, "XIC", "Time");
				_regionManager.AddToRegion("DocumentRegion", view);
			}
			view.Show();
			view.Activate();
			Close();
		}

		private void OnCancel(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
