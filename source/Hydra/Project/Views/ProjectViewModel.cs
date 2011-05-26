using System.ComponentModel;
using Hydra.Core.Domain;
using MassSpecStudio.Core;
using MassSpecStudio.Core.Domain;

namespace Hydra.Modules.Project.Views
{
	[DefaultProperty("Name")]
	public class ProjectViewModel : TreeViewItemBase<ProjectViewModel, ExperimentViewModel>
	{
		private ProjectBase _rootProject;

		public ProjectViewModel(ProjectBase rootProject)
		{
			_rootProject = rootProject;
			Data = rootProject;

			foreach (ExperimentBase experimentBase in rootProject.Experiments)
			{
				Experiment experiment = experimentBase as Experiment;
				ExperimentViewModel experimentViewModel = new ExperimentViewModel(experiment);
				SamplesViewModel sampleViewModel = new SamplesViewModel(experiment);
				experimentViewModel.Samples = sampleViewModel;

				PeptidesViewModel peptidesViewModel = new PeptidesViewModel(experiment);
				experimentViewModel.Peptides = peptidesViewModel;

				ResultsViewModel resultsViewModel = new ResultsViewModel(experiment);
				experimentViewModel.Results = resultsViewModel;

				Children.Add(experimentViewModel);

				IsExpanded = true;
				experimentViewModel.IsExpanded = true;
			}
		}

		[Category("Common Information")]
		[ReadOnly(true)]
		public override string Name
		{
			get
			{
				return _rootProject.Name;
			}

			set
			{
				_rootProject.Name = value;
				NotifyPropertyChanged(() => Name);
			}
		}

		[Category("Common Information")]
		[ReadOnly(true)]
		public string Location
		{
			get { return _rootProject.Location; }
		}

		internal ProjectBase Project
		{
			get { return _rootProject; }
		}
	}
}
