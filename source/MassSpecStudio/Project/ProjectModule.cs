using System.ComponentModel.Composition;
using MassSpecStudio.Core.Events;
using MassSpecStudio.Modules.Project.Views;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;

namespace MassSpecStudio.Modules.Project
{
	[ModuleExport(typeof(ProjectModule))]
	[ExportMetadata("Title", "Project Creation")]
	[ExportMetadata("Type", "Module")]
	[ExportMetadata("Author", "Mass Spec Studio")]
	[ExportMetadata("Description", "This module allows users to create new projects and experiments of any installed project type.")]
	public class ProjectModule : IModule
	{
		private readonly IRegionManager _regionManager;
		private readonly IEventAggregator _eventAggregator;
		private readonly IServiceLocator _serviceLocator;

		[ImportingConstructor]
		public ProjectModule(IRegionManager regionManager, IEventAggregator eventAggregator, IServiceLocator serviceLocator)
		{
			_eventAggregator = eventAggregator;
			_regionManager = regionManager;
			_serviceLocator = serviceLocator;
			_eventAggregator.GetEvent<NewProjectEvent>().Subscribe(ShowNewProjectWindow);
		}

		public void Initialize()
		{
			_regionManager.RegisterViewWithRegion("DocumentRegion", typeof(StartPageView));
		}

		public void ShowNewProjectWindow(string value)
		{
			NewProjectView dialog = new NewProjectView(new NewProjectViewModel(_eventAggregator, _serviceLocator));
			dialog.Show();
		}
	}
}
