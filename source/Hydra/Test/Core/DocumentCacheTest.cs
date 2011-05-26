using Hydra.Core;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Hydra.Test.Core
{
	[TestClass]
	public class DocumentCacheTest
	{
		private Mock<IServiceLocator> mockServiceLocator;

		[TestInitialize]
		public void TestInitialize()
		{
			mockServiceLocator = new Mock<IServiceLocator>();
		}

		[TestMethod]
		public void Properties()
		{
			DocumentCache.ProjectViewModel = "test";
			Assert.AreEqual("test", DocumentCache.ProjectViewModel);

			DocumentCache.Experiment = TestHelper.GetTestExperiment(mockServiceLocator);
			Assert.AreEqual("test", DocumentCache.Experiment.Name);
		}
	}
}
