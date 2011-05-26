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
	public class XicSelectionTest
	{
		private XicSelection xicSelection;
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
			xicSelection = new XicSelection(mockEventAggregator.Object, mockRegionManager.Object);
		}

		[TestMethod]
		public void MzVarValue()
		{
			xicSelection.MzVar = 2.5;
			Assert.AreEqual(2.5, xicSelection.MzVar);
		}

		[TestMethod]
		public void Execute()
		{
			mockDataProvider.Setup(mock => mock.GetExtractedIonChromatogram(200, 200, 2.5, TimeUnit.Seconds)).Returns(new XYData(new double[] { 1, 2 }, new double[] { 3, 4 }));
			clickableOutputEvent.Subscribe(OnPublish);

			Sample sample = new Sample("test.mzxml", "test.mzxml", mockDataProvider.Object);
			xicSelection.MzVar = 2.5;
			IXYData xyData = xicSelection.Execute(sample, 200);

			Assert.AreEqual("XIC (200.0000 mzTol=2.5) - test.mzxml", xyData.Title);
			Assert.AreEqual(true, publishCalled);
		}

		private void OnPublish(ClickableOutputEvent outputEvent)
		{
			publishCalled = true;
			Assert.AreEqual("     XIC found (sample=test.mzxml, x values=2,  y values=2, time unit=Seconds)", outputEvent.Text);
			Assert.AreEqual(2, ((IXYData)outputEvent.Parameter).XValues.Count);
			Assert.AreEqual(1, ((IXYData)outputEvent.Parameter).XValues[0]);
			Assert.AreEqual(2, ((IXYData)outputEvent.Parameter).XValues[1]);
			Assert.AreEqual(3, ((IXYData)outputEvent.Parameter).YValues[0]);
			Assert.AreEqual(4, ((IXYData)outputEvent.Parameter).YValues[1]);
			Assert.IsNotNull(outputEvent.Click);
		}
	}
}
