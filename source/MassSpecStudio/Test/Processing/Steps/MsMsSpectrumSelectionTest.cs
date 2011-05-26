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
	public class MsMsSpectrumSelectionTest
	{
		private MsMsSpectrumSelection msmsSpectrumSelection;
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
			msmsSpectrumSelection = new MsMsSpectrumSelection(mockEventAggregator.Object, mockRegionManager.Object);
		}

		[TestMethod]
		public void Properties()
		{
			msmsSpectrumSelection.LCPeakElutionWidth = 2.5;
			Assert.AreEqual(2.5, msmsSpectrumSelection.LCPeakElutionWidth);
			msmsSpectrumSelection.MZVariablility = 3.5;
			Assert.AreEqual(3.5, msmsSpectrumSelection.MZVariablility);
		}

		[TestMethod]
		public void Execute()
		{
			mockDataProvider.Setup(mock => mock.GetMsMsSpectrum(7.3, 0.5, 210.2, 1.5)).Returns(new XYData(new double[] { 1, 2 }, new double[] { 3, 4 }));
			clickableOutputEvent.Subscribe(OnPublish);

			Sample sample = new Sample("test.mzxml", "test.mzxml", mockDataProvider.Object);
			msmsSpectrumSelection.LCPeakElutionWidth = 0.5;
			msmsSpectrumSelection.MZVariablility = 1.5;
			IXYData xyData = msmsSpectrumSelection.Execute(sample, 7.3, 210.2);

			Assert.AreEqual("MSMS (RT=7.30 LCPeakElutionWidth=0.50 mz=210.20 MZ Range=1.50) - test.mzxml", xyData.Title);
			Assert.AreEqual(true, publishCalled);
		}

		private void OnPublish(ClickableOutputEvent outputEvent)
		{
			publishCalled = true;
			Assert.AreEqual("     MSMS Spectrum found (sample=test.mzxml, x values=2,  y values=2)", outputEvent.Text);
			Assert.AreEqual(2, ((XYData)outputEvent.Parameter).Count);
			Assert.AreEqual(1, ((XYData)outputEvent.Parameter).XValues[0]);
			Assert.AreEqual(2, ((XYData)outputEvent.Parameter).XValues[1]);
			Assert.AreEqual(3, ((XYData)outputEvent.Parameter).YValues[0]);
			Assert.AreEqual(4, ((XYData)outputEvent.Parameter).YValues[1]);
			Assert.IsNotNull(outputEvent.Click);
		}
	}
}