using System;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using AvalonDock;
using MassSpecStudio.Composition.Adapter;
using MassSpecStudio.Composition.Regions;
using MassSpecStudio.Core;
using MassSpecStudio.Modules.ExtensionManager;
using MassSpecStudio.Modules.Project;
using MassSpecStudio.Modules.Properties;
using MassSpecStudio.Modules.StatusBar;
using Microsoft.Practices.Prism.MefExtensions;
using Microsoft.Practices.Prism.Regions;

namespace MassSpecStudio
{
	public class Bootstrapper : MefBootstrapper
	{
		public DirectoryCatalog ModulesCatalog { get; set; }

		protected override DependencyObject CreateShell()
		{
			return Container.GetExportedValue<Shell>();
		}

		protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
		{
			var regionAdapterMappings = Container.GetExportedValueOrDefault<RegionAdapterMappings>();
			if (regionAdapterMappings != null)
			{
				regionAdapterMappings.RegisterMapping(typeof(DockablePane), this.Container.GetExportedValue<DockablePaneRegionAdapter>());
				regionAdapterMappings.RegisterMapping(typeof(DocumentPane), this.Container.GetExportedValue<DocumentPaneRegionAdapter>());
				regionAdapterMappings.RegisterMapping(typeof(Menu), this.Container.GetExportedValue<MenuRegionAdapter>());
			}

			return base.ConfigureRegionAdapterMappings();
		}

		protected override void InitializeShell()
		{
			base.InitializeShell();

			CreateProjectsDirectoryIfNecessary();

			App.Current.MainWindow = (Window)this.Shell;
			App.Current.MainWindow.Show();
		}

		protected override void InitializeModules()
		{
			base.InitializeModules();

			((Shell)App.Current.MainWindow).LoadLayout();
		}

		protected override void ConfigureAggregateCatalog()
		{
			base.ConfigureAggregateCatalog();

			this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(Bootstrapper).Assembly));
			this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(DockablePaneRegionAdapter).Assembly));

			this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(StatusModule).Assembly));
			this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(PropertiesModule).Assembly));
			this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(ExtensionManagerModule).Assembly));
			this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(ProjectModule).Assembly));
			this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(DocumentManager).Assembly));
			this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(OutputModule).Assembly));

			string executingDirectory = Path.GetDirectoryName(Assembly.GetAssembly(typeof(Bootstrapper)).Location);
			if (!Directory.Exists(Path.Combine(executingDirectory, "DataProviders")))
			{
				Directory.CreateDirectory(Path.Combine(executingDirectory, "DataProviders"));
			}
			ModulesCatalog = new DirectoryCatalog(Path.Combine(executingDirectory, "DataProviders"));
			this.AggregateCatalog.Catalogs.Add(ModulesCatalog);

			if (!Directory.Exists(Path.Combine(executingDirectory, "Modules")))
			{
				Directory.CreateDirectory(Path.Combine(executingDirectory, "Modules"));
			}
			ModulesCatalog = new DirectoryCatalog(Path.Combine(executingDirectory, "Modules"));
			this.AggregateCatalog.Catalogs.Add(ModulesCatalog);

			if (!Directory.Exists(Path.Combine(executingDirectory, "Processing")))
			{
				Directory.CreateDirectory(Path.Combine(executingDirectory, "Processing"));
			}
			ModulesCatalog = new DirectoryCatalog(Path.Combine(executingDirectory, "Processing"));
			this.AggregateCatalog.Catalogs.Add(ModulesCatalog);

			if (!Directory.Exists(Path.Combine(executingDirectory, "Packages")))
			{
				Directory.CreateDirectory(Path.Combine(executingDirectory, "Packages"));
			}
			ModulesCatalog = new DirectoryCatalog(Path.Combine(executingDirectory, "Packages"));
			this.AggregateCatalog.Catalogs.Add(ModulesCatalog);
		}

		private void CreateProjectsDirectoryIfNecessary()
		{
			string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			string projectsDirectory = Path.Combine(documents, "Mass Spec Studio Projects");

			if (!Directory.Exists(projectsDirectory))
			{
				Directory.CreateDirectory(projectsDirectory);
			}
		}
	}
}
