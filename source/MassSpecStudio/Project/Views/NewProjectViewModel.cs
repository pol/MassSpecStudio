using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MassSpecStudio.Core;
using MassSpecStudio.Core.Domain;
using MassSpecStudio.Core.Events;
using MassSpecStudio.Core.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;

namespace MassSpecStudio.Modules.Project.Views
{
	public class NewProjectViewModel : NotificationObject
	{
		private readonly IEventAggregator _eventAggregator;
		private readonly IDocumentManager _documentManager;
		private string _projectName;
		private string _experimentName;
		private string _location;
		private IList<string> _recentLocations;
		private IEnumerable<IExperimentType> _experimentTypes;
		private IExperimentType _selectedExperimentType;

		public NewProjectViewModel(IEventAggregator eventAggregator, IServiceLocator serviceLocator)
		{
			_eventAggregator = eventAggregator;
			_experimentTypes = serviceLocator.GetAllInstances<IExperimentType>();
			_documentManager = serviceLocator.GetInstance<IDocumentManager>();

			_recentLocations = Properties.Settings.Default.RecentLocations.Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries);

			_selectedExperimentType = _experimentTypes.Where(type => type.Name == Properties.Settings.Default.LastSelectedExperimentType).FirstOrDefault();
			_selectedExperimentType = _selectedExperimentType ?? _experimentTypes.First();

			if (string.IsNullOrEmpty(Properties.Settings.Default.LastBrowseLocation))
			{
				string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				Properties.Settings.Default.LastBrowseLocation = Path.Combine(documents, "Mass Spec Studio Projects");
			}
			_location = Properties.Settings.Default.LastBrowseLocation;
			_projectName = GetNextDirectoryName(_location, _selectedExperimentType.Name + "Project");
			_experimentName = _selectedExperimentType.Name + "Experiment";
		}

		public IEnumerable<IExperimentType> ExperimentTypes
		{
			get { return _experimentTypes; }
			private set { _experimentTypes = value; }
		}

		public IExperimentType SelectedExperimentType
		{
			get
			{
				return _selectedExperimentType;
			}

			set
			{
				_selectedExperimentType = value;
				RaisePropertyChanged(() => SelectedExperimentType);
			}
		}

		[RegexValidator("^[A-Za-z0-9]+$", MessageTemplate = "Directory name contains invalid characters.")]
		[StringLengthValidator(1, 100, MessageTemplate = "Experiment name must be less than 100 characters.")]
		public string ExperimentName
		{
			get
			{
				return _experimentName;
			}

			set
			{
				_experimentName = value;
				RaisePropertyChanged(() => ExperimentName);
			}
		}

		[RegexValidator("^[A-Za-z0-9]+$", MessageTemplate = "Directory name contains invalid characters.")]
		[DirectoryDoesNotExistValidator("Location", MessageTemplate = "A project with this name already exists.")]
		[StringLengthValidator(1, 100, MessageTemplate = "Project name must be less than 100 characters.")]
		public string ProjectName
		{
			get
			{
				return _projectName;
			}

			set
			{
				_projectName = value;

				// Notify if project/directory already exists.
				RaisePropertyChanged(() => ProjectName);
			}
		}

		[DirectoryDoesNotExistValidator("Location", Negated = true, MessageTemplate = "This location does not exist.")]
		public string Location
		{
			get
			{
				return _location;
			}

			set
			{
				_location = value;
				RaisePropertyChanged(() => Location);
			}
		}

		public IList<string> RecentLocations
		{
			get { return _recentLocations; }
		}

		public bool IsCreateProjectValid()
		{
			return Validation.Validate(this).Count() == 0;
		}

		public bool CreateProject()
		{
			if (IsCreateProjectValid())
			{
				Properties.Settings.Default.LastSelectedExperimentType = _selectedExperimentType.Name;
				AddToRecentLocationsList();
				Properties.Settings.Default.Save();

				ProjectBase project = new ProjectBase(ProjectName, Location);
				project.ExperimentReferences.Add(new ProjectBase.ExperimentReference(ExperimentName, DirectoryHelper.GetRelativePath(Path.Combine(Location, ProjectName, ExperimentName, ExperimentName + ".mssexp"), project.Directory), _selectedExperimentType.ExperimentType));

				_eventAggregator.GetEvent<CreateProjectEvent>().Publish(project);
				return true;
			}
			return false;
		}

		private static string GetNextDirectoryName(string startingLocation, string newDirectoryHeader)
		{
			if (!Directory.Exists(startingLocation))
			{
				return newDirectoryHeader + "1";
			}

			string[] directoryNames = Directory.GetDirectories(startingLocation);

			for (int i = 1; i < 100; i++)
			{
				if (!Directory.Exists(Path.Combine(startingLocation, newDirectoryHeader + i)))
				{
					return newDirectoryHeader + i;
				}
			}
			return string.Empty;
		}

		private void AddToRecentLocationsList()
		{
			if (!Properties.Settings.Default.RecentLocations.Contains(_location + ";"))
			{
				Properties.Settings.Default.RecentLocations = _location + ";" + Properties.Settings.Default.RecentLocations;
			}

			string[] locations = Properties.Settings.Default.RecentLocations.Split(';');
			Properties.Settings.Default.RecentLocations = string.Empty;
			for (int i = 0; i < locations.Length && i < 10; i++)
			{
				if (Directory.Exists(locations[i]))
				{
					Properties.Settings.Default.RecentLocations += locations[i] + ";";
				}
			}
		}
	}
}
