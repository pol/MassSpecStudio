using System;
using System.Collections.Generic;
using DeconTools.MassSpecStudio.Processing.Steps;
using MassSpecStudio.Core;
using MassSpecStudio.Core.Domain;
using MassSpecStudio.Core.Events;
using Microsoft.Practices.Prism.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ProteoWizard.MassSpecStudio.DataProvider;

namespace DeconTools.MassSpecStudio.Processing.Test.Steps
{
	[TestClass]
	public class ChromatographicPeakDetectionTest
	{
		private ChromatographicPeakDetection chromatographicPeakDetection;
		private Mock<IEventAggregator> mockEventAggregator;
		private Mock<IDataProvider> mockDataProvider;
		private OutputEvent outputEvent;
		private bool publishCalled = false;
		private int messageCount = 0;

		[TestInitialize]
		public void MyTestInitialize()
		{
			mockDataProvider = new Mock<IDataProvider>();
			mockEventAggregator = new Mock<IEventAggregator>();

			outputEvent = new OutputEvent();
			mockEventAggregator.Setup(mock => mock.GetEvent<OutputEvent>()).Returns(outputEvent);
			chromatographicPeakDetection = new ChromatographicPeakDetection(mockEventAggregator.Object);
		}

		[TestMethod]
		public void ProcessingParameterValues()
		{
			chromatographicPeakDetection.PeakToBackgroundRatio = 2.5;
			Assert.AreEqual(2.5, chromatographicPeakDetection.PeakToBackgroundRatio);
			chromatographicPeakDetection.SignalToNoiseThreshold = 3.5;
			Assert.AreEqual(3.5, chromatographicPeakDetection.SignalToNoiseThreshold);
		}

		[TestMethod]
		public void Execute()
		{
			IDataProvider realDataProvider = new ProteoWizardDataProvider(mockEventAggregator.Object);
			realDataProvider.Open(Properties.Settings.Default.mzXMLTestFile1, 0);
			IXYData xyData = realDataProvider.GetTotalIonChromatogram(TimeUnit.Seconds);
			outputEvent.Subscribe(OnPublish);

			IList<ChromatographicPeak> peaks = chromatographicPeakDetection.Execute(xyData);

			Assert.AreEqual(1, peaks.Count);
			Assert.AreEqual(7.58158, Math.Round(peaks[0].Rt, 5));
			Assert.AreEqual(69884, peaks[0].PeakHeight);
			Assert.AreEqual(3, messageCount);
			Assert.AreEqual(true, publishCalled);
		}

		[TestMethod]
		public void ExecuteWithoutXYData()
		{
			IXYData xyData = new XYData(new List<XYPoint>());
			outputEvent.Subscribe(OnPublishWithoutPeaks);

			IList<ChromatographicPeak> peaks = chromatographicPeakDetection.Execute(xyData);

			Assert.AreEqual(0, peaks.Count);
			Assert.AreEqual(2, messageCount);
			Assert.AreEqual(true, publishCalled);
		}

		[TestMethod]
		public void ExecuteWithoutPeaks()
		{
			XYData xyData = new XYData(new double[] { 1, 2 }, new double[] { 1, 1 });
			outputEvent.Subscribe(OnPublishWithoutPeaks);

			IList<ChromatographicPeak> peaks = chromatographicPeakDetection.Execute(xyData);

			Assert.AreEqual(0, peaks.Count);
			Assert.AreEqual(2, messageCount);
			Assert.AreEqual(true, publishCalled);
		}

		private void OnPublish(string actualValue)
		{
			publishCalled = true;
			switch (messageCount)
			{
				case 0:
					Assert.AreEqual("     Chromatographic Peak Detection Started (PeakToBackgroundRatio=2, SignalToNoiseThreshold=3)", actualValue);
					break;
				case 1:
					Assert.AreEqual("     Peak Found: RT=7.58158020730557, PeakHeight=69884, PeakWidth=0.112783404683688)", actualValue);
					break;
				case 2:
					Assert.AreEqual("     1 peaks found.", actualValue);
					break;
			}
			messageCount++;
		}

		private void OnPublishWithoutPeaks(string actualValue)
		{
			publishCalled = true;
			switch (messageCount)
			{
				case 0:
					Assert.AreEqual("     Chromatographic Peak Detection Started (PeakToBackgroundRatio=2, SignalToNoiseThreshold=3)", actualValue);
					break;
				case 1:
					Assert.AreEqual("     0 peaks found.", actualValue);
					break;
			}
			messageCount++;
		}
	}
}