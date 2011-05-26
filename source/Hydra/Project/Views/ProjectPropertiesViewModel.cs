using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Hydra.Core;
using MassSpecStudio.Core;
using MassSpecStudio.Core.Events;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;

namespace Hydra.Modules.Project.Views
{
	[Export]
	public class ProjectPropertiesViewModel : ViewModelBase
	{
		private SamplesViewModel _samples;
		private IEventAggregator _eventAggregator;
		private string _selectedDataPath;
		private string[] _files;
		private ProjectViewModel _project;

		[ImportingConstructor]
		public ProjectPropertiesViewModel(IEventAggregator eventAggregator)
		{
			_eventAggregator = eventAggregator;
			_project = DocumentCache.ProjectViewModel as ProjectViewModel;

			_samples = _project.Children.First().Samples;
			AddRunData = new DelegateCommand<IList>(OnAddRunData);
			_selectedDataPath = Properties.Settings.Default.LastBrowseDataPath;
			LoadFiles();
		}

		public DelegateCommand<IList> AddRunData { get; set; }

		public SamplesViewModel Samples
		{
			get { return _samples; }
		}

		public string SelectedDataPath
		{
			get
			{
				return _selectedDataPath;
			}

			set
			{
				_selectedDataPath = value;
				NotifyPropertyChanged(() => SelectedDataPath);
				LoadFiles();
			}
		}

		public string[] Files
		{
			get { return _files; }
		}

		public ObservableCollection<ProteinStateViewModel> ProteinStates
		{
			get { return _samples.Children; }
		}

		public object SelectedItem { get; set; }

		public void OnSelectionChanged()
		{
			_eventAggregator.GetEvent<ObjectSelectionEvent>().Publish(SelectedItem);
		}

		private void LoadFiles()
		{
			if (Directory.Exists(_selectedDataPath))
			{
				_files = Directory.GetFiles(SelectedDataPath, "*.mzml");
				Properties.Settings.Default.LastBrowseDataPath = SelectedDataPath;

				NotifyPropertyChanged(() => Files);
				Properties.Settings.Default.Save();
			}
		}

		private void OnAddRunData(IList values)
		{
			if (SelectedItem != null && !string.IsNullOrEmpty(SelectedDataPath))
			{
				foreach (string value in values)
				{
					((LabelingViewModel)SelectedItem).AddRunDataAndCopyFile.Execute(value);
				}
			}
		}
	}
}
