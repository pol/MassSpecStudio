using System;
using System.Collections.Generic;
using Hydra.Processing.Algorithm;
using MassSpecStudio.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hydra.Test.Processing.Algorithm
{
	[TestClass]
	public class MSUtilityTest
	{
		[TestMethod]
		public void GetAverageMassList()
		{
			IList<MSPeak> msPeakList = GenerateTestPeakList();

			IList<double> averageMassList = MSUtility.GetAverageMassList(msPeakList);

			Assert.AreEqual(averageMassList.Count, msPeakList.Count);
			Assert.AreEqual(averageMassList[0], 500);
			Assert.AreEqual(averageMassList[1], 500.33333333333331);
			Assert.AreEqual(averageMassList[2], 500.4375);
			Assert.AreEqual(averageMassList[3], 500.45341614906835);
		}

		[TestMethod]
		public void GetAverageMassListWithNoPeaks()
		{
			IList<double> averageMassList = MSUtility.GetAverageMassList(new List<MSPeak>());

			Assert.AreEqual(0, averageMassList.Count);
		}

		[TestMethod]
		public void GetAverageMassFromPeakList()
		{
			double averageMass = MSUtility.GetAverageMassFromPeakList(GenerateTestPeakList(), 3);

			Assert.AreEqual(500.4375, averageMass);
		}

		[TestMethod]
		public void GetAverageMassFromPeakListWithSinglePeaksInCalc()
		{
			double averageMass = MSUtility.GetAverageMassFromPeakList(GenerateTestPeakList(), 1);

			Assert.AreEqual(500, averageMass);
		}

		[TestMethod]
		public void GetAverageMassFromPeakListWithNoPeaks()
		{
			IList<MSPeak> msPeakList = new List<MSPeak>();

			Assert.AreEqual(0, MSUtility.GetAverageMassFromPeakList(msPeakList, 2));
		}

		[TestMethod]
		public void GetAverageMassFromPeakListWithNoPeaksInCalc()
		{
			double averageMass = MSUtility.GetAverageMassFromPeakList(GenerateTestPeakList(), 0);

			Assert.AreEqual(0, averageMass);
		}

		[TestMethod]
		public void GetIsotopicProfileWidth()
		{
			Assert.AreEqual(3, MSUtility.GetIsotopicProfileWidth(GenerateTestPeakList()));
		}

		[TestMethod]
		public void GetIsotopicProfileWidthWithNoPeaks()
		{
			Assert.AreEqual(0, MSUtility.GetIsotopicProfileWidth(new List<MSPeak>()));
		}

		[TestMethod]
		public void GetMostIntenseMSPeak()
		{
			Assert.AreEqual(500, MSUtility.GetMostIntenseMSPeak(GenerateTestPeakList()).MZ);
		}

		[TestMethod]
		public void GetMostIntenseMSPeakWithNoPeaks()
		{
			Assert.AreEqual(0, MSUtility.GetMostIntenseMSPeak(new List<MSPeak>()).MZ);
		}

		[TestMethod]
		public void ConvertToRelativeMassIsotopicPeakList()
		{
			IList<MSPeak> peaks = MSUtility.ConvertToRelativeMassIsotopicPeakList(GenerateTestPeakList(), 2, 200);

			Assert.AreEqual(4, peaks.Count);
			AssertMSPeak(peaks[0], 200, 100, 1);
			AssertMSPeak(peaks[1], 200.5, 50, 1);
			AssertMSPeak(peaks[2], 201, 10, 1);
			AssertMSPeak(peaks[3], 201.5, 1, 1);
		}

		[TestMethod]
		public void ConvertToRelativeMassIsotopicPeakListWithSingleChargeState()
		{
			IList<MSPeak> peaks = MSUtility.ConvertToRelativeMassIsotopicPeakList(GenerateTestPeakList(), 1, 200);

			Assert.AreEqual(4, peaks.Count);
			AssertMSPeak(peaks[0], 200, 100, 1);
			AssertMSPeak(peaks[1], 201, 50, 1);
			AssertMSPeak(peaks[2], 202, 10, 1);
			AssertMSPeak(peaks[3], 203, 1, 1);
		}

		[TestMethod]
		public void ConvertToRelativeMassIsotopicPeakListWithNoPeaks()
		{
			IList<MSPeak> peaks = MSUtility.ConvertToRelativeMassIsotopicPeakList(new List<MSPeak>(), 1, 200);

			Assert.AreEqual(0, peaks.Count);
		}

		[TestMethod]
		public void GetBestMSPeakWhenNoPeaks()
		{
			int stopIndex;
			MSPeak peak = MSUtility.GetBestMSPeak(200, new List<MSPeak>(), 0, 0.5, 0, 1, 0, Hydra.Core.MSPeakSelectionOption.MostIntenseWithinMzVariation, out stopIndex);

			Assert.AreEqual(0, stopIndex);
			Assert.AreEqual(null, peak);
		}

		[TestMethod]
		public void GetBestMSPeakMostIntenseWithinMzVariation()
		{
			int stopIndex;
			MSPeak peak = MSUtility.GetBestMSPeak(501.25, GenerateTestPeakList(), 0, 1.5, 0, 1, 0, Hydra.Core.MSPeakSelectionOption.MostIntenseWithinMzVariation, out stopIndex);

			Assert.AreEqual(1, stopIndex);
			Assert.AreEqual(500, peak.MZ);
		}

		[TestMethod]
		public void GetBestMSPeakMostIntenseWithinMzVariationWhenNoPeaksInVariation()
		{
			int stopIndex;
			MSPeak peak = MSUtility.GetBestMSPeak(501.25, GenerateTestPeakList(), 0, 0.2, 0, 1, 0, Hydra.Core.MSPeakSelectionOption.MostIntenseWithinMzVariation, out stopIndex);

			Assert.AreEqual(0, stopIndex);
			Assert.AreEqual(null, peak);
		}

		[TestMethod]
		public void GetBestMSPeakClosestToMzWithinMzVariation()
		{
			int stopIndex;
			MSPeak peak = MSUtility.GetBestMSPeak(501.25, GenerateTestPeakList(), 0, 1.5, 0, 1, 0, Hydra.Core.MSPeakSelectionOption.ClosestToMzWithinMzVariation, out stopIndex);

			Assert.AreEqual(1, stopIndex);
			Assert.AreEqual(501, peak.MZ);
		}

		[TestMethod]
		public void GetBestMSPeakClosestToMzWithinMzVariationWhenNoPeaksInVariation()
		{
			int stopIndex;
			MSPeak peak = MSUtility.GetBestMSPeak(501.25, GenerateTestPeakList(), 0, 0.2, 0, 1, 0, Hydra.Core.MSPeakSelectionOption.ClosestToMzWithinMzVariation, out stopIndex);

			Assert.AreEqual(0, stopIndex);
			Assert.AreEqual(null, peak);
		}

		[TestMethod]
		public void GetBestMSPeakMostIntenseWithinMzVariationWhenNoPeaksFallWithinIntensityThreshold()
		{
			int stopIndex;
			MSPeak peak = MSUtility.GetBestMSPeak(501.25, GenerateTestPeakList(), 0, 1.5, 0, 1, 101, Hydra.Core.MSPeakSelectionOption.MostIntenseWithinMzVariation, out stopIndex);

			Assert.AreEqual(1, stopIndex);
			Assert.AreEqual(null, peak);
		}

		[TestMethod]
		public void GetBestMSPeakClosestToMzWithinMzVariationWhenNoPeaksFallWithinIntensityThreshold()
		{
			int stopIndex;
			MSPeak peak = MSUtility.GetBestMSPeak(501.25, GenerateTestPeakList(), 0, 1.5, 0, 1, 51, Hydra.Core.MSPeakSelectionOption.ClosestToMzWithinMzVariation, out stopIndex);

			Assert.AreEqual(1, stopIndex);
			Assert.AreEqual(500, peak.MZ);
		}

		[TestMethod]
		public void GetBestMSPeakWhenNoPeaksFallWithinPeakWidthRange()
		{
			int stopIndex;
			MSPeak peak = MSUtility.GetBestMSPeak(501.25, GenerateTestPeakList(), 0, 1.5, 2, 3, 0, Hydra.Core.MSPeakSelectionOption.ClosestToMzWithinMzVariation, out stopIndex);

			Assert.AreEqual(1, stopIndex);
			Assert.AreEqual(null, peak);
		}

		private IList<MSPeak> GenerateTestPeakList()
		{
			IList<MSPeak> msPeaklist = new List<MSPeak>();
			MSPeak mspeak1 = new MSPeak(500, 100, 1);
			MSPeak mspeak2 = new MSPeak(501, 50, 1);
			MSPeak mspeak3 = new MSPeak(502, 10, 1);
			MSPeak mspeak4 = new MSPeak(503, 1, 1);

			msPeaklist.Add(mspeak1);
			msPeaklist.Add(mspeak2);
			msPeaklist.Add(mspeak3);
			msPeaklist.Add(mspeak4);

			return msPeaklist;
		}

		private void AssertMSPeak(MSPeak peak, double expectedMz, double expectedIntensity, double expectedPeakWidth)
		{
			Assert.AreEqual(expectedMz, peak.MZ);
			Assert.AreEqual(Math.Round(expectedIntensity, 5), Math.Round(peak.Intensity, 5));
			Assert.AreEqual(Math.Round(expectedPeakWidth, 5), Math.Round(peak.PeakWidth, 5));
		}
	}
}
