using System;
using System.Linq;
using Hydra.Modules.Project.Views;
using Microsoft.Practices.Prism.Commands;

namespace Hydra.Modules.Project.WizardViews
{
	public class ProteinStatesViewModel : WizardStepViewModel
	{
		private readonly ExperimentViewModel _experimentViewModel;
		private ProteinStateViewModel _selectedItem;

		public ProteinStatesViewModel(ExperimentViewModel experimentViewModel)
		{
			_experimentViewModel = experimentViewModel;
			ViewModel = experimentViewModel.Samples;
			Add = new DelegateCommand<string>(OnAdd);
			Remove = new DelegateCommand<ProteinStateViewModel>(OnRemove, CanRemove);
		}

		public override string Title
		{
			get { return "Add Protein States"; }
		}

		public SamplesViewModel ViewModel
		{
			get;
			private set;
		}

		public ProteinStateViewModel SelectedItem
		{
			get
			{
				return _selectedItem;
			}

			set
			{
				_selectedItem = value;
				NotifyPropertyChanged(() => SelectedItem);
				Remove.RaiseCanExecuteChanged();
			}
		}

		public DelegateCommand<string> Add { get; set; }

		public DelegateCommand<ProteinStateViewModel> Remove { get; set; }

		public override Type ViewType
		{
			get { return typeof(ProteinStatesView); }
		}

		public override bool CanNext
		{
			get { return ViewModel.Children.Count > 0; }
		}

		public override bool CanBack
		{
			get { return false; }
		}

		private void OnAdd(string value)
		{
			ViewModel.AddProteinState.Execute(null);
			SelectedItem = ViewModel.Children.Last();
			NotifyPropertyChanged(() => CanNext);
		}

		private void OnRemove(ProteinStateViewModel value)
		{
			ViewModel.RemoveProteinState.Execute(value);
			SelectedItem = ViewModel.Children.LastOrDefault();
			NotifyPropertyChanged(() => CanNext);
		}

		private bool CanRemove(ProteinStateViewModel value)
		{
			return _selectedItem != null;
		}
	}
}
