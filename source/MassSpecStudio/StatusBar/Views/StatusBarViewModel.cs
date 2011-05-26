using System.ComponentModel.Composition;
using MassSpecStudio.Core.Events;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.ViewModel;

namespace MassSpecStudio.Modules.StatusBar.Views
{
	[Export]
	public class StatusBarViewModel : NotificationObject
	{
		private string statusMessage;

		[ImportingConstructor]
		public StatusBarViewModel(IEventAggregator eventAggregator)
		{
			statusMessage = "Ready";
			eventAggregator.GetEvent<StatusUpdateEvent>().Subscribe(UpdateStatusMessage);
		}

		public string StatusMessage
		{
			get
			{
				return statusMessage;
			}

			private set
			{
				statusMessage = value;
				RaisePropertyChanged(() => StatusMessage);
			}
		}

		private void UpdateStatusMessage(string message)
		{
			StatusMessage = message;
		}
	}
}
