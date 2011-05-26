using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;

namespace MSStudio.Modules.MainMenu
{
	[ModuleExport(typeof(MainMenuModule))]
	public class MainMenuModule : IModule
	{
		private readonly IRegionManager _regionManager;

		[ImportingConstructor]
		public MainMenuModule(IRegionManager regionManager)
		{
			_regionManager = regionManager;
		}

		public void Initialize()
		{
			////_regionManager.RegisterViewWithRegion("MainMenuRegion", typeof(Views.MainMenuView));
		}
	}
}
