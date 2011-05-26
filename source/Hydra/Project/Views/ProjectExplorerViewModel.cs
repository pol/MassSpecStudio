using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using Hydra.Core.Domain;
using Hydra.Core.Events;
using MassSpecStudio.Core;
using MassSpecStudio.Core.Events;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

namespace Hydra.Modules.Project.Views
{
	[Export]
	public class ProjectExplorerViewModel : ViewModelBase
	{
		private readonly IEventAggregator _eventAggregator;
		private readonly IRegionManager _regionManager;
		private readonly ObservableCollection<ProjectViewModel> _project;

		[ImportingConstructor]
		public ProjectExplorerViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
		{
			_eventAggregator = eventAggregator;
			_regionManager = regionManager;
			_project = new ObservableCollection<ProjectViewModel>();
			eventAggregator.GetEvent<LoadProjectDataEvent>().Subscribe(OnLoadProject);
			eventAggregator.GetEvent<ResultAddedEvent>().Subscribe(OnResultAdded, ThreadOption.UIThread);
		}

		public ObservableCollection<ProjectViewModel> Project
		{
			get { return _project; }
		}

		public ITreeViewItem SelectedItem { get; set; }

		public void OnSelectionChanged()
		{
			_eventAggregator.GetEvent<ObjectSelectionEvent>().Publish(SelectedItem);
		}

		private void OnResultAdded(Result result)
		{
			ResultViewModel resultViewModel = new ResultViewModel(result);
			result.Save();

			// TODO: Only supports one experiment
			ResultsViewModel resultsViewModel = (ResultsViewModel)Project[0].Children[0].Children.Where(item => item is ResultsViewModel).FirstOrDefault();

			if (resultsViewModel != null)
			{
				resultsViewModel.Children.Add(resultViewModel);
			}
		}

		private void OnLoadProject(object project)
		{
			Project.Clear();
			Project.Add(project as ProjectViewModel);
		}
	}
}
