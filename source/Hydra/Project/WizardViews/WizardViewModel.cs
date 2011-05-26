using System;
using MassSpecStudio.Core;
using Microsoft.Practices.Prism.Commands;

namespace Hydra.Modules.Project.WizardViews
{
	public abstract class WizardViewModel : ViewModelBase
	{
		public WizardViewModel()
		{
			NextCommand = new DelegateCommand<string>(OnNext, CanNext);
			BackCommand = new DelegateCommand<string>(OnBack, CanBack);
			CancelCommand = new DelegateCommand<string>(OnCancel, CanCancel);
		}

		public EventHandler FinishedEvent { get; set; }

		public DelegateCommand<string> NextCommand { get; set; }

		public DelegateCommand<string> BackCommand { get; set; }

		public DelegateCommand<string> CancelCommand { get; set; }

		public abstract void OnNext(string value);

		public virtual bool CanNext(string value)
		{
			return true;
		}

		public abstract void OnBack(string value);

		public virtual bool CanBack(string value)
		{
			return true;
		}

		public abstract void OnCancel(string value);

		public virtual bool CanCancel(string value)
		{
			return true;
		}

		public abstract void InitializeView();
	}
}
