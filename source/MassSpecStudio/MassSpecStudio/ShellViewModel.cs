using System.Collections.Generic;
using System.ComponentModel.Composition;
using MassSpecStudio.Core;
using MassSpecStudio.Core.Events;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;

namespace MassSpecStudio
{
	[Export]
	public class ShellViewModel : ViewModelBase
	{
		private readonly IEventAggregator _eventAggregator;
		private readonly IDocumentManager _documentManager;

		[ImportingConstructor]
		public ShellViewModel(IEventAggregator eventAggregator, IDocumentManager documentManager)
		{
			_eventAggregator = eventAggregator;
			_documentManager = documentManager;
			RecentProjects = Core.Properties.Settings.Default.RecentProjects.Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries);

			Process = new DelegateCommand<string>(OnProcess, CanProcess);

			_eventAggregator.GetEvent<ProjectClosedEvent>().Subscribe(OnProjectClosed);
			_eventAggregator.GetEvent<LoadProjectEvent>().Subscribe(OnProjectOpened);
		}

		public IList<string> RecentProjects { get; set; }

		public DelegateCommand<string> Process { get; set; }

		private void OnProcess(string value)
		{
			_eventAggregator.GetEvent<ProcessingDialogEvent>().Publish(null);
		}

		private bool CanProcess(string value)
		{
			return _documentManager.IsProjectOpen;
		}

		private void OnProjectClosed(object value)
		{
			Process.RaiseCanExecuteChanged();
		}

		private void OnProjectOpened(object value)
		{
			Process.RaiseCanExecuteChanged();
		}
	}
}
