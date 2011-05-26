using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Hydra.Core.Domain;
using MassSpecStudio.Core;

namespace Hydra.Modules.Project.Views
{
	[DefaultProperty("Name")]
	public class ProteinStateViewModel : TreeViewItemBase<SamplesViewModel, LabelingViewModel>
	{
		private ProteinState _proteinState;

		public ProteinStateViewModel(ProteinState proteinState, Experiment rootExperiment)
		{
			_proteinState = proteinState;
			Data = proteinState;

			foreach (Labeling labeling in rootExperiment.Labeling)
			{
				Children.Add(new LabelingViewModel(labeling, proteinState, rootExperiment));
				Children.Last().Parent = this;
			}
		}

		[Category("Common Properties")]
		[Browsable(true)]
		[Required]
		public override string Name
		{
			get
			{
				return _proteinState.Name;
			}

			set
			{
				_proteinState.Name = value;
				NotifyPropertyChanged(() => Name);
			}
		}

		[Browsable(false)]
		internal ProteinState ProteinState
		{
			get { return _proteinState; }
		}
	}
}
