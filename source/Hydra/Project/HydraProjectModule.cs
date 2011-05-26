using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using Hydra.Core;
using Hydra.Core.Domain;
using Hydra.Modules.Project.Views;
using Hydra.Modules.Project.WizardViews;
using MassSpecStudio.Core;
using MassSpecStudio.Core.Domain;
using MassSpecStudio.Core.Events;
using MassSpecStudio.UI.Controls;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;

namespace Hydra.Modules.Project
{
	[ModuleExport(typeof(HydraProjectModule))]
	[ExportMetadata("Title", "Project Explorer")]
	[ExportMetadata("Type", "Module")]
	[ExportMetadata("Author", "Hydra")]
	[ExportMetadata("Description", "This module allows the user to create new projects via a wizard and also include a tree view of the project and experiments.")]
	public class HydraProjectModule : IModule
	{
		private readonly IRegionManager _regionManager;
		private readonly IEventAggregator _eventAggregator;
		private readonly IDocumentManager _documentManager;
		private readonly IServiceLocator _serviceLocator;

		[ImportingConstructor]
		public HydraProjectModule(IRegionManager regionManager, IEventAggregator eventAggregator, IDocumentManager documentManager, IServiceLocator serviceLocator)
		{
			_regionManager = regionManager;
			_eventAggregator = eventAggregator;
			_documentManager = documentManager;
			_serviceLocator = serviceLocator;

			eventAggregator.GetEvent<CreateProjectEvent>().Subscribe(OnCreateProject);
			eventAggregator.GetEvent<LoadProjectEvent>().Subscribe(OnLoadProject);
			eventAggregator.GetEvent<ProjectClosedEvent>().Subscribe(OnProjectClosed);
		}

		public void Initialize()
		{
		}

		private void OnLoadProject(ProjectBase project)
		{
			_regionManager.Display(Regions.LeftRegion.ToString(), typeof(Views.ProjectExplorerView), _serviceLocator);
			DocumentCache.ProjectViewModel = new ProjectViewModel(project);
			DocumentCache.Experiment = project.Experiments.First() as Experiment;
			_eventAggregator.GetEvent<LoadProjectDataEvent>().Publish(DocumentCache.ProjectViewModel);
		}

		private void OnCreateProject(ProjectBase project)
		{
			CreateProjectHelper.ConvertExperimentReferencesToExperiments(project, _serviceLocator);

			Experiment experiment = project.Experiments.First() as Experiment;

			WizardShell wizard = new WizardShell(_regionManager, CreateProjectHelper.CreateController(experiment, _regionManager, _serviceLocator));
			wizard.ShowDialog();

			if (wizard.Finished)
			{
				ProgressDialog dialog = new ProgressDialog("Creating experiment");

				dialog.RunWorkerThread(project, DoCreateProjectAndExperimentWork);

				_documentManager.Open(project.Location);
			}
		}

		private void DoCreateProjectAndExperimentWork(object sender, DoWorkEventArgs e)
		{
			// the sender property is a reference to the dialog's BackgroundWorker
			// component
			BackgroundWorker worker = (BackgroundWorker)sender;
			ProjectBase project = e.Argument as ProjectBase;
			CreateProjectHelper helper = new CreateProjectHelper(worker, project);

			worker.ReportProgress(50, "Creating project and experiment");
			helper.CreateProjectDirectoryStructure();
			helper.CopyDataFiles();
			helper.ReadPeptides();
			worker.ReportProgress(50, "Saving");
			project.Save();
		}

		private void OnProjectClosed(object value)
		{
			_regionManager.Remove(Regions.LeftRegion.ToString(), typeof(ProjectExplorerView));
		}
	}
}
