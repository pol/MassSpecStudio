using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;

namespace MassSpecStudio.Modules.Properties
{
	[ModuleExport(typeof(PropertiesModule))]
	[ExportMetadata("Title", "Properties Pane")]
	[ExportMetadata("Type", "Module")]
	[ExportMetadata("Author", "Mass Spec Studio")]
	[ExportMetadata("Description", "This module provides a display and the ability to edit any selected element within the project or experiment.")]
	public class PropertiesModule : IModule
	{
		private readonly IRegionManager regionManager;

		[ImportingConstructor]
		public PropertiesModule(IRegionManager regionManager)
		{
			this.regionManager = regionManager;
		}

		public void Initialize()
		{
			regionManager.RegisterViewWithRegion("RightRegion", typeof(Views.PropertiesView));
		}
	}
}
