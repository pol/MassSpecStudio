using System.Collections.Generic;
using Hydra.Core;
using Hydra.Core.Domain;
using Hydra.Processing.Algorithm.Steps;
using MassSpecStudio.Core.Domain;
using MassSpecStudio.Core.Events;
using Microsoft.Practices.Prism.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Hydra.Test.Processing.Algorithm.Steps
{
	[TestClass]
	public class XicPeakPickerTest
	{
		private XicPeakPicker xicPeakPicker;
		private Mock<IEventAggregator> mockEventAggregator;
		private OutputEvent outputEvent;
		private bool publishCalled = false;
		private int messageCount = 0;

		[TestInitialize]
		public void MyTestInitialize()
		{
			mockEventAggregator = new Mock<IEventAggregator>();

			outputEvent = new OutputEvent();
			mockEventAggregator.Setup(mock => mock.GetEvent<OutputEvent>()).Returns(outputEvent);
			xicPeakPicker = new XicPeakPicker(mockEventAggregator.Object);
		}

		[TestMethod]
		public void ProcessingParameterValues()
		{
			xicPeakPicker.XicPeakPickerOption = XicPeakPickerOption.ClosestToRTWithinRTVariation;
			Assert.AreEqual(XicPeakPickerOption.ClosestToRTWithinRTVariation, xicPeakPicker.XicPeakPickerOption);
		}

		[TestMethod]
		public void ExecuteWithMostIntenseWithinEntireXic()
		{
			IList<ChromatographicPeak> testPeaks = SetupPeakList();

			Peptide testPeptide = new Peptide("AAA");

			outputEvent.Subscribe(OnPublishMostIntenseWithinEntireXic);

			xicPeakPicker.XicPeakPickerOption = XicPeakPickerOption.MostIntenseWithinEntireXic;
			ChromatographicPeak selectedPeak = xicPeakPicker.Execute(testPeaks, testPeptide);

			Assert.IsNotNull(selectedPeak);
			Assert.AreEqual(testPeaks[3], selectedPeak);
			Assert.AreEqual(true, publishCalled);
			Assert.AreEqual(2, messageCount);
		}

		[TestMethod]
		public void ExecuteWithMostIntenseWithinEntireXicWithoutAnyPeaks()
		{
			IList<ChromatographicPeak> testPeaks = new List<ChromatographicPeak>();

			Peptide testPeptide = new Peptide("AAA");

			outputEvent.Subscribe(OnPublishMostIntenseWithinEntireXicWithoutPeaks);

			xicPeakPicker.XicPeakPickerOption = XicPeakPickerOption.MostIntenseWithinEntireXic;
			ChromatographicPeak selectedPeak = xicPeakPicker.Execute(testPeaks, testPeptide);

			Assert.IsNull(selectedPeak);
			Assert.AreEqual(true, publishCalled);
			Assert.AreEqual(2, messageCount);
		}

		[TestMethod]
		public void ExecuteMostIntenseWithinRtVariation()
		{
			IList<ChromatographicPeak> testPeaks = SetupPeakList();

			Peptide testPeptide = CreatePeptide(3.25, 0.3);

			outputEvent.Subscribe(OnPublishMostIntenseWithinRtVariation);

			xicPeakPicker.XicPeakPickerOption = XicPeakPickerOption.MostIntenseWithinRtVariation;
			ChromatographicPeak selectedPeak = xicPeakPicker.Execute(testPeaks, testPeptide);

			Assert.IsNotNull(selectedPeak);
			Assert.AreEqual(testPeaks[1], selectedPeak);
			Assert.AreEqual(true, publishCalled);
			Assert.AreEqual(2, messageCount);
		}

		[TestMethod]
		public void ExecuteMostIntenseWithinRtVariationWithNoPeaksWithinTheRtWindow()
		{
			IList<ChromatographicPeak> testPeaks = SetupPeakList();

			Peptide testPeptide = CreatePeptide(4.0, 0.3);

			outputEvent.Subscribe(OnPublishMostIntenseWithinRtVariationWithoutPeakInRtWindow);

			xicPeakPicker.XicPeakPickerOption = XicPeakPickerOption.MostIntenseWithinRtVariation;
			ChromatographicPeak selectedPeak = xicPeakPicker.Execute(testPeaks, testPeptide);

			Assert.IsNull(selectedPeak);
			Assert.AreEqual(true, publishCalled);
			Assert.AreEqual(2, messageCount);
		}

		[TestMethod]
		public void ExecuteMostIntenseWithinRtVariationWithoutAnyPeaks()
		{
			IList<ChromatographicPeak> testPeaks = new List<ChromatographicPeak>();

			Peptide testPeptide = CreatePeptide(3.25, 0.3);

			outputEvent.Subscribe(OnPublishMostIntenseWithinRtVariationWithoutPeakFound);

			xicPeakPicker.XicPeakPickerOption = XicPeakPickerOption.MostIntenseWithinRtVariation;
			ChromatographicPeak selectedPeak = xicPeakPicker.Execute(testPeaks, testPeptide);

			Assert.IsNull(selectedPeak);
			Assert.AreEqual(true, publishCalled);
			Assert.AreEqual(2, messageCount);
		}

		[TestMethod]
		public void ExecuteClosestToRTWithinRTVariation()
		{
			IList<ChromatographicPeak> testPeaks = SetupPeakList();

			Peptide testPeptide = CreatePeptide(3.99, 1.0);

			outputEvent.Subscribe(OnPublishClosestToRTWithinRTVariation);

			xicPeakPicker.XicPeakPickerOption = XicPeakPickerOption.ClosestToRTWithinRTVariation;
			ChromatographicPeak selectedPeak = xicPeakPicker.Execute(testPeaks, testPeptide);

			Assert.IsNotNull(selectedPeak);
			Assert.AreEqual(testPeaks[2], selectedPeak);
			Assert.AreEqual(true, publishCalled);
			Assert.AreEqual(2, messageCount);
		}

		[TestMethod]
		public void ExecuteClosestToRTWithinRTVariationWithPeakOutsideTheRtWindow()
		{
			IList<ChromatographicPeak> testPeaks = SetupPeakList();

			Peptide testPeptide = CreatePeptide(4.0, 0.35);

			outputEvent.Subscribe(OnPublishClosestToRTWithinRTVariationWithPeaksOutsideTheRtWindow);

			xicPeakPicker.XicPeakPickerOption = XicPeakPickerOption.ClosestToRTWithinRTVariation;
			ChromatographicPeak selectedPeak = xicPeakPicker.Execute(testPeaks, testPeptide);

			Assert.IsNull(selectedPeak);
			Assert.AreEqual(true, publishCalled);
			Assert.AreEqual(2, messageCount);
		}

		[TestMethod]
		public void ExecuteClosestToRTWithinRTVariationWithoutAnyPeaks()
		{
			IList<ChromatographicPeak> testPeaks = new List<ChromatographicPeak>();

			Peptide testPeptide = CreatePeptide(3.25, 0.25);

			outputEvent.Subscribe(OnPublishClosestToRTWithinRTVariationWithoutPeakFound);

			xicPeakPicker.XicPeakPickerOption = XicPeakPickerOption.ClosestToRTWithinRTVariation;
			ChromatographicPeak selectedPeak = xicPeakPicker.Execute(testPeaks, testPeptide);

			Assert.IsNull(selectedPeak);
			Assert.AreEqual(true, publishCalled);
			Assert.AreEqual(2, messageCount);
		}

		private void OnPublishMostIntenseWithinEntireXic(string actualValue)
		{
			publishCalled = true;
			switch (messageCount)
			{
				case 0:
					Assert.AreEqual("     XIC Peak Picking Started (XICPeakPickerOption=MostIntenseWithinEntireXic)", actualValue);
					break;
				case 1:
					Assert.AreEqual("     Largest Peak Found: RT=4.5, PeakHeight=100, PeakWidth=2)", actualValue);
					break;
			}
			messageCount++;
		}

		private void OnPublishMostIntenseWithinEntireXicWithoutPeaks(string actualValue)
		{
			publishCalled = true;
			switch (messageCount)
			{
				case 0:
					Assert.AreEqual("     XIC Peak Picking Started (XICPeakPickerOption=MostIntenseWithinEntireXic)", actualValue);
					break;
				case 1:
					Assert.AreEqual("     No peaks present within the chromatographic peak list, so no peak could be selected.", actualValue);
					break;
			}
			messageCount++;
		}

		private void OnPublishMostIntenseWithinRtVariation(string actualValue)
		{
			publishCalled = true;
			switch (messageCount)
			{
				case 0:
					Assert.AreEqual("     XIC Peak Picking Started (XICPeakPickerOption=MostIntenseWithinRtVariation)", actualValue);
					break;
				case 1:
					Assert.AreEqual("     Most Intense Peak Found: RT=3.5, PeakHeight=50, PeakWidth=2)", actualValue);
					break;
			}
			messageCount++;
		}

		private void OnPublishMostIntenseWithinRtVariationWithoutPeakFound(string actualValue)
		{
			publishCalled = true;
			switch (messageCount)
			{
				case 0:
					Assert.AreEqual("     XIC Peak Picking Started (XICPeakPickerOption=MostIntenseWithinRtVariation)", actualValue);
					break;
				case 1:
					Assert.AreEqual("     No peaks found within RT windows (2.95 - 3.55)", actualValue);
					break;
			}
			messageCount++;
		}

		private void OnPublishMostIntenseWithinRtVariationWithoutPeakInRtWindow(string actualValue)
		{
			publishCalled = true;
			switch (messageCount)
			{
				case 0:
					Assert.AreEqual("     XIC Peak Picking Started (XICPeakPickerOption=MostIntenseWithinRtVariation)", actualValue);
					break;
				case 1:
					Assert.AreEqual("     No peaks found within RT windows (3.7 - 4.3)", actualValue);
					break;
			}
			messageCount++;
		}

		private void OnPublishClosestToRTWithinRTVariation(string actualValue)
		{
			publishCalled = true;
			switch (messageCount)
			{
				case 0:
					Assert.AreEqual("     XIC Peak Picking Started (XICPeakPickerOption=ClosestToRTWithinRTVariation)", actualValue);
					break;
				case 1:
					Assert.AreEqual("     Closest RT Peak Found: RT=3.6, PeakHeight=30, PeakWidth=2)", actualValue);
					break;
			}
			messageCount++;
		}

		private void OnPublishClosestToRTWithinRTVariationWithoutPeakFound(string actualValue)
		{
			publishCalled = true;
			switch (messageCount)
			{
				case 0:
					Assert.AreEqual("     XIC Peak Picking Started (XICPeakPickerOption=ClosestToRTWithinRTVariation)", actualValue);
					break;
				case 1:
					Assert.AreEqual("     No peaks found within RT windows (3 - 3.5)", actualValue);
					break;
			}
			messageCount++;
		}

		private void OnPublishClosestToRTWithinRTVariationWithPeaksOutsideTheRtWindow(string actualValue)
		{
			publishCalled = true;
			switch (messageCount)
			{
				case 0:
					Assert.AreEqual("     XIC Peak Picking Started (XICPeakPickerOption=ClosestToRTWithinRTVariation)", actualValue);
					break;
				case 1:
					Assert.AreEqual("     No peaks found within RT windows (3.65 - 4.35)", actualValue);
					break;
			}
			messageCount++;
		}

		private IList<ChromatographicPeak> SetupPeakList()
		{
			IList<ChromatographicPeak> testPeaks = new List<ChromatographicPeak>();
			testPeaks.Add(new ChromatographicPeak(3.4, 0, 20, 2));
			testPeaks.Add(new ChromatographicPeak(3.5, 0, 50, 2));
			testPeaks.Add(new ChromatographicPeak(3.6, 0, 30, 2));
			testPeaks.Add(new ChromatographicPeak(4.5, 0, 100, 2));
			testPeaks.Add(new ChromatographicPeak(5.5, 0, 75, 2));
			return testPeaks;
		}

		private Peptide CreatePeptide(double rt, double rtVariance)
		{
			Peptide testPeptide = new Peptide("AAA");
			testPeptide.RT = rt;
			testPeptide.RtVariance = rtVariance;
			return testPeptide;
		}
	}
}
