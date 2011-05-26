using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;

namespace DeconTools.MassSpecStudio.Processing
{
	[ModuleExport(typeof(DeconToolsModule))]
	[ExportMetadata("Title", "DeconTools Processing Algorithms")]
	[ExportMetadata("Type", "Processing")]
	[ExportMetadata("Author", "Mass Spec Studio")]
	[ExportMetadata("Description", "This module provides general processing steps for use in algorithms (eg. smoothing and peak detections).")]
	public class DeconToolsModule : IModule
	{
		[ImportingConstructor]
		public DeconToolsModule()
		{
		}

		public void Initialize()
		{
		}
	}
}
