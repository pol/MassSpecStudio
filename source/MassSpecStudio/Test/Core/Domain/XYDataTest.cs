using MassSpecStudio.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MassSpecStudio.Test.Core.Domain
{
	[TestClass]
	public class XYDataTest
	{
		private IXYData xyData;

		[TestInitialize()]
		public void MyTestInitialize()
		{
			xyData = new XYData(XYDataHelper.XValueTestSet1, XYDataHelper.YValueTestSet1);
		}

		[TestMethod]
		public void Constructor()
		{
			xyData = new XYData(XYDataHelper.XValueTestSet2, XYDataHelper.YValueTestSet2);

			XYDataHelper.AssertXYData(xyData, XYDataHelper.XValueTestSet2, XYDataHelper.YValueTestSet2);
		}

		[TestMethod]
		public void GetXValues()
		{
			Assert.AreEqual(3, xyData.XValues.Count);
		}

		[TestMethod]
		public void GetYValues()
		{
			Assert.AreEqual(3, xyData.YValues.Count);
		}
	}
}
