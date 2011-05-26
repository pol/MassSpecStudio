using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Hydra.Core.Domain;
using MassSpecStudio.Core;
using Microsoft.Practices.Prism.Commands;

namespace Hydra.Modules.Project.Views
{
	[Browsable(false)]
	public class SamplesViewModel : TreeViewItemBase<ExperimentViewModel, ProteinStateViewModel>
	{
		private Core.Domain.Experiment _rootExperiment;

		public SamplesViewModel(Core.Domain.Experiment rootExperiment)
		{
			_rootExperiment = rootExperiment;
			AddProteinState = new DelegateCommand<object>(OnAddProteinState);
			RemoveProteinState = new DelegateCommand<ProteinStateViewModel>(OnRemoveProteinState);
			AddLabeling = new DelegateCommand<object>(OnAddLabeling);
			RemoveLabeling = new DelegateCommand<LabelingViewModel>(OnRemoveLabeling);

			foreach (ProteinState proteinState in rootExperiment.ProteinStates)
			{
				Children.Add(new ProteinStateViewModel(proteinState, rootExperiment));
				Children.Last().Parent = this;
			}
		}

		[Browsable(false)]
		public DelegateCommand<object> AddProteinState { get; set; }

		[Browsable(false)]
		public DelegateCommand<ProteinStateViewModel> RemoveProteinState { get; set; }

		[Browsable(false)]
		public DelegateCommand<object> AddLabeling { get; set; }

		[Browsable(false)]
		public DelegateCommand<LabelingViewModel> RemoveLabeling { get; set; }

		[Browsable(false)]
		public override string Name
		{
			get
			{
				return _rootExperiment.Name;
			}

			set
			{
				_rootExperiment.Name = value;
				NotifyPropertyChanged(() => Name);
			}
		}

		internal Core.Domain.Experiment Experiment
		{
			get { return _rootExperiment; }
		}

		public void OnAddProteinState(object value)
		{
			ProteinState proteinState = new ProteinState(_rootExperiment);
			ProteinStateViewModel proteinStateViewModel = new ProteinStateViewModel(proteinState, _rootExperiment);

			Children.Add(proteinStateViewModel);
			proteinStateViewModel.Parent = this;
			proteinStateViewModel.IsExpanded = true;
			NotifyPropertyChanged(() => RemoveProteinState);
		}

		public void OnRemoveProteinState(ProteinStateViewModel value)
		{
			if (value != null)
			{
				_rootExperiment.ProteinStates.Remove(((ProteinState)value.Data));

				// Remove dependant runs.
				IList<Run> runs = _rootExperiment.Runs.Where(item => item.ProteinState == ((ProteinState)value.Data)).ToList();

				foreach (Run run in runs)
				{
					_rootExperiment.Runs.Remove(run);
				}

				Children.Remove(value);
				NotifyPropertyChanged(() => RemoveProteinState);
			}
		}

		public void OnAddLabeling(object value)
		{
			Labeling labeling = new Labeling(_rootExperiment);

			foreach (ProteinStateViewModel proteinStateViewModel in Children)
			{
				LabelingViewModel labelingViewModel = new LabelingViewModel(labeling, proteinStateViewModel.ProteinState, _rootExperiment);
				proteinStateViewModel.Children.Add(labelingViewModel);
				labelingViewModel.Parent = proteinStateViewModel;
				proteinStateViewModel.IsExpanded = true;
			}
		}

		public void OnRemoveLabeling(LabelingViewModel value)
		{
			if (value != null)
			{
				_rootExperiment.Labeling.Remove(((Labeling)value.Data));

				// Remove dependant runs.
				IList<Run> runs = _rootExperiment.Runs.Where(item => item.Labeling == ((Labeling)value.Data)).ToList();
				foreach (Run run in runs)
				{
					_rootExperiment.Runs.Remove(run);
				}

				// Remove labeling from each protein state.
				foreach (ProteinStateViewModel proteinStateViewModel in Children)
				{
					LabelingViewModel labelingViewModel = proteinStateViewModel.Children.Where(item => item.Name == value.Name).FirstOrDefault();
					if (labelingViewModel != null)
					{
						proteinStateViewModel.Children.Remove(labelingViewModel);
					}
				}
			}
		}
	}
}
