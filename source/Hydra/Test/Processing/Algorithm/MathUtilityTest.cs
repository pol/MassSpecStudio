using System.Collections.Generic;
using Hydra.Processing.Algorithm;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hydra.Test.Processing.Algorithm
{
	[TestClass]
	public class MathUtilityTest
	{
		private double[] testValues1 = new double[] { 0.740435867544875, 0.808635362302084, 0.725712087432157, 0.784402326493819, 0.73567651483927, 0.699395076131395 };
		private double[] testValues2 = new double[] { 0.30490661744534, 0.306350705557861, 0.290739894537865, 0.296395824495448, 0.269736885125212 };
		private double[] calmodulinApo10percent = { 0.326883584384313, 0.325947346337873, 0.298581706005955 };
		private double[] calmodulinHolo10percent = { 0.018320986074628, 0.0299240447718751, 0.0140870092423757 };

		[TestMethod]
		public void GetAverageViaListOfDoubles()
		{
			List<double> list = ConvertToList(testValues1);
			Assert.AreEqual(0.74904287245726664d, MathUtility.GetAverage(list));
		}

		[TestMethod]
		public void GetAverageViaArrayOfDoubles()
		{
			Assert.AreEqual(0.74904287245726664d, MathUtility.GetAverage(testValues1));
		}

		[TestMethod]
		public void GetAverageViaArrayOfDoublesWhenNoValues()
		{
			List<double> list = new List<double>();
			Assert.AreEqual(double.NaN, MathUtility.GetAverage(list));
		}

		[TestMethod]
		public void GetStandardDeviationViaListOfDoubles()
		{
			List<double> list = ConvertToList(testValues1);
			Assert.AreEqual(0.040158498714730234d, MathUtility.GetStandardDeviation(list));
		}

		[TestMethod]
		public void GetStandardDeviationViaArrayOfDoubles()
		{
			Assert.AreEqual(0.040158498714730234d, MathUtility.GetStandardDeviation(testValues1));
			Assert.AreEqual(0.016076645235656092, MathUtility.GetStandardDeviation(calmodulinApo10percent));
		}

		[TestMethod]
		public void GetStandardDeviationViaArrayOfDoublesWhenNoValues()
		{
			List<double> list = new List<double>();
			Assert.AreEqual(double.NaN, MathUtility.GetStandardDeviation(list));
		}

		[TestMethod]
		public void GetStandardDeviationViaArrayOfDoublesWhenValueIsLessThanThree()
		{
			double[] values = new double[] { 4.2, 6.3 };
			Assert.AreEqual(double.NaN, MathUtility.GetStandardDeviation(values));
		}

		[TestMethod]
		public void GetTValueAssumingEqualVariance()
		{
			int degreesFreedom;
			Assert.AreEqual(23.863866062543309d, MathUtility.GetTValueForTwoMeansAssumingEqualVarianceAndUnequalSampleSize(testValues1, testValues2, out degreesFreedom));
			Assert.AreEqual(9, degreesFreedom);

			Assert.AreEqual(23.863866062543309d, MathUtility.GetTValueForTwoMeansAssumingEqualVarianceAndUnequalSampleSize(testValues2, testValues1, out degreesFreedom));
			Assert.AreEqual(9, degreesFreedom);
		}

		[TestMethod]
		public void GetTValueAssumingEqualVarianceWith4DegreesOfFreedom()
		{
			double[] values1 = new double[] { 4.5, 5.6, 7.8, 5.2 };
			double[] values2 = new double[] { 4.2, 6.3 };

			int degreesFreedom;
			Assert.AreEqual(double.NaN, MathUtility.GetTValueForTwoMeansAssumingEqualVarianceAndUnequalSampleSize(values1, values2, out degreesFreedom));
			Assert.AreEqual(4, degreesFreedom);
		}

		[TestMethod]
		public void GetTValueAssumingEqualVarianceWith1DegreesOfFreedom()
		{
			double[] values1 = new double[] { 4.5 };
			double[] values2 = new double[] { 4.2, 6.3 };

			int degreesFreedom;
			Assert.AreEqual(double.NaN, MathUtility.GetTValueForTwoMeansAssumingEqualVarianceAndUnequalSampleSize(values1, values2, out degreesFreedom));
			Assert.AreEqual(1, degreesFreedom);
		}

		[TestMethod]
		public void GetTValueAssumingEqualVarianceWith0DegreesofFreedom()
		{
			double[] values1 = new double[] { 4.5 };
			double[] values2 = new double[] { 4.2 };

			int degreesFreedom;
			Assert.AreEqual(double.NaN, MathUtility.GetTValueForTwoMeansAssumingEqualVarianceAndUnequalSampleSize(values1, values2, out degreesFreedom));
			Assert.AreEqual(0, degreesFreedom);
		}

		[TestMethod]
		public void StandardDeviationOfRatio()
		{
			double apoAverage = MathUtility.GetAverage(calmodulinApo10percent);
			double apoStdev = MathUtility.GetStandardDeviation(calmodulinApo10percent);
			double holoAverage = MathUtility.GetAverage(calmodulinHolo10percent);
			double holoStdev = MathUtility.GetStandardDeviation(calmodulinHolo10percent);

			double ratio = holoAverage / apoAverage;
			double ratioSD = ratio * System.Math.Sqrt(((apoStdev / apoAverage) * (apoStdev / apoAverage)) + ((holoStdev / holoAverage) * (holoStdev / holoAverage)));

			Assert.AreEqual(0.02606646346470055, ratioSD);
		}

		[TestMethod]
		public void TTestOfRatio()
		{
			double apoAverage = MathUtility.GetAverage(calmodulinApo10percent);
			double apoStdev = MathUtility.GetStandardDeviation(calmodulinApo10percent);
			double holoAverage = MathUtility.GetAverage(calmodulinHolo10percent);
			double holoStdev = MathUtility.GetStandardDeviation(calmodulinHolo10percent);

			double ratio = holoAverage / apoAverage;
			double ratioSD = ratio * System.Math.Sqrt(((apoStdev / apoAverage) * (apoStdev / apoAverage)) + ((holoStdev / holoAverage) * (holoStdev / holoAverage)));

			double t = (ratio - 1) / (ratioSD / System.Math.Sqrt(calmodulinApo10percent.GetLength(0)));

			Assert.AreEqual(-62.094156424742458, t);
		}

		private List<double> ConvertToList(double[] values)
		{
			List<double> list = new List<double>();
			foreach (double d in values)
			{
				list.Add(d);
			}
			return list;
		}
	}
}
