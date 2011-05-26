using System.Windows;
using AvalonDock;
using MassSpecStudio.Core.Extensions;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

namespace Hydra.Modules.Run.Views
{
	/// <summary>
	/// Interaction logic for RunView.xaml
	/// </summary>
	public partial class RunView : DocumentContent
	{
		private readonly IEventAggregator _eventAggregator;
		private readonly IRegionManager _regionManager;
		private readonly RunViewModel _viewModel;

		public RunView(IEventAggregator eventAggregator, IRegionManager regionManager, RunViewModel viewModel, string title, string xAxis)
		{
			_eventAggregator = eventAggregator;
			_regionManager = regionManager;
			_viewModel = viewModel;

			DataContext = viewModel;
			IsToolBarVisible = title == "TIC" ? Visibility.Visible : Visibility.Collapsed;
			InitializeComponent();

			graphView.LoadGraph(xAxis, title, viewModel.XYData.XValues, viewModel.XYData.YValues);
			listView.ItemsSource = viewModel.XYData.GetXYPairs();
		}

		public Visibility IsToolBarVisible { get; set; }

		private void OnXic(object sender, System.Windows.RoutedEventArgs e)
		{
			XicSelectionDialog dialog = new XicSelectionDialog(_eventAggregator, _regionManager, _viewModel.Run);
			dialog.ShowDialog();
		}

		private void OnSpectrum(object sender, System.Windows.RoutedEventArgs e)
		{
			SpectrumSelectionDialog dialog = new SpectrumSelectionDialog(_eventAggregator, _regionManager, _viewModel.Run);
			dialog.ShowDialog();
		}
	}
}
