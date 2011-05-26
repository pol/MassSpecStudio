using System;
using System.Collections.Generic;
using DeconTools.MassSpecStudio.Processing.Steps;
using MassSpecStudio.Core;
using MassSpecStudio.Core.Domain;
using MassSpecStudio.Core.Events;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ProteoWizard.MassSpecStudio.DataProvider;

namespace DeconTools.MassSpecStudio.Processing.Test.Steps
{
	[TestClass]
	public class SavitzkyGolaySmoothingTest
	{
		private SavitzkyGolaySmoothing smoothing;
		private Mock<IEventAggregator> mockEventAggregator;
		private Mock<IDataProvider> mockDataProvider;
		private OutputEvent outputEvent;
		private ClickableOutputEvent clickableOutputEvent;
		private bool publishCalled = false;
		private bool clickablePublishCalled = false;

		[TestInitialize]
		public void MyTestInitialize()
		{
			Mock<IRegionManager> mockRegionManager = new Mock<IRegionManager>();
			mockDataProvider = new Mock<IDataProvider>();
			mockEventAggregator = new Mock<IEventAggregator>();

			outputEvent = new OutputEvent();
			clickableOutputEvent = new ClickableOutputEvent();
			mockEventAggregator.Setup(mock => mock.GetEvent<ClickableOutputEvent>()).Returns(clickableOutputEvent);
			mockEventAggregator.Setup(mock => mock.GetEvent<OutputEvent>()).Returns(outputEvent);
			smoothing = new SavitzkyGolaySmoothing(mockEventAggregator.Object, mockRegionManager.Object);
		}

		[TestMethod]
		public void ProcessingParameterValues()
		{
			smoothing.Enabled = true;
			Assert.AreEqual(true, smoothing.Enabled);
			smoothing.LeftParam = 3;
			Assert.AreEqual(3, smoothing.LeftParam);
			smoothing.RightParam = 4;
			Assert.AreEqual(4, smoothing.RightParam);
			smoothing.Order = 5;
			Assert.AreEqual(5, smoothing.Order);
		}

		[TestMethod]
		public void ExecuteOnSpectrum()
		{
			IDataProvider realDataProvider = new ProteoWizardDataProvider(mockEventAggregator.Object);
			realDataProvider.Open(Properties.Settings.Default.mzXMLTestFile1, 0);
			IXYData xyData = realDataProvider.GetSpectrum(1.5, 1.5, 298, 302, 0.1);
			SetupOutputEvents();

			IXYData smoothedXYData = smoothing.Execute(xyData);

			Assert.AreEqual(xyData.XValues.Count, smoothedXYData.XValues.Count);
			Assert.AreEqual(37, xyData.YValues[14]);
			Assert.AreEqual(15.47619, Math.Round(smoothedXYData.YValues[14], 5));
			Assert.AreEqual(true, publishCalled);
			Assert.AreEqual(true, clickablePublishCalled);
		}

		[TestMethod]
		public void ExecuteOnChromatogram()
		{
			IXYData xyData = LoadChromatogram();

			IXYData smoothedXYData = smoothing.Execute(xyData);

			Assert.AreEqual(xyData.XValues.Count, smoothedXYData.XValues.Count);
			Assert.AreEqual(11338, xyData.YValues[200]);
			Assert.AreEqual(7062.24, Math.Round(smoothedXYData.YValues[200], 2));
			Assert.AreEqual(true, publishCalled);
			Assert.AreEqual(true, clickablePublishCalled);
		}

		[TestMethod]
		public void SmoothingDisabled()
		{
			IXYData xyData = LoadChromatogram();

			smoothing.Enabled = false;
			IXYData smoothedXYData = smoothing.Execute(xyData);

			Assert.AreEqual(xyData.XValues.Count, smoothedXYData.XValues.Count);
			Assert.AreEqual(11338, xyData.YValues[200]);
			Assert.AreEqual(11338, Math.Round(smoothedXYData.YValues[200], 5));
			Assert.AreEqual(false, publishCalled);
			Assert.AreEqual(false, clickablePublishCalled);
		}

		[TestMethod]
		public void ExecuteWithoutXYData()
		{
			IXYData xyData = new XYData(new List<XYPoint>());
			SetupOutputEvents();
			IXYData smoothedXYData = smoothing.Execute(xyData);

			Assert.AreEqual(xyData.XValues.Count, smoothedXYData.XValues.Count);
			Assert.AreEqual(0, smoothedXYData.XValues.Count);
			Assert.AreEqual(true, publishCalled);
			Assert.AreEqual(true, clickablePublishCalled);
		}

		[TestMethod]
		public void ExecuteWithoutPeaks()
		{
			IXYData xyData = new XYData(new double[] { 1, 2, 3, 4, 5 }, new double[] { 2, 2, 2, 2, 2 });
			SetupOutputEvents();
			IXYData smoothedXYData = smoothing.Execute(xyData);

			Assert.AreEqual(xyData.XValues.Count, smoothedXYData.XValues.Count);
			Assert.AreEqual(2, xyData.YValues[3]);
			Assert.AreEqual(2.00000, Math.Round(smoothedXYData.YValues[3], 5));
			Assert.AreEqual(true, publishCalled);
			Assert.AreEqual(true, clickablePublishCalled);
		}

		private void OnPublish(string actualValue)
		{
			publishCalled = true;
			Assert.AreEqual("     Savitzky Golay Smoothing Started (LeftParam=3, RightParam=3, Order=2)", actualValue);
		}

		private void OnClickablePublish(ClickableOutputEvent actualValue)
		{
			clickablePublishCalled = true;
			Assert.AreEqual("     Smoothing completed", actualValue.Text);
		}

		private IXYData LoadChromatogram()
		{
			IDataProvider realDataProvider = new ProteoWizardDataProvider(mockEventAggregator.Object);
			realDataProvider.Open(Properties.Settings.Default.mzXMLTestFile1, 0);
			IXYData xyData = realDataProvider.GetTotalIonChromatogram(TimeUnit.Seconds);
			SetupOutputEvents();
			return xyData;
		}

		private void SetupOutputEvents()
		{
			outputEvent.Subscribe(OnPublish);
			clickableOutputEvent.Subscribe(OnClickablePublish);
		}
	}
}
