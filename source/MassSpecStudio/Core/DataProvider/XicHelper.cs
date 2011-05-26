using System;
using System.Collections.Generic;
using System.Linq;

namespace MassSpecStudio.Core.DataProvider
{
	public static class XicHelper
	{
		public static void GetIntensities(IList<double> mzValues, IList<double> intensityValues, double rt, double mass1, double mzTolerance, List<double> msLevelXVals, List<double> msLevelYVals)
		{
			if (mzValues.Count > 0)
			{
				int indexOfMass = IndexOfMassInSpectrum(mzValues, mass1, mzTolerance);

				if (WasMassFound(indexOfMass))
				{
					indexOfMass = FindIndexOfHighestIntensityWithinTolerance(mzValues, intensityValues, indexOfMass, mass1, mzTolerance);

					if (DoesThisMassBelongWithThePreviousRt(msLevelXVals, rt))
					{
						msLevelYVals[msLevelYVals.Count - 1] += (double)intensityValues[indexOfMass];
					}
					else
					{
						msLevelXVals.Add(rt);
						msLevelYVals.Add((double)intensityValues[indexOfMass]);
					}
				}
				else
				{
					msLevelXVals.Add(rt);
					msLevelYVals.Add(0);
				}
			}
		}

		private static int IndexOfMassInSpectrum(IList<double> mzValues, double mass1, double mzTolerance)
		{
			for (int j = 0; j < mzValues.Count; ++j)
			{
				double d = (double)mzValues[j];
				if (Math.Abs(mass1 - d) <= mzTolerance)
				{
					return j;
				}
			}

			return -1;
		}

		private static bool WasMassFound(int indexOfMass)
		{
			return indexOfMass >= 0;
		}

		private static int FindIndexOfHighestIntensityWithinTolerance(IList<double> mzValues, IList<double> intensityValues, int foundIndex, double mass1, double mzTolerance)
		{
			for (int k = foundIndex; k < mzValues.Count; k++)
			{
				double d = (double)mzValues[k];
				if (Math.Abs(mass1 - mzValues[k]) <= mzTolerance)
				{
					// select index with greatest intensity
					if (intensityValues[k] > intensityValues[foundIndex])
					{
						foundIndex = k;
					}
				}
				else
				{
					break;
				}
			}
			return foundIndex;
		}

		private static bool DoesThisMassBelongWithThePreviousRt(IList<double> xValues, double rt)
		{
			return xValues.Count > 0 && xValues.Last() == rt;
		}
	}
}
