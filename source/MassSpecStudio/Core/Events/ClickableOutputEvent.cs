using System.Windows.Input;
using Microsoft.Practices.Prism.Events;

namespace MassSpecStudio.Core.Events
{
	public class ClickableOutputEvent : CompositePresentationEvent<ClickableOutputEvent>
	{
		public string Text { get; set; }

		public ICommand Click { get; set; }

		public object Parameter { get; set; }
	}
}
