using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MassSpecStudio.Core.Domain;

namespace MassSpecStudio.Core.DataProvider
{
	public static class SpectrumHelper
	{
		public static ISpectrum CreateAveragedSpectrum(List<ISpectrum> spectraToBeAveraged)
		{
			if (spectraToBeAveraged.Count > 0)
			{
				if (IsOnlyOneSpectrumToBeAveraged(spectraToBeAveraged))
				{
					return spectraToBeAveraged[0];
				}

				List<XYPoint> sumedXYData = SumIntensities(spectraToBeAveraged);
				SortSumedXYData(sumedXYData);
				return new Domain.Spectrum(
					spectraToBeAveraged.First().StartRT,
					spectraToBeAveraged.Last().StartRT,
					CalculateAverageIntensities(sumedXYData));
			}

			return new Domain.Spectrum(0, new List<XYPoint>());
		}

		private static List<XYPoint> SumIntensities(List<Domain.ISpectrum> spectraToBeAveraged)
		{
			Dictionary<double, XYPoint> all = new Dictionary<double, XYPoint>();

			for (int j = 0; j < spectraToBeAveraged.Count; j++)
			{
				for (int k = 0; k < spectraToBeAveraged[j].Count; k++)
				{
					XYPoint point = spectraToBeAveraged[j].GetXYPair(k);

					ApplyThreshold(point);

					if (all.Keys.Contains(point.XValue))
					{
						all[point.XValue].YValue += point.YValue;
						all[point.XValue].NumberOfDuplicates++;
					}
					else
					{
						point.NumberOfDuplicates = 1;
						all.Add(point.XValue, point);
					}
				}
			}

			List<XYPoint> sumedPoints = new List<XYPoint>();
			IList<double> keys = all.Keys.ToList();
			for (int i = 0; i < keys.Count; i++)
			{
				sumedPoints.Add(all[keys[i]]);
			}

			return sumedPoints;
		}

		private static void ApplyThreshold(XYPoint point)
		{
			point.XValue = Math.Round(point.XValue, 3);
		}

		private static bool IsOnlyOneSpectrumToBeAveraged(List<Domain.ISpectrum> spectraToBeAveraged)
		{
			return spectraToBeAveraged.Count == 1;
		}

		private static void SortSumedXYData(List<XYPoint> sumedXYData)
		{
			sumedXYData.Sort(
						delegate(XYPoint p1, XYPoint p2)
						{
							return p1.XValue.CompareTo(p2.XValue);
						});
		}

		private static List<XYPoint> CalculateAverageIntensities(List<XYPoint> sumedXYData)
		{
			List<XYPoint> averagedXYData = sumedXYData;
			for (int i = 0; i < sumedXYData.Count; i++)
			{
				if (sumedXYData[i].NumberOfDuplicates > 0)
				{
					averagedXYData[i].YValue = sumedXYData[i].YValue / ((double)sumedXYData[i].NumberOfDuplicates);
				}
			}
			return averagedXYData;
		}
	}
}
