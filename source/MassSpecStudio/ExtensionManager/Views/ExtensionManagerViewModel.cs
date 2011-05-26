using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;

namespace MassSpecStudio.Modules.ExtensionManager.Views
{
	[Export]
	public class ExtensionManagerViewModel
	{
		private IList<ModuleDefinitionViewModel> _modules;
		private IList<ModuleDefinitionViewModel> _processing;
		private IList<ModuleDefinitionViewModel> _dataProviders;

		[ImportingConstructor]
		public ExtensionManagerViewModel()
		{
			_modules = new List<ModuleDefinitionViewModel>();
			_modules = GetModules(@"./", "*.Modules.*", "Module");
			_modules = Modules.Concat(GetModules(@"./Modules", "*.Modules.*", "Module")).ToList();

			_dataProviders = new List<ModuleDefinitionViewModel>();
			_dataProviders = GetModules(@"./DataProviders", "*", "Data Provider");

			_processing = new List<ModuleDefinitionViewModel>();
			_processing = GetModules(@"./Processing", "*", "Processing");
		}

		public IList<ModuleDefinitionViewModel> Modules
		{
			get { return _modules; }
		}

		public IList<ModuleDefinitionViewModel> DataProviders
		{
			get { return _dataProviders; }
		}

		public IList<ModuleDefinitionViewModel> Processing
		{
			get { return _processing; }
		}

		private IList<ModuleDefinitionViewModel> GetModules(string path, string filter, string type)
		{
			DirectoryCatalog catalog = new DirectoryCatalog(path, filter);
			IList<ComposablePartDefinition> parts = catalog.Parts.ToList();

			return (from p in parts
					where p.ExportDefinitions.First().Metadata.Keys.Contains("Type") && p.ExportDefinitions.First().Metadata["Type"].ToString() == type
					select new ModuleDefinitionViewModel()
					{
						Title = p.ExportDefinitions.First().Metadata["Title"].ToString(),
						Description = p.ExportDefinitions.First().Metadata["Description"].ToString(),
						ModuleType = p.ExportDefinitions.First().Metadata["Type"].ToString(),
						Author = p.ExportDefinitions.First().Metadata["Author"].ToString(),
					}).ToList();
		}
	}
}
