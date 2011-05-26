using System;
using System.Collections.Generic;
using System.Linq;
using Hydra.Core.Domain;
using Hydra.Modules.Project.Views;
using MassSpecStudio.Core;
using Microsoft.Practices.ServiceLocation;

namespace Hydra.Modules.Project.WizardViews
{
	public class DataProviderViewModel : WizardStepViewModel
	{
		private readonly ExperimentViewModel _experimentViewModel;
		private readonly IServiceLocator serviceLocator;
		private IList<IDataProvider> dataProviders;
		private IDataProvider selectedDataProvider;

		public DataProviderViewModel(ExperimentViewModel experimentViewModel, IServiceLocator serviceLocator)
		{
			this.serviceLocator = serviceLocator;
			_experimentViewModel = experimentViewModel;
			ViewModel = experimentViewModel;
			dataProviders = serviceLocator.GetAllInstances<IDataProvider>().ToList();
			selectedDataProvider = dataProviders.First();
		}

		public IList<IDataProvider> DataProviders
		{
			get { return dataProviders; }
		}

		public override Type ViewType
		{
			get
			{
				return typeof(DataProviderView);
			}
		}

		public IDataProvider SelectedDataProvider
		{
			get
			{
				return selectedDataProvider;
			}

			set
			{
				selectedDataProvider = value;
				NotifyPropertyChanged(() => SelectedDataProvider);
			}
		}

		public override string Title
		{
			get { return "Select Data Provider"; }
		}

		public ExperimentViewModel ViewModel
		{
			get;
			private set;
		}

		public override bool OnNext()
		{
			((Experiment)ViewModel.Data).DataProvider = SelectedDataProvider;
			return true;
		}
	}
}
