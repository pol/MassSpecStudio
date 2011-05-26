using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hydra.Core.Domain;
using Hydra.Modules.Project.Views;
using MassSpecStudio.Core;
using Microsoft.Practices.Prism.Commands;

namespace Hydra.Modules.Project.WizardViews
{
	public class RunsViewModel : WizardStepViewModel
	{
		private readonly ExperimentViewModel _experimentViewModel;
		private string _selectedDataPath;
		private object _selectedItem;
		private string[] _recentBrowseLocations;
		private string[] _files;
		private string _selectedData;
		private string _selectedFileType;
		private IDataProvider _dataProvider;

		public RunsViewModel(ExperimentViewModel experimentViewModel)
		{
			_experimentViewModel = experimentViewModel;
			ViewModel = experimentViewModel.Samples;
			AddRunData = new DelegateCommand<IList>(OnAddRunData, CanAddRunData);
			RemoveRunData = new DelegateCommand<object>(OnRemoveRunData, CanRemoveRunData);
			_selectedDataPath = Properties.Settings.Default.LastBrowseDataPath;
			LoadRecentLocations();
			LoadFiles();
		}

		public override string Title
		{
			get { return "Add Runs"; }
		}

		public string[] FileTypes
		{
			get { return _dataProvider != null ? _dataProvider.FileTypes : null; }
		}

		public string SelectedFileType
		{
			get
			{
				return _selectedFileType;
			}

			set
			{
				_selectedFileType = value;
				LoadFiles();
				NotifyPropertyChanged(() => SelectedFileType);
			}
		}

		public SamplesViewModel ViewModel
		{
			get;
			private set;
		}

		public DelegateCommand<IList> AddRunData { get; set; }

		public DelegateCommand<object> RemoveRunData { get; set; }

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

		public string[] RecentBrowseLocations
		{
			get
			{
				return _recentBrowseLocations;
			}

			set
			{
				_recentBrowseLocations = value;
				NotifyPropertyChanged(() => RecentBrowseLocations);
			}
		}

		public string[] Files
		{
			get { return _files; }
		}

		public object SelectedItem
		{
			get
			{
				return _selectedItem;
			}

			set
			{
				_selectedItem = value;
				UpdateAddRemoveButtons();
			}
		}

		public string SelectedData
		{
			get
			{
				return _selectedData;
			}

			set
			{
				_selectedData = value;
				UpdateAddRemoveButtons();
			}
		}

		public override Type ViewType
		{
			get { return typeof(RunsView); }
		}

		public void Load()
		{
			LoadFiles();
		}

		private void LoadFiles()
		{
			if (Directory.Exists(_selectedDataPath))
			{
				// TODO: support multiple file formats.
				List<string> files = new List<string>();
				IDataProvider dataProvider = ((Experiment)_experimentViewModel.Data).DataProvider;
				if (dataProvider != null && dataProvider != _dataProvider)
				{
					_dataProvider = dataProvider;
					NotifyPropertyChanged(() => FileTypes);
				}

				if (dataProvider != null)
				{
					if (string.IsNullOrEmpty(SelectedFileType))
					{
						SelectedFileType = FileTypes.First();
					}

					string[] allFiles = Directory.GetFiles(SelectedDataPath);
					foreach (string file in allFiles)
					{
						if (_dataProvider.IsCorrectFileType(file, SelectedFileType))
						{
							files.Add(file);
						}
					}

					Properties.Settings.Default.LastBrowseDataPath = SelectedDataPath;
					Properties.Settings.Default.RecentBrowseDataPaths = RecentListHelper.AddToRecentList(Properties.Settings.Default.RecentBrowseDataPaths, _selectedDataPath);
					LoadRecentLocations();

					_files = files.ToArray();
					NotifyPropertyChanged(() => Files);
					Properties.Settings.Default.Save();
				}
			}
		}

		private void OnAddRunData(IList values)
		{
			if (SelectedItem != null && SelectedItem is LabelingViewModel)
			{
				foreach (string value in values)
				{
					if (((Experiment)_experimentViewModel.Data).Runs.Where(run => run.FileName == value).FirstOrDefault() == null)
					{
						string file = Path.Combine(SelectedDataPath, value);
						if (File.Exists(file))
						{
							((LabelingViewModel)SelectedItem).AddRunData(file);
						}
					}
				}
			}
			values.Clear();
		}

		private bool CanAddRunData(IList values)
		{
			return SelectedData != null && SelectedItem != null && SelectedItem is LabelingViewModel;
		}

		private void OnRemoveRunData(object value)
		{
			if (value != null && value is RunViewModel)
			{
				((RunViewModel)value).Parent.RemoveRunData.Execute((RunViewModel)value);
			}
		}

		private bool CanRemoveRunData(object value)
		{
			return value != null && value is RunViewModel;
		}

		private void LoadRecentLocations()
		{
			RecentBrowseLocations = Properties.Settings.Default.RecentBrowseDataPaths.Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries);
		}

		private void UpdateAddRemoveButtons()
		{
			AddRunData.RaiseCanExecuteChanged();
			RemoveRunData.RaiseCanExecuteChanged();
		}
	}
}
