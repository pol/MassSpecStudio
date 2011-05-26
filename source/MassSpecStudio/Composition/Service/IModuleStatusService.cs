using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Microsoft.Practices.Composite.Modularity;

namespace MSStudio.Composition.Service
{
	/// <summary>
	/// Contract of the module status service
	/// </summary>
	public interface IModuleStatusService
	{
		/// <summary>
		/// Registers a loaded module type. This is used by the module tracking
		/// unity extension, but can also be used by any other IoC container to register 
		/// loaded module types
		/// </summary>
		/// <param name="moduleType">Type information of the module that is loaded</param>
		void RegisterLoadedModule(Type moduleType);
		/// <summary>
		/// Retrieves the status information of all available modules in the application
		/// </summary>
		/// <returns>Returns a collection containing all modules and their status</returns>
		ReadOnlyCollection<ModuleStatusInfo> GetModules();
	}

	public interface IModuleEnumerator
	{
		ModuleInfo GetModule(string moduleName);
		ModuleInfo[] GetModules();
		ModuleInfo[] GetStartupLoadedModules();
	}
}