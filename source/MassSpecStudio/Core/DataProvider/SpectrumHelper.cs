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
			IList<double> xValues = spectraToBeAveraged[0].XValues;
			IList<double> sumedYValues = new List<double>(spectraToBeAveraged[0].Count);
			IList<int> numberOfDuplicates = new List<int>(spectraToBeAveraged[0].Count);
			IList<int> currentSearchIndexes = new List<int>(spectraToBeAveraged.Count);

			for (int i = 0; i < xValues.Count; i++)
			{
				for (int j = 0; j < spectraToBeAveraged.Count; j++)
				{
					currentSearchIndexes.Add(0);
					for (int k = currentSearchIndexes[j]; k < spectraToBeAveraged[j].Count; k++)
					{
						currentSearchIndexes[j] = k;
						double difference = spectraToBeAveraged[j].XValues[k] - xValues[i];
						if (difference == 0)
						{
							if (i >= sumedYValues.Count)
							{
								sumedYValues.Add(0);
							}
							sumedYValues[i] += spectraToBeAveraged[j].YValues[k];
							if (i >= numberOfDuplicates.Count)
							{
								numberOfDuplicates.Add(0);
							}
							numberOfDuplicates[i]++;
							break;
						}
						else if (difference > 0)
						{
							break;
						}
					}
				}
			}

			List<XYPoint> sumedPoints = new List<XYPoint>();
			for (int i = 0; i < xValues.Count; i++)
			{
				XYPoint point = new XYPoint(xValues[i], sumedYValues[i]);
				point.NumberOfDuplicates = numberOfDuplicates[i];
				sumedPoints.Add(point);
			}

			return sumedPoints;
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
