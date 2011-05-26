using System.ComponentModel;
using System.IO;
using Hydra.Core.Domain;
using MassSpecStudio.Core;
using Microsoft.Practices.Prism.Commands;

namespace Hydra.Modules.Project.Views
{
	[DefaultProperty("Name")]
	public class RunViewModel : TreeViewItemBase<LabelingViewModel, RunViewModel>
	{
		private Run _run;

		public RunViewModel(Run run)
		{
			_run = run;
			Data = run;
			Remove = new DelegateCommand<RunViewModel>(OnRemove);
		}

		[Browsable(false)]
		public DelegateCommand<RunViewModel> Remove { get; set; }

		[Browsable(false)]
		public override string Name
		{
			get
			{
				return Path.GetFileName(_run.FileName);
			}

			set
			{
				// Do Nothing;
			}
		}

		[Category("Common Properties")]
		[ReadOnly(true)]
		public string Location
		{
			get { return _run.FileName; }
		}

		public void OnRemove(RunViewModel value)
		{
			((Run)value.Data).Experiment.Runs.Remove(((Run)value.Data));
			value.Parent.Children.Remove(value);
		}
	}
}
