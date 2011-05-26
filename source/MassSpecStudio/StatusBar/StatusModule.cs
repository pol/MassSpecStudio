using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;

namespace MassSpecStudio.Modules.StatusBar
{
	[ModuleExport(typeof(StatusModule))]
	[ExportMetadata("Title", "Status Bar")]
	[ExportMetadata("Type", "Module")]
	[ExportMetadata("Author", "Mass Spec Studio")]
	[ExportMetadata("Description", "This module displays the status bar and provides status services that other modules can subscribe to in order to publish status information.")]
	public class StatusModule : IModule
	{
		private readonly IRegionManager _regionManager;

		[ImportingConstructor]
		public StatusModule(IRegionManager regionManager)
		{
			_regionManager = regionManager;
		}

		public void Initialize()
		{
			_regionManager.RegisterViewWithRegion("StatusRegion", typeof(Views.StatusBarView));
		}
	}
}
