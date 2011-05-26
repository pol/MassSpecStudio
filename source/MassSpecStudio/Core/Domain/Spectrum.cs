using System.Collections.Generic;
using System.Linq;

namespace MassSpecStudio.Core.Domain
{
	public class Spectrum : XYData, ISpectrum
	{
		private double totalIntensity;

		public Spectrum(double rt, double[] xValues, double[] yValues)
			: base(xValues, yValues)
		{
			StartRT = rt;
			EndRT = rt;
			totalIntensity = yValues.Sum();
		}

		public Spectrum(double rt, IList<double> xValues, IList<double> yValues)
			: base(xValues, yValues)
		{
			StartRT = rt;
			EndRT = rt;
			totalIntensity = yValues.Sum();
		}

		public Spectrum(double startRt, double endRt, double[] xValues, double[] yValues)
			: base(xValues, yValues)
		{
			StartRT = startRt;
			EndRT = endRt;
			totalIntensity = yValues.Sum();
		}

		public Spectrum(double startRt, double endRt, IList<double> xValues, IList<double> yValues)
			: base(xValues, yValues)
		{
			StartRT = startRt;
			EndRT = endRt;
			totalIntensity = yValues.Sum();
		}

		public Spectrum(double rt, List<XYPoint> xyPoints)
			: base(xyPoints)
		{
			StartRT = rt;
			EndRT = rt;
			totalIntensity = xyPoints.Sum(point => point.YValue);
		}

		public Spectrum(double startRt, double endRt, List<XYPoint> xyPoints)
			: base(xyPoints)
		{
			StartRT = startRt;
			EndRT = endRt;
			totalIntensity = xyPoints.Sum(point => point.YValue);
		}

		public double StartRT { get; private set; }

		public double EndRT { get; private set; }

		public double TotalIntensity
		{
			get { return totalIntensity; }
		}
	}
}
