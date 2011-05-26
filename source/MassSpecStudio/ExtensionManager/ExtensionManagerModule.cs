using System.ComponentModel.Composition;
using System.Windows;
using MassSpecStudio.Core.Events;
using MassSpecStudio.Modules.ExtensionManager.Views;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;

namespace MassSpecStudio.Modules.ExtensionManager
{
	[ModuleExport(typeof(IModule))]
	[ExportMetadata("Title", "Extension Manager")]
	[ExportMetadata("Type", "Module")]
	[ExportMetadata("Author", "Mass Spec Studio")]
	[ExportMetadata("Description", "This module alloows viewing and management of installed modules within Mass Spec Studio.")]
	public class ExtensionManagerModule : IModule
	{
		[ImportingConstructor]
		public ExtensionManagerModule(IEventAggregator eventAggregator)
		{
			eventAggregator.GetEvent<OpenExtensionManagerEvent>().Subscribe(ShowExtensionManager);
		}

		public void ShowExtensionManager(string value)
		{
			ExtensionManagerView dialog = new ExtensionManagerView(new ExtensionManagerViewModel());
			dialog.Owner = Application.Current.MainWindow;
			dialog.Show();
		}

		public void Initialize()
		{
		}
	}
}
