using System;
using MassSpecStudio.Core;

namespace Hydra.Modules.Project.WizardViews
{
	public abstract class WizardStepViewModel : ViewModelBase
	{
		public WizardStepViewModel()
		{
		}

		public abstract string Title
		{
			get;
		}

		public virtual bool CanNext
		{
			get { return true; }
		}

		public virtual bool CanBack
		{
			get { return true; }
		}

		public virtual string NextButtonText
		{
			get { return "Next"; }
		}

		public abstract Type ViewType
		{
			get;
		}

		public virtual bool OnNext()
		{
			return true;
		}

		public virtual bool OnBack()
		{
			return true;
		}
	}
}
