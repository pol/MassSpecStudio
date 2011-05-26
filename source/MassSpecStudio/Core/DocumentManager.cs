using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using MassSpecStudio.Core.Domain;
using MassSpecStudio.Core.Events;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;

namespace MassSpecStudio.Core
{
	[Export(typeof(IDocumentManager))]
	public class DocumentManager : IDocumentManager
	{
		private IEventAggregator _eventAggregator;
		private IServiceLocator _serviceLocator;
		private ProjectBase _projectFile;
		private IList<ExperimentBase> _experiments;
		private IEnumerable<IExperimentType> _experimentTypes;

		[ImportingConstructor]
		public DocumentManager(IEventAggregator eventAggregator, IServiceLocator serviceLocator)
		{
			Save = new DelegateCommand<string>(OnSave, CanSave);
			Close = new DelegateCommand<string>(OnClose, CanClose);
			New = new DelegateCommand<string>(OnNew, CanNew);
			_experiments = new List<ExperimentBase>();
			_eventAggregator = eventAggregator;
			_serviceLocator = serviceLocator;
			_experimentTypes = _serviceLocator.GetAllInstances<IExperimentType>();
		}

		public ProjectBase ProjectFile
		{
			get { return _projectFile; }
			set { _projectFile = value; }
		}

		public IList<ExperimentBase> Experiments
		{
			get { return _experiments; }
		}

		public bool IsProjectOpen
		{
			get { return _projectFile != null; }
		}

		public DelegateCommand<string> New
		{
			get;
			private set;
		}

		public DelegateCommand<string> Save
		{
			get;
			private set;
		}

		public DelegateCommand<string> Close
		{
			get;
			private set;
		}

		public bool IsDirty
		{
			get { throw new NotImplementedException(); }
		}

		public void Open(string path)
		{
			_projectFile = ProjectBase.Open(path);
			_eventAggregator.GetEvent<ProjectOpeningEvent>().Publish(null);

			_projectFile.Experiments.Clear();
			Experiments.Clear();
			foreach (MassSpecStudio.Core.Domain.ProjectBase.ExperimentReference experimentFile in _projectFile.ExperimentReferences)
			{
				ExperimentBase experiment = OpenExperiment(experimentFile, _projectFile);
				Experiments.Add(experiment);
				_projectFile.Experiments.Add(experiment);
			}

			_eventAggregator.GetEvent<LoadProjectEvent>().Publish(_projectFile);
			SaveRecentProjectList(path);
			Save.RaiseCanExecuteChanged();
		}

		public void CloseProject()
		{
			_projectFile = null;
			Experiments.Clear();
			Save.RaiseCanExecuteChanged();
		}

		public void OnSave(string value)
		{
			_projectFile.Save();
		}

		public bool CanSave(string value)
		{
			return IsProjectOpen;
		}

		public void OnClose(string value)
		{
			CloseProject();
		}

		public bool CanClose(string value)
		{
			return IsProjectOpen;
		}

		public void OnNew(string value)
		{
			CloseProject();
			_eventAggregator.GetEvent<NewProjectEvent>().Publish(string.Empty);
		}

		public bool CanNew(string value)
		{
			return true;
		}

		private ExperimentBase OpenExperiment(MassSpecStudio.Core.Domain.ProjectBase.ExperimentReference experimentFile, ProjectBase project)
		{
			IExperimentType experimentType = _experimentTypes.Where(data => data.ExperimentType == experimentFile.ExperimentType).First();

			ExperimentBase experiment = experimentType.Open(Path.Combine(project.Directory, experimentFile.Location), project);
			experiment.ExperimentTypeObject = experimentType;
			return experiment;
		}

		private void SaveRecentProjectList(string projectFileName)
		{
			Core.Properties.Settings.Default.RecentProjects = RecentListHelper.AddToRecentList(Core.Properties.Settings.Default.RecentProjects, projectFileName);
		}
	}
}