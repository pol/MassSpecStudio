using System;
using System.ComponentModel.Composition;
using MassSpecStudio.Core;
using Microsoft.Practices.ServiceLocation;

namespace Hydra.Modules.Project
{
	[Export(typeof(IExperimentType))]
	public class TemplatedHydraExperimentType : HydraExperimentType
	{
		[ImportingConstructor]
		public TemplatedHydraExperimentType(IServiceLocator serviceLocator)
			: base(serviceLocator)
		{
		}

		public override string Icon
		{
			get { return "/MassSpecStudio;component/Images/copy-item.png"; }
		}

		public override string Name
		{
			get { return "TemplatedHydra"; }
			set { throw new NotImplementedException(); }
		}

		public override string Description
		{
			get { return "Used to investigate HDX data starting from an existing project as a template."; }
			set { throw new NotImplementedException(); }
		}

		public override Guid ExperimentType
		{
			get
			{
				return new Guid("a1439744-7b81-4275-bd66-94d51b3abf8c");
			}
		}

		public override IExperimentType ExperimentTypeObject
		{
			get
			{
				return this;
			}
		}
	}
}
