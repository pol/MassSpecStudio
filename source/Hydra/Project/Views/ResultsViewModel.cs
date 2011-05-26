using System.ComponentModel;
using Hydra.Core.Domain;
using MassSpecStudio.Core;
using Microsoft.Practices.Prism.Commands;

namespace Hydra.Modules.Project.Views
{
	[Browsable(false)]
	public class ResultsViewModel : TreeViewItemBase<ExperimentViewModel, ResultViewModel>
	{
		private Experiment _experiment;

		public ResultsViewModel(Experiment experiment)
		{
			_experiment = experiment;
			Data = _experiment.Results;

			foreach (Result result in _experiment.Results)
			{
				ResultViewModel resultViewModel = new ResultViewModel(result);
				resultViewModel.Parent = this;
				Children.Add(resultViewModel);
			}

			Remove = new DelegateCommand<ResultViewModel>(OnRemove);
		}

		[Browsable(false)]
		public override string Name
		{
			get
			{
				return "Results";
			}

			set
			{
			}
		}

		[Browsable(false)]
		public DelegateCommand<ResultViewModel> Remove { get; set; }

		private void OnRemove(ResultViewModel value)
		{
			_experiment.Results.Remove(value.Data as Result);
			Children.Remove(value);
			((Result)value.Data).Delete();
		}
	}
}
