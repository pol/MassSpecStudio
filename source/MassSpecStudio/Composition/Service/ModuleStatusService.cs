using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Unity;

namespace MSStudio.Composition.Service
{
	public class ModuleStatusService : IModuleStatusService
	{
		IUnityContainer _container;
		private List<Type> _loadedModules;

		/// <summary>
		/// Initializes a new instance of the ModuleStatusService class.
		/// </summary>
		[InjectionConstructor]
		public ModuleStatusService(IUnityContainer container)
		{
			_loadedModules = new List<Type>();
			_container = container;
		}

		/// <summary>
		/// Registers a loaded module type. This is used by the module tracking
		/// unity extension, but can also be used by any other IoC container to register 
		/// loaded module types
		/// </summary>
		/// <param name="moduleType">Type information of the module that is loaded</param>
		public void RegisterLoadedModule(Type moduleType)
		{
			if (moduleType.GetInterface(typeof(IModule).FullName) == null)
				throw new ArgumentException("Tried to register a non-module type for status tracking");

			if (_loadedModules.Contains(moduleType))
				return;

			_loadedModules.Add(moduleType);
		}

		/// <summary>
		/// Retrieves the status information of all available modules in the application
		/// </summary>
		/// <returns>Returns a collection containing all modules and their status</returns>
		public ReadOnlyCollection<ModuleStatusInfo> GetModules()
		{
			List<ModuleStatusInfo> modules = new List<ModuleStatusInfo>();
			IModuleEnumerator moduleEnumerator = _container.Resolve<IModuleEnumerator>();

			foreach (ModuleInfo module in moduleEnumerator.GetModules())
			{
				////Assembly assembly = Assembly.LoadFrom(module.AssemblyFile);
				////Type type = assembly.GetType(module.ModuleType);

				////if (_loadedModules.Contains(type))
				////{
				////    modules.Add(new ModuleStatusInfo(module.ModuleName, ModuleStatus.Loaded));
				////}
				////else
				////{
				////    modules.Add(new ModuleStatusInfo(module.ModuleName, ModuleStatus.Unloaded));
				////}
			}

			return new ReadOnlyCollection<ModuleStatusInfo>(modules);
		}
	}
}