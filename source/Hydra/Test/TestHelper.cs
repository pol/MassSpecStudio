using Hydra.Core.Domain;
using Hydra.Modules.Project;
using MassSpecStudio.Core.Domain;
using Microsoft.Practices.ServiceLocation;
using Moq;

namespace Hydra.Test
{
	public static class TestHelper
	{
		public static Experiment GetTestExperiment(Mock<IServiceLocator> mockServiceLocator)
		{
			return new Experiment("test", new ProjectBase("test", @"c:\temp"), new HydraExperimentType(mockServiceLocator.Object));
		}
	}
}
