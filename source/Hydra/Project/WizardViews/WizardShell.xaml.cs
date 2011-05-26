using System;
using System.Windows;
using Hydra.Core.Domain;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;

namespace Hydra.Modules.Project.WizardViews
{
	/// <summary>
	/// Interaction logic for WizardShell.xaml
	/// </summary>
	public partial class WizardShell : Window
	{
		private readonly WizardViewModel _controller;
		private readonly IRegionManager _regionManager;

		public WizardShell(IRegionManager regionManager, WizardViewModel controller)
		{
			Finished = false;
			_regionManager = regionManager;
			_controller = controller;
			_controller.FinishedEvent += OnFinished;
			InitializeComponent();

			RegionManager.SetRegionManager(this, regionManager);
			_controller.InitializeView();
			DataContext = _controller;
		}

		public bool Finished { get; set; }

		public WizardViewModel Controller
		{
			get { return _controller; }
		}

		private void OnFinished(object sender, EventArgs e)
		{
			Finished = true;
			Close();
		}

		private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			_regionManager.Regions.Remove("WizardContentRegion");
			RegionManager.UpdateRegions();
		}
	}
}
