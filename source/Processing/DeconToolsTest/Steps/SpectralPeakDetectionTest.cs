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
	public class SpectralPeakDetectionTest
	{
		private SpectralPeakDetection spectralPeakDetection;
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
			spectralPeakDetection = new SpectralPeakDetection(mockEventAggregator.Object);
		}

		[TestMethod]
		public void ProcessingParameterValues()
		{
			spectralPeakDetection.PeakToBackgroundRatio = 2.5;
			Assert.AreEqual(2.5, spectralPeakDetection.PeakToBackgroundRatio);
			spectralPeakDetection.SignalToNoiseThreshold = 3.5;
			Assert.AreEqual(3.5, spectralPeakDetection.SignalToNoiseThreshold);
		}

		[TestMethod]
		public void Execute()
		{
			IDataProvider realDataProvider = new ProteoWizardDataProvider(mockEventAggregator.Object);
			realDataProvider.Open(Properties.Settings.Default.mzXMLTestFile1, 0);
			IXYData xyData = realDataProvider.GetSpectrum(1.5, 1.5, 298, 302, 0.1);
			outputEvent.Subscribe(OnPublish);

			IList<MSPeak> peaks = spectralPeakDetection.Execute(xyData);

			Assert.AreEqual(2, peaks.Count);
			Assert.AreEqual(301.11931, Math.Round(peaks[0].MZ, 5));
			Assert.AreEqual(18, peaks[0].Intensity);
			Assert.AreEqual(301.13492, Math.Round(peaks[1].MZ, 5));
			Assert.AreEqual(37, peaks[1].Intensity);
			Assert.AreEqual(4, messageCount);
			Assert.AreEqual(true, publishCalled);
		}

		[TestMethod]
		public void ExecuteWithoutXYData()
		{
			XYData xyData = new XYData(new List<XYPoint>());
			outputEvent.Subscribe(OnPublishWithoutPeaks);

			IList<MSPeak> peaks = spectralPeakDetection.Execute(xyData);

			Assert.AreEqual(0, peaks.Count);
			Assert.AreEqual(2, messageCount);
			Assert.AreEqual(true, publishCalled);
		}

		[TestMethod]
		public void ExecuteWithoutPeaks()
		{
			IDataProvider realDataProvider = new ProteoWizardDataProvider(mockEventAggregator.Object);
			realDataProvider.Open(Properties.Settings.Default.mzXMLTestFile1, 0);
			IXYData xyData = realDataProvider.GetSpectrum(1.5, 1.5, 298, 300, 0.1);
			outputEvent.Subscribe(OnPublishWithoutPeaks);

			IList<MSPeak> peaks = spectralPeakDetection.Execute(xyData);

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
					Assert.AreEqual("     Spectral Peak Detection Started (PeakToBackgroundRatio=2, SignalToNoiseThreshold=3)", actualValue);
					break;
				case 1:
					Assert.AreEqual("     Peak Found: MZ=301.119312738355, Intensity=18, PeakWidth=0.0580520629882813)", actualValue);
					break;
				case 2:
					Assert.AreEqual("     Peak Found: MZ=301.13491708261, Intensity=37, PeakWidth=0.0253819552335131)", actualValue);
					break;
				case 3:
					Assert.AreEqual("     2 peaks found.", actualValue);
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
					Assert.AreEqual("     Spectral Peak Detection Started (PeakToBackgroundRatio=2, SignalToNoiseThreshold=3)", actualValue);
					break;
				case 1:
					Assert.AreEqual("     0 peaks found.", actualValue);
					break;
			}
			messageCount++;
		}
	}
}