using MassSpecStudio.Core;
using MassSpecStudio.Core.Domain;
using MassSpecStudio.Core.Events;
using MassSpecStudio.Processing.Steps;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MassSpecStudio.Test.Processing.Steps
{
	[TestClass]
	public class SpectrumSelectionTest
	{
		private SpectrumSelection spectrumSelection;
		private Mock<IDataProvider> mockDataProvider;
		private ClickableOutputEvent clickableOutputEvent;
		private bool publishCalled = false;

		[TestInitialize]
		public void MyTestInitialize()
		{
			mockDataProvider = new Mock<IDataProvider>();
			Mock<IEventAggregator> mockEventAggregator = new Mock<IEventAggregator>();
			Mock<IRegionManager> mockRegionManager = new Mock<IRegionManager>();

			clickableOutputEvent = new ClickableOutputEvent();
			mockEventAggregator.Setup(mock => mock.GetEvent<ClickableOutputEvent>()).Returns(clickableOutputEvent);
			spectrumSelection = new SpectrumSelection(mockEventAggregator.Object, mockRegionManager.Object);
		}

		[TestMethod]
		public void MzOffsetValue()
		{
			spectrumSelection.MZLowerOffset = 2.5;
			Assert.AreEqual(2.5, spectrumSelection.MZLowerOffset);
			spectrumSelection.MZUpperOffset = 3.5;
			Assert.AreEqual(3.5, spectrumSelection.MZUpperOffset);
		}

		[TestMethod]
		public void Execute()
		{
			mockDataProvider.Setup(mock => mock.GetSpectrum(2.25, 2.75, 250.5, 251.5, 0)).Returns(new XYData(new double[] { 1, 2 }, new double[] { 3, 4 }));
			clickableOutputEvent.Subscribe(OnPublish);

			Sample sample = new Sample("test.mzxml", "test.mzxml", mockDataProvider.Object);
			spectrumSelection.MZLowerOffset = 0.5;
			spectrumSelection.MZUpperOffset = 1.5;
			IXYData xyData = spectrumSelection.Execute(sample, 2.25, 2.75, 250);

			Assert.AreEqual("MS (RT=2.25-2.75 Mass=250.0000) - test.mzxml", xyData.Title);
			Assert.AreEqual(true, publishCalled);
		}

		private void OnPublish(ClickableOutputEvent outputEvent)
		{
			publishCalled = true;
			Assert.AreEqual("     Spectrum found (sample=test.mzxml, x values=2,  y values=2)", outputEvent.Text);
			Assert.AreEqual(2, ((IXYData)outputEvent.Parameter).XValues.Count);
			Assert.AreEqual(1, ((IXYData)outputEvent.Parameter).XValues[0]);
			Assert.AreEqual(2, ((IXYData)outputEvent.Parameter).XValues[1]);
			Assert.AreEqual(3, ((IXYData)outputEvent.Parameter).YValues[0]);
			Assert.AreEqual(4, ((IXYData)outputEvent.Parameter).YValues[1]);
			Assert.IsNotNull(outputEvent.Click);
		}
	}
}