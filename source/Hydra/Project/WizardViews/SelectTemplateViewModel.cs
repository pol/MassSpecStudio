using System;
using System.IO;
using Hydra.Core.Domain;
using MassSpecStudio.Core;
using Microsoft.Practices.ServiceLocation;

namespace Hydra.Modules.Project.WizardViews
{
	public class SelectTemplateViewModel : WizardStepViewModel
	{
		private readonly IServiceLocator serviceLocator;
		private Experiment experiment;
		private string _selectedDataPath;
		private string[] _recentDataLocations;

		public SelectTemplateViewModel(Experiment experiment, IServiceLocator serviceLocator)
		{
			this.experiment = experiment;
			this.serviceLocator = serviceLocator;
			_selectedDataPath = Properties.Settings.Default.LastBrowseTemplateDataPath;
			LoadRecentLocations();
		}

		public override Type ViewType
		{
			get { return typeof(SelectTemplateView); }
		}

		public override string Title
		{
			get { return "Select a project as a template"; }
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

				Properties.Settings.Default.LastBrowseTemplateDataPath = _selectedDataPath;
				Properties.Settings.Default.RecentBrowseTemplateDataPaths = RecentListHelper.AddToRecentList(Properties.Settings.Default.RecentBrowseTemplateDataPaths, _selectedDataPath);
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

		public override bool OnNext()
		{
			if (File.Exists(_selectedDataPath))
			{
				Experiment templateExperiment = Experiment.Open(_selectedDataPath, null, serviceLocator);

				experiment.DataProvider = templateExperiment.DataProvider;
				experiment.DataProviderType = templateExperiment.DataProviderType;

				foreach (ProteinState proteinState in templateExperiment.ProteinStates)
				{
					experiment.ProteinStates.Add(proteinState);
				}

				foreach (Labeling labeling in templateExperiment.Labeling)
				{
					experiment.Labeling.Add(labeling);
					labeling.Experiment = experiment;
				}

				foreach (Run run in templateExperiment.Runs)
				{
					experiment.Runs.Add(run);
					run.FileName = Path.Combine(Path.GetDirectoryName(_selectedDataPath), run.FileName);
					run.Experiment = experiment;
				}

				foreach (Peptide peptide in templateExperiment.Peptides.PeptideCollection)
				{
					experiment.Peptides.PeptideCollection.Add(peptide);
				}

				return true;
			}
			return false;
		}

		private void LoadRecentLocations()
		{
			RecentDataLocations = Properties.Settings.Default.RecentBrowseTemplateDataPaths.Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries);
		}
	}
}
