using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydra.Processing.Algorithm
{
	public static class MathUtility
	{
		public static double GetAverage(IList<double> values)
		{
			return GetAverage(values.ToArray());
		}

		public static double GetAverage(double[] values)
		{
			if (values == null || values.Length == 0)
			{
				return double.NaN;
			}
			double sum = 0;
			foreach (double val in values)
			{
				sum += val;
			}
			return sum / values.GetLength(0);
		}

		public static double GetStandardDeviation(IList<double> values)
		{
			return GetStandardDeviation(values.ToArray());
		}

		public static double GetStandardDeviation(double[] values)
		{
			if (values.GetLength(0) < 3)
			{
				return double.NaN;
			}

			double average = GetAverage(values);

			double sum = 0;

			foreach (double val in values)
			{
				sum += (average - val) * (average - val);
			}

			return System.Math.Sqrt((sum / (values.GetLength(0) - 1)));
		}

		/// <summary>
		/// Returns the value of t for comparing two means assuming equal variance and unequal sample size.
		/// Reference:  JC Miller and JN Miller, Statistics for Analytical Chemistry (1993), 3rd Edition, p55
		/// </summary>
		/// <param name="array1">First array.</param>
		/// <param name="array2">Second array.</param>
		/// <param name="degreesFreedom">Degrees of freedom.</param>
		/// <returns>Returns the value.</returns>
		public static double GetTValueForTwoMeansAssumingEqualVarianceAndUnequalSampleSize(double[] array1, double[] array2, out int degreesFreedom)
		{
			double avg1 = GetAverage(array1);
			double avg2 = GetAverage(array2);

			double sd1 = GetStandardDeviation(array1);
			double sd2 = GetStandardDeviation(array2);

			double n1 = Convert.ToDouble(array1.Length);
			double n2 = Convert.ToDouble(array2.Length);

			degreesFreedom = array1.Length + array2.Length - 2;

			if (degreesFreedom < 1)
			{
				return double.NaN;
			}

			double pooledStandardDeviation = System.Math.Sqrt((((n1 - 1) * sd1 * sd1) + ((n2 - 1) * sd2 * sd2)) / degreesFreedom);

			double t = (avg1 - avg2) / (pooledStandardDeviation * System.Math.Sqrt((1 / n1) + (1 / n2)));

			return System.Math.Abs(t);
		}
	}
}
