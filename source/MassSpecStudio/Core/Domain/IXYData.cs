using System.Collections.Generic;

namespace MassSpecStudio.Core.Domain
{
	public interface IXYData
	{
		string Title { get; set; }

		IList<double> XValues { get; }

		IList<double> YValues { get; }

		int Count { get; }

		MassSpecStudio.Core.TimeUnit TimeUnit { get; set; }

		XYPoint GetXYPair(int index);

		double GetYValue(double x);
	}
}
