using System;
using System.Collections.Generic;
using MassSpecStudio.Core.Domain;

namespace Hydra.Processing.Algorithm
{
	public static class MSUtility
	{
		/// <summary>
		/// Creates an 'average mass array'. 
		/// Calculates the average mass for peak 1, peak 1+peak2, peak 1+peak2+peak3, ...
		/// </summary>
		/// <param name="peakList">The peak list.</param>
		/// <returns>Return a list of average masses</returns>
		public static IList<double> GetAverageMassList(IList<MSPeak> peakList)
		{
			IList<double> averageMass = new List<double>();

			// calculate total intensity and then make sure it is greater than 0 (to avoid dividing by 0)
			for (int i = 1; i <= peakList.Count; i++)
			{
				averageMass.Add(GetAverageMassFromPeakList(peakList, i));
			}

			return averageMass;
		}

		/// <summary>
		/// Calculates average mass for MS peaks 
		/// </summary>
		/// <param name="peakList">The peak list.</param>
		/// <param name="numberOfPeaksInCalculation">The number of peaks in the calculation</param>
		/// <returns>Returns the average mass from the peak list.</returns>
		public static double GetAverageMassFromPeakList(IList<MSPeak> peakList, int numberOfPeaksInCalculation)
		{
			double totalIntensity = 0;
			double averageMass = 0;

			// calculate total intensity and then make sure it is greater than 0 (to avoid dividing by 0)
			for (int i = 0; i < numberOfPeaksInCalculation && i < peakList.Count; i++)
			{
				totalIntensity += peakList[i].Intensity;
			}

			for (int i = 0; i < numberOfPeaksInCalculation && i < peakList.Count; i++)
			{
				averageMass += (peakList[i].MZ * peakList[i].Intensity) / totalIntensity;
			}
			return averageMass;
		}

		public static double GetIsotopicProfileWidth(IList<MSPeak> msPeakList)
		{
			double width = 0;

			if (msPeakList.Count > 0)
			{
				width = msPeakList[msPeakList.Count - 1].MZ - msPeakList[0].MZ;
			}

			return width;
		}

		public static MSPeak GetMostIntenseMSPeak(IList<MSPeak> msPeakList)
		{
			int maxpeakIndex = 0;
			if (msPeakList.Count == 0)
			{
				return new MSPeak(0, 0, 0);
			}

			for (int i = 0; i < msPeakList.Count; i++)
			{
				if (msPeakList[i].Intensity > msPeakList[maxpeakIndex].Intensity)
				{
					maxpeakIndex = i;
				}
			}

			return msPeakList[maxpeakIndex];
		}

		public static IList<MSPeak> ConvertToRelativeMassIsotopicPeakList(IList<MSPeak> isotopicPeakList, int chargeState, double monoIsotopicMass)
		{
			IList<MSPeak> returnedList = new List<MSPeak>();
			double relativeMZ = monoIsotopicMass;
			double massStep = Math.Abs(1 / Convert.ToDouble(chargeState));

			for (int i = 0; i < isotopicPeakList.Count; i++)
			{
				MSPeak peak = new MSPeak();

				peak.MZ = relativeMZ;
				peak.Intensity = isotopicPeakList[i].Intensity;
				peak.PeakWidth = isotopicPeakList[i].PeakWidth;
				relativeMZ = relativeMZ + massStep;
				returnedList.Add(peak);
			}
			return returnedList;
		}

		public static MSPeak GetBestMSPeak(double targetMZ, IList<MSPeak> msPeakList, int startingIndex, double mzVariability, double peakWidthMin, double peakWidthMax, double intensityThreshold, Hydra.Core.MSPeakSelectionOption msPeakSelectionOption, out int stopIndex)
		{
			// First, collect all ms peaks within mz variability
			IList<MSPeak> withinMassVariabilityMSPeaklist;
			IList<MSPeak> aboveThresholdMSPeaklist;
			IList<MSPeak> withinPeakWidthRequirementsMSPeaklist;

			withinMassVariabilityMSPeaklist = new List<MSPeak>();
			for (int i = startingIndex; i < msPeakList.Count; i++)
			{
				if (System.Math.Abs(msPeakList[i].MZ - targetMZ) <= mzVariability)
				{
					withinMassVariabilityMSPeaklist.Add(msPeakList[i]);

					if (i + 1 != msPeakList.Count)
					{
						for (int j = i + 1; j < msPeakList.Count; j++)
						{
							// if we found a peak and now the variability is 
							// way out, then don't continue iterating over MS peak list
							if (Math.Abs(msPeakList[j].MZ - targetMZ) > mzVariability * 10)
							{
								break;
							}
							if (Math.Abs(msPeakList[j].MZ - targetMZ) <= mzVariability)
							{
								withinMassVariabilityMSPeaklist.Add(msPeakList[j]);
							}
						}
						startingIndex = i + 1;
						break;
					}
				}
			}

			aboveThresholdMSPeaklist = new List<MSPeak>();
			foreach (MSPeak mspeak in withinMassVariabilityMSPeaklist)
			{
				if (mspeak.Intensity >= intensityThreshold)
				{
					aboveThresholdMSPeaklist.Add(mspeak);
				}
			}

			withinPeakWidthRequirementsMSPeaklist = new List<MSPeak>();
			foreach (MSPeak mspeak in aboveThresholdMSPeaklist)
			{
				if (mspeak.PeakWidth >= peakWidthMin && mspeak.PeakWidth <= peakWidthMax)
				{
					withinPeakWidthRequirementsMSPeaklist.Add(mspeak);
				}
			}

			int selectedMSPeakIndex = 0;

			switch (msPeakSelectionOption)
			{
				case Hydra.Core.MSPeakSelectionOption.MostIntenseWithinMzVariation:
					for (int k = 0; k < withinPeakWidthRequirementsMSPeaklist.Count; k++)
					{
						if (withinPeakWidthRequirementsMSPeaklist[k].Intensity > withinPeakWidthRequirementsMSPeaklist[selectedMSPeakIndex].Intensity)
						{
							selectedMSPeakIndex = k;
						}
					}
					break;
				case Hydra.Core.MSPeakSelectionOption.ClosestToMzWithinMzVariation:
					for (int k = 0; k < withinPeakWidthRequirementsMSPeaklist.Count; k++)
					{
						if (System.Math.Abs(withinPeakWidthRequirementsMSPeaklist[k].MZ - targetMZ) < System.Math.Abs(withinPeakWidthRequirementsMSPeaklist[selectedMSPeakIndex].MZ - targetMZ))
						{
							selectedMSPeakIndex = k;
						}
					}
					break;
				default:
					break;
			}

			stopIndex = startingIndex;
			if (selectedMSPeakIndex < withinPeakWidthRequirementsMSPeaklist.Count)
			{
				return withinPeakWidthRequirementsMSPeaklist[selectedMSPeakIndex];
			}
			return null;
		}
	}
}
