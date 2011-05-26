using System;
using Hydra.Modules.Project;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Hydra.Test.Modules.Project
{
	[TestClass]
	public class TemplatedHydraExperimentTypeTest
	{
		private TemplatedHydraExperimentType templatedHydraExperimentType;
		private Mock<IServiceLocator> mockServiceLocator;

		[TestInitialize]
		public void ClassInitialize()
		{
			mockServiceLocator = new Mock<IServiceLocator>();
			templatedHydraExperimentType = new TemplatedHydraExperimentType(mockServiceLocator.Object);
		}

		[TestMethod]
		public void Parameters()
		{
			Assert.AreEqual("/MassSpecStudio;component/Images/copy-item.png", templatedHydraExperimentType.Icon);
			Assert.AreEqual("TemplatedHydra", templatedHydraExperimentType.Name);
			Assert.AreEqual("Used to investigate HDX data starting from an existing project as a template.", templatedHydraExperimentType.Description);
			Assert.AreEqual(new Guid("a1439744-7b81-4275-bd66-94d51b3abf8c"), templatedHydraExperimentType.ExperimentType);
			Assert.AreEqual(templatedHydraExperimentType, templatedHydraExperimentType.ExperimentTypeObject);
		}
	}
}
