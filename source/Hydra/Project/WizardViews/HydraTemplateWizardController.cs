using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hydra.Core.Domain;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;

namespace Hydra.Modules.Project.WizardViews
{
	public class HydraTemplateWizardController : WizardViewModel
	{
		private readonly Experiment _experiment;
		private readonly IRegionManager _regionManager;
		private readonly IServiceLocator _serviceLocator;
		private readonly IList<WizardStepViewModel> _wizardViewModels;
		private int _currentViewModelIndex;

		public HydraTemplateWizardController(IRegionManager regionManager, IServiceLocator serviceLocator, Experiment experiment)
		{
			_regionManager = regionManager;
			_experiment = experiment;
			_serviceLocator = serviceLocator;
			_wizardViewModels = new List<WizardStepViewModel>();

			SelectTemplateViewModel templateViewModel = new SelectTemplateViewModel(experiment, serviceLocator);
			_wizardViewModels.Add(templateViewModel);
		}

		public WizardStepViewModel CurrentWizardViewModel
		{
			get { return _wizardViewModels[_currentViewModelIndex]; }
		}

		public override void InitializeView()
		{
			_currentViewModelIndex = 0;

			_regionManager.Display("WizardContentRegion", _wizardViewModels.First().ViewType, _serviceLocator);
		}

		public override void OnNext(string value)
		{
			if (CurrentWizardViewModel.OnNext())
			{
				if (_currentViewModelIndex + 1 < _wizardViewModels.Count)
				{
					_currentViewModelIndex++;
					NotifyPropertyChanged(() => CurrentWizardViewModel);
					ActivateView();
				}
				else
				{
					if (FinishedEvent != null)
					{
						FinishedEvent(this, null);
					}
				}
			}
		}

		public override void OnBack(string value)
		{
			if (CurrentWizardViewModel.OnBack())
			{
				if (_currentViewModelIndex > 0)
				{
					_currentViewModelIndex--;
					NotifyPropertyChanged(() => CurrentWizardViewModel);
					ActivateView();
				}
			}
		}

		public override void OnCancel(string value)
		{
		}

		private void ActivateView()
		{
			_regionManager.Display("WizardContentRegion", CurrentWizardViewModel.ViewType, _serviceLocator);
		}
	}
}
