using System.Collections.ObjectModel;
using System.ComponentModel;
using MassSpecStudio.Core;

namespace Hydra.Modules.Project.Views
{
	public class ExperimentViewModel : TreeViewItemBase<ProjectViewModel, SamplesViewModel>
	{
		private Core.Domain.Experiment _experiment;
		private ObservableCollection<object> _genericChildren;
		private SamplesViewModel _samples;
		private PeptidesViewModel _peptides;
		private ResultsViewModel _results;

		public ExperimentViewModel(Core.Domain.Experiment experiment)
		{
			_genericChildren = new ObservableCollection<object>();
			_experiment = experiment;
			Data = experiment;

			_samples = new SamplesViewModel(experiment);
			_peptides = new PeptidesViewModel(experiment);
			_results = new ResultsViewModel(experiment);
		}

		[Category("Common Information")]
		[ReadOnly(true)]
		public string Location
		{
			get { return _experiment.Location; }
		}

		[Browsable(false)]
		public new ObservableCollection<object> Children
		{
			get { return _genericChildren; }
			set { _genericChildren = value; }
		}

		[Browsable(false)]
		public SamplesViewModel Samples
		{
			get
			{
				return _samples;
			}

			set
			{
				_samples = value;
				Children.Add(_samples);
			}
		}

		[Browsable(false)]
		public PeptidesViewModel Peptides
		{
			get
			{
				return _peptides;
			}

			set
			{
				_peptides = value;
				Children.Add(_peptides);
			}
		}

		[Browsable(false)]
		public ResultsViewModel Results
		{
			get
			{
				return _results;
			}

			set
			{
				_results = value;
				Children.Add(_results);
			}
		}

		[Browsable(false)]
		public override string Name
		{
			get
			{
				return "Experiment";
			}

			set
			{
			}
		}
	}
}
