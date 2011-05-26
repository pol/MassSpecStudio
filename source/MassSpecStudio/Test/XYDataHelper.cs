using System;
using System.Diagnostics;
using MassSpecStudio.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MassSpecStudio.Test
{
	public static class XYDataHelper
	{
		public static readonly double[] XValueTestSet1 = new double[] { 1, 2, 3 };
		public static readonly double[] YValueTestSet1 = new double[] { 4, 5, 6 };
		public static readonly double[] XValueTestSet2 = new double[] { 7, 8, 9 };
		public static readonly double[] YValueTestSet2 = new double[] { 10, 11, 12 };

		public static IXYData Generate(double[] xValues, double[] yValues)
		{
			IXYData xyData = new XYData(xValues, yValues);
			return xyData;
		}

		public static ISpectrum Generate(double rt, double[] xValues, double[] yValues)
		{
			ISpectrum spectrum = new Spectrum(rt, xValues, yValues);
			return spectrum;
		}

		public static void AssertXYData(IXYData xyData, double[] expectedXValues, double[] expectedYValues)
		{
			Assert.AreEqual(expectedXValues.Length, xyData.XValues.Count);
			Assert.AreEqual(expectedYValues.Length, xyData.YValues.Count);

			for (int i = 0; i < xyData.XValues.Count; i++)
			{
				Assert.AreEqual(expectedXValues[i], xyData.XValues[i]);
				Assert.AreEqual(expectedYValues[i], xyData.YValues[i]);
			}
		}

		public static void AssertValue(IXYData xyData, int index, double expectedXValue, double expectedYValue)
		{
			Assert.AreEqual(expectedXValue, Math.Round(xyData.XValues[index], 10));
			Assert.AreEqual(expectedYValue, Math.Round(xyData.YValues[index], 10));
		}

		public static void AssertSpectrum(ISpectrum spectrum, double expectedRT, int expectedDataPoints, Stopwatch timer, int minimumMillisecondsGenerationShouldTake, int maximumMillisecondsGenerationShouldTake)
		{
			AssertSpectrum(spectrum, expectedRT, expectedRT, expectedDataPoints, timer, minimumMillisecondsGenerationShouldTake, maximumMillisecondsGenerationShouldTake);
		}

		public static void AssertSpectrum(ISpectrum spectrum, double expectedStartRT, double expectedEndRT, int expectedDataPoints, Stopwatch timer, int minimumMillisecondsGenerationShouldTake, int maximumMillisecondsGenerationShouldTake)
		{
			Assert.AreEqual(expectedStartRT, Math.Round(spectrum.StartRT, 10));
			Assert.AreEqual(expectedEndRT, Math.Round(spectrum.EndRT, 10));
			Assert.AreEqual(expectedDataPoints, spectrum.YValues.Count);
			Assert.IsTrue(
				timer.ElapsedMilliseconds >= minimumMillisecondsGenerationShouldTake &&
				timer.ElapsedMilliseconds <= maximumMillisecondsGenerationShouldTake,
				"The Spectrum took " + timer.ElapsedMilliseconds + " which is outside of the acceptable performance range of (" + minimumMillisecondsGenerationShouldTake + " - " + maximumMillisecondsGenerationShouldTake + ")");
		}

		public static void AssertChromatogram(IXYData tic, int expectedDataPoints, Stopwatch timer, int minimumMillisecondsGenerationShouldTake, int maximumMillisecondsGenerationShouldTake)
		{
			Assert.AreEqual(expectedDataPoints, tic.XValues.Count);
			Assert.IsTrue(
				timer.ElapsedMilliseconds >= minimumMillisecondsGenerationShouldTake &&
				timer.ElapsedMilliseconds <= maximumMillisecondsGenerationShouldTake,
				"The TIC took " + timer.ElapsedMilliseconds + " which is outside of the acceptable performance range of (" + minimumMillisecondsGenerationShouldTake + " - " + maximumMillisecondsGenerationShouldTake + ")");
		}
	}
}
