using System;
using System.Linq;
using Hydra.Modules.Project.Views;
using Microsoft.Practices.Prism.Commands;

namespace Hydra.Modules.Project.WizardViews
{
	public class LabellingsViewModel : WizardStepViewModel
	{
		private readonly ExperimentViewModel _experimentViewModel;
		private LabelingViewModel _selectedItem;

		public LabellingsViewModel(ExperimentViewModel experimentViewModel)
		{
			_experimentViewModel = experimentViewModel;
			ViewModel = experimentViewModel.Samples;
			Add = new DelegateCommand<string>(OnAdd);
			Remove = new DelegateCommand<LabelingViewModel>(OnRemove, CanRemove);
		}

		public override string Title
		{
			get { return "Add Labelings"; }
		}

		public SamplesViewModel ViewModel
		{
			get;
			private set;
		}

		public ProteinStateViewModel FirstProteinState
		{
			get { return ViewModel.Children.First(); }
		}

		public LabelingViewModel SelectedItem
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

		public DelegateCommand<LabelingViewModel> Remove { get; set; }

		public override Type ViewType
		{
			get { return typeof(LabellingsView); }
		}

		public override bool CanNext
		{
			get { return FirstProteinState.Children.Count > 0; }
		}

		public override bool OnBack()
		{
			SelectedItem = null;
			return base.OnBack();
		}

		private void OnAdd(string value)
		{
			ViewModel.AddLabeling.Execute(null);
			SelectedItem = FirstProteinState.Children.Last();
			NotifyPropertyChanged(() => CanNext);
		}

		private void OnRemove(LabelingViewModel value)
		{
			ViewModel.RemoveLabeling.Execute(value);
			SelectedItem = FirstProteinState.Children.LastOrDefault();
			NotifyPropertyChanged(() => CanNext);
		}

		private bool CanRemove(LabelingViewModel value)
		{
			return _selectedItem != null;
		}
	}
}
