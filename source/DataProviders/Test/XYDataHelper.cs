using MSStudio.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MSStudio.DataProvider.Test
{
	internal static class XYDataHelper
	{
		internal static XYData Generate(double[] xValues, double[] yValues)
		{
			XYData xyData = new XYData(xValues, yValues);
			return xyData;
		}

		internal static void AssertXYData(XYData xyData, double[] expectedXValues, double[] expectedYValues)
		{
			Assert.AreEqual(expectedXValues.Length, xyData.XValues.Length);
			Assert.AreEqual(expectedYValues.Length, xyData.YValues.Length);

			for (int i=0; i < xyData.XValues.Length; i++)
			{
				Assert.AreEqual(expectedXValues[i], xyData.XValues[i]);
				Assert.AreEqual(expectedYValues[i], xyData.YValues[i]);
			}
		}
	}
}
