using System;
using Hydra.Core.Domain;
using Hydra.Modules.Project.Views;
using MassSpecStudio.Core;

namespace Hydra.Modules.Project.WizardViews
{
	public class ImportPeptidesViewModel : WizardStepViewModel
	{
		private readonly ExperimentViewModel _experimentViewModel;
		private string _selectedDataPath;
		private string[] _recentDataLocations;

		public ImportPeptidesViewModel(ExperimentViewModel experimentViewModel)
		{
			_experimentViewModel = experimentViewModel;
			ViewModel = experimentViewModel.Samples;
			_selectedDataPath = Properties.Settings.Default.LastBrowsePeptideDataPath;
			LoadRecentLocations();
		}

		public override string Title
		{
			get { return "Import peptides"; }
		}

		public SamplesViewModel ViewModel
		{
			get;
			private set;
		}

		public string SelectedFileName
		{
			get
			{
				return _selectedDataPath;
			}

			set
			{
				_selectedDataPath = value;

				Properties.Settings.Default.LastBrowsePeptideDataPath = _selectedDataPath;
				Properties.Settings.Default.RecentBrowsePeptideDataPaths = RecentListHelper.AddToRecentList(Properties.Settings.Default.RecentBrowsePeptideDataPaths, _selectedDataPath);
				Properties.Settings.Default.Save();

				LoadRecentLocations();
				NotifyPropertyChanged(() => SelectedFileName);
				NotifyPropertyChanged(() => CanNext);
			}
		}

		public string[] RecentDataLocations
		{
			get
			{
				return _recentDataLocations;
			}

			set
			{
				_recentDataLocations = value;
				NotifyPropertyChanged(() => RecentDataLocations);
			}
		}

		public override bool CanNext
		{
			get
			{
				return !string.IsNullOrEmpty(SelectedFileName);
			}
		}

		public override string NextButtonText
		{
			get { return "Finish"; }
		}

		public override Type ViewType
		{
			get
			{
				return typeof(ImportPeptidesView);
			}
		}

		public override bool OnNext()
		{
			((Experiment)_experimentViewModel.Data).PeptideFilePendingImport = SelectedFileName;
			return true;
		}

		private void LoadRecentLocations()
		{
			RecentDataLocations = Properties.Settings.Default.RecentBrowsePeptideDataPaths.Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries);
		}
	}
}
