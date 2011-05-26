using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using MassSpecStudio.Core;
using MassSpecStudio.Core.EventArg;

namespace MassSpecStudio.Modules.Project.Views
{
	[Export(typeof(StartPageViewModel))]
	public class StartPageViewModel : ViewModelBase
	{
		[ImportingConstructor]
		public StartPageViewModel(IDocumentManager documentManager)
		{
			RecentProjects = Core.Properties.Settings.Default.RecentProjects.Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries);
			DocumentManager = documentManager;
		}

		public EventHandler<MessageEventArgs> ErrorMessageEvent { get; set; }

		public IList<string> RecentProjects { get; private set; }

		public IDocumentManager DocumentManager { get; private set; }

		public void Open(string path)
		{
			try
			{
				DocumentManager.Open(path);
			}
			catch (Exception ex)
			{
				if (ErrorMessageEvent != null)
				{
					ErrorMessageEvent(this, new MessageEventArgs(ex.Message));
				}
			}
		}
	}
}
