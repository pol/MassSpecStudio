using System.Collections.Generic;
using MassSpecStudio.Core.Domain;

namespace MassSpecStudio.Core.Extensions
{
	public static class XYDataHelper
	{
		public static IList<XYPoint> GetXYPairs(this IXYData xyData)
		{
			IList<XYPoint> xyPairs = new List<XYPoint>();

			for (int i = 0; i < xyData.XValues.Count; i++)
			{
				xyPairs.Add(xyData.GetXYPair(i));
			}
			return xyPairs;
		}
	}
}
