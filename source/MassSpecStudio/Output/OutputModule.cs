using System.ComponentModel.Composition;
using MassSpecStudio.Modules.Output.Views;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;

namespace MassSpecStudio.Modules.Project
{
	[ModuleExport(typeof(OutputModule))]
	[ExportMetadata("Title", "Output Pane")]
	[ExportMetadata("Type", "Module")]
	[ExportMetadata("Author", "Mass Spec Studio")]
	[ExportMetadata("Description", "This module displays progress and logs information about processing for viewing by the user.")]
	public class OutputModule : IModule
	{
		private readonly IRegionManager _regionManager;

		[ImportingConstructor]
		public OutputModule(IRegionManager regionManager)
		{
			_regionManager = regionManager;
		}

		public void Initialize()
		{
			_regionManager.RegisterViewWithRegion("BottomRegion", typeof(OutputView));
		}
	}
}
