using System.IO;
using Hydra.Core.Domain;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ProteoWizard.MassSpecStudio.DataProvider;

namespace Hydra.Test.Core.Domain
{
	[TestClass]
	public class RunTest
	{
		private Mock<IServiceLocator> mockServiceLocator;
		private Mock<IEventAggregator> mockEventAggregator;
		private Run run;
		private Experiment experiment;

		[TestInitialize]
		public void TestInitialize()
		{
			mockServiceLocator = new Mock<IServiceLocator>();
			mockEventAggregator = new Mock<IEventAggregator>();
			experiment = TestHelper.GetTestExperiment(mockServiceLocator);
			run = new Run(Path.GetFileName(Properties.Settings.Default.mzXMLTestFile1), Properties.Settings.Default.mzXMLTestFile1, new ProteinState(experiment), new Labeling(experiment), experiment, new ProteoWizardDataProvider(mockEventAggregator.Object));
		}

		[TestMethod]
		public void Constructor()
		{
			Assert.IsNotNull(run.ProteinState);
			Assert.IsNotNull(run.Labeling);
			Assert.AreEqual(experiment, run.Experiment);
			Assert.AreEqual(1, experiment.Runs.Count);
		}
	}
}
