using System.ComponentModel.Composition;
using MassSpecStudio.Core.Events;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.ViewModel;

namespace MassSpecStudio.Modules.Properties.Views
{
	[Export]
	public class PropertiesViewModel : NotificationObject
	{
		private object _objectInstance;

		[ImportingConstructor]
		public PropertiesViewModel(IEventAggregator eventAggregator)
		{
			eventAggregator.GetEvent<ObjectSelectionEvent>().Subscribe(OnSelectionChanged);
		}

		public object ObjectInstance
		{
			get
			{
				return _objectInstance;
			}

			set
			{
				_objectInstance = value;
				RaisePropertyChanged(() => ObjectInstance);
			}
		}

		public void OnSelectionChanged(object value)
		{
			ObjectInstance = value;
		}
	}
}
