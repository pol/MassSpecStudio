using System;
using System.Collections.Generic;
using Mapack;
using MassSpecStudio.Core.Domain;

namespace Hydra.Processing.Algorithm
{
	public class DeuterationDistributionCalculator
	{
		public DeuterationDistributionCalculator()
		{
		}

		public void CalculateDeuterationDistribution(
			IList<MSPeak> theoreticalMSPeakList,
			IList<MSPeak> msPeakList,
			bool truncateTheoreticalMSPeakListBasedOnRelIntensity,
			bool truncateMSPeaklistBasedOnRelIntensity,
			int leftPadding,
			int rightPadding,
			double theoreticalPeaklistThreshold,
			int theoreticalPeaklistAbsoluteNumberOfPeaks,
			double msPeaklistThreshold,
			int msPeakListAbsoluteNumberOfPeaks,
			out double[] solvedxvals,
			out double[] solvedyvals)
		{
			IList<MSPeak> truncatedPeakList = TruncateMSPeakList(msPeakList, truncateMSPeaklistBasedOnRelIntensity, msPeaklistThreshold, msPeakListAbsoluteNumberOfPeaks);
			IList<MSPeak> truncatedTheoreticalPeakList = TruncateMSPeakList(theoreticalMSPeakList, truncateTheoreticalMSPeakListBasedOnRelIntensity, theoreticalPeaklistThreshold, theoreticalPeaklistAbsoluteNumberOfPeaks);

			int matrixLength = truncatedPeakList.Count + leftPadding + rightPadding;

			Matrix theoreticalMatrix = BuildMatrix(truncatedTheoreticalPeakList, matrixLength, (matrixLength - truncatedTheoreticalPeakList.Count + 1));

			Matrix experimentMatrix = BuildMatrix(truncatedPeakList, matrixLength, 1);
			Matrix result = theoreticalMatrix.Solve(experimentMatrix);

			double[] yvals = new double[result.Rows];
			double[] xvals = new double[result.Rows];

			for (int i = 0; i < result.Rows; i++)
			{
				xvals[i] = i;
				yvals[i] = result[i, 0];
			}

			solvedxvals = xvals;
			solvedyvals = yvals;
		}

		public Matrix BuildMatrix(IList<MSPeak> msPeakList, int matrixLength, int degreesOfFreedom)
		{
			Matrix matrix = new Matrix(matrixLength, degreesOfFreedom);

			double sum = 0;
			foreach (MSPeak peak in msPeakList)
			{
				sum += peak.Intensity;
			}

			List<double> intensityVals = new List<double>();

			for (int i = 0; i < msPeakList.Count; i++)
			{
				intensityVals.Add(msPeakList[i].Intensity / sum);
			}

			for (int i = 0; i < degreesOfFreedom; i++)
			{
				for (int i2 = 0; i2 < intensityVals.Count; i2++)
				{
					matrix[i2 + i, i] = intensityVals[i2];
				}
			}
			return matrix;
		}

		public void DisplayMatrix(string name, Matrix d)
		{
			Console.WriteLine("Matrix: {0}", name);
			for (int x = 0; x < d.Rows; ++x)
			{
				for (int y = 0; y < d.Columns; ++y)
				{
					Console.Write(d[x, y]);
					Console.Write(" ");
				}
				Console.WriteLine();
			}
			Console.WriteLine();
		}

		public double CalculateDeuteriumIncorporationFromDistribution(double[] xvals, double[] yvals)
		{
			double sum = 0;
			for (int i = 0; i < xvals.Length; i++)
			{
				sum += xvals[i] * yvals[i];
			}
			return sum;
		}

		private IList<MSPeak> TruncateMSPeakList(IList<MSPeak> msPeakList, bool truncateBasedOnRelIntensity, double threshold, int absoluteNumberOfPeaks)
		{
			IList<MSPeak> truncatedPeakList = new List<MSPeak>();

			if (truncateBasedOnRelIntensity)
			{
				double maxIntensity = 0;

				foreach (MSPeak peak in msPeakList)
				{
					if (peak.Intensity > maxIntensity)
					{
						maxIntensity = peak.Intensity;
					}
				}

				bool foundPeakAboveThreshold = false;

				foreach (MSPeak peak in msPeakList)
				{
					if (peak.Intensity / maxIntensity * 100 >= threshold)
					{
						foundPeakAboveThreshold = true;
					}

					if (foundPeakAboveThreshold)
					{
						if (peak.Intensity / maxIntensity * 100 >= threshold)
						{
							truncatedPeakList.Add(peak);
						}
						else
						{
							break;
						}
					}
					else
					{
						truncatedPeakList.Add(peak);
					}
				}
			}
			else
			{
				for (int i = 0; i < Math.Min(absoluteNumberOfPeaks, msPeakList.Count); i++)
				{
					truncatedPeakList.Add(msPeakList[i]);
				}
			}

			return truncatedPeakList;
		}
	}
}
