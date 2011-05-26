using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MassSpecStudio.Core.EventArg
{
	public class MessageEventArgs : EventArgs
	{
		public MessageEventArgs(string message)
		{
			Message = message;
		}

		public string Message { get; private set; }
	}
}
