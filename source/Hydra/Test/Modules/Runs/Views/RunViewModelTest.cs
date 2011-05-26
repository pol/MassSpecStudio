using System.IO;
using Hydra.Core.Domain;
using Hydra.Modules.Run.Views;
using MassSpecStudio.Core;
using MassSpecStudio.Core.Domain;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Hydra.Test.Modules.Runs.Views
{
	[TestClass]
	public class RunViewModelTest
	{
		private RunViewModel viewModel;
		private Mock<IServiceLocator> mockServiceLocator;
		private Mock<IEventAggregator> mockEventAggregator;
		private Mock<IDataProvider> mockDataProvider;
		private Run run;
		private Experiment experiment;

		[TestInitialize]
		public void TestInitialize()
		{
			IXYData xyData = new XYData(new double[] { 1, 2 }, new double[] { 3, 4 });
			mockServiceLocator = new Mock<IServiceLocator>();
			mockEventAggregator = new Mock<IEventAggregator>();
			mockDataProvider = new Mock<IDataProvider>();
			mockDataProvider.Setup(mock => mock.GetTotalIonChromatogram(TimeUnit.Seconds)).Returns(xyData);
			mockDataProvider.Setup(mock => mock.GetExtractedIonChromatogram(2, 2, 0.5, TimeUnit.Seconds)).Returns(xyData);
			mockDataProvider.Setup(mock => mock.GetSpectrum(2, 3, 3, 6, 0)).Returns(xyData);

			experiment = Test.TestHelper.GetTestExperiment(mockServiceLocator);
			run = new Run(Path.GetFileName(Properties.Settings.Default.mzXMLTestFile1), Properties.Settings.Default.mzXMLTestFile1, new ProteinState(experiment), new Labeling(experiment), experiment, mockDataProvider.Object);
		}

		[TestMethod]
		public void ConstructorTic()
		{
			viewModel = new RunViewModel(run);
			Assert.AreEqual(run, viewModel.Run);
			Assert.AreEqual("TIC - file07 - run1 - apo-CaM 50%0025D2O trial1.wiff.mzXML", viewModel.XYData.Title);
			Assert.AreEqual(2, viewModel.XYData.XValues.Count);
		}

		[TestMethod]
		public void ConstructorXic()
		{
			viewModel = new RunViewModel(run, 2, 0.5);
			Assert.AreEqual(run, viewModel.Run);
			Assert.AreEqual("XIC (2.0000 mzTol=0.5) - file07 - run1 - apo-CaM 50%0025D2O trial1.wiff.mzXML", viewModel.XYData.Title);
			Assert.AreEqual(2, viewModel.XYData.XValues.Count);
		}

		[TestMethod]
		public void ConstructorSpectrum()
		{
			viewModel = new RunViewModel(run, 2, 3, 4, 1, 2);
			Assert.AreEqual(run, viewModel.Run);
			Assert.AreEqual("MS (RT=2.00-3.00 Mass=4.0000) - file07 - run1 - apo-CaM 50%0025D2O trial1.wiff.mzXML", viewModel.XYData.Title);
			Assert.AreEqual(2, viewModel.XYData.XValues.Count);
		}
	}
}
