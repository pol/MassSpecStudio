using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;

namespace MassSpecStudio.Processing
{
	[ModuleExport(typeof(ProcessingModule))]
	[ExportMetadata("Title", "Mass Spec Studio Processing Algorithms")]
	[ExportMetadata("Type", "Processing")]
	[ExportMetadata("Author", "Mass Spec Studio")]
	[ExportMetadata("Description", "This module provides views of tic, xic and spectra.")]
	public class ProcessingModule : IModule
	{
		[ImportingConstructor]
		public ProcessingModule()
		{
		}

		public void Initialize()
		{
		}
	}
}
