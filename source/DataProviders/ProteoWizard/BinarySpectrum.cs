using System.Linq;
using MassSpecStudio.Core.Domain;
using pwiz.CLI.msdata;

namespace ProteoWizard.MassSpecStudio.DataProvider
{
	public class BinarySpectrum : XYBinaryData, ISpectrum
	{
		private double totalIntensity;
		private double mzLower = 0;
		private double mzUpper = double.MaxValue;

		public BinarySpectrum(double rt, BinaryData xValues, BinaryData yValues)
			: this(rt, rt, xValues, yValues)
		{
		}

		public BinarySpectrum(double startRt, double endRt, BinaryData xValues, BinaryData yValues)
			: base(xValues, yValues)
		{
			StartRT = startRt;
			EndRT = endRt;
			totalIntensity = yValues.Sum();
		}

		public BinarySpectrum(double rt, XYBinaryData xyData)
			: this(rt, rt, xyData)
		{
		}

		public BinarySpectrum(double startRt, double endRt, XYBinaryData xyData)
			: this(startRt, endRt, xyData.XValues as BinaryData, xyData.YValues as BinaryData)
		{
		}

		public BinarySpectrum(double startRt, double endRt, BinaryData xValues, BinaryData yValues, double mzLower, double mzUpper)
			: this(startRt, endRt, xValues, yValues)
		{
			this.mzLower = mzLower;
			this.mzUpper = mzUpper;
		}

		public override System.Collections.Generic.IList<double> XValues
		{
			get
			{
				if (mzLower == 0 && mzUpper == double.MaxValue)
				{
					return base.XValues;
				}

				BinaryData binaryData = new BinaryData();
				foreach (double value in base.XValues)
				{
					if (value > mzUpper)
					{
						break;
					}
					if (value >= mzLower)
					{
						binaryData.Add(value);
					}
				}
				return binaryData;
			}
		}

		public override System.Collections.Generic.IList<double> YValues
		{
			get
			{
				if (mzLower == 0 && mzUpper == double.MaxValue)
				{
					return base.YValues;
				}

				BinaryData binaryData = new BinaryData();
				for (int i = 0; i < base.XValues.Count; i++)
				{
					double value = base.XValues[i];
					if (value > mzUpper)
					{
						break;
					}
					if (value >= mzLower)
					{
						binaryData.Add(base.YValues[i]);
					}
				}
				return binaryData;
			}
		}

		public double StartRT { get; private set; }

		public double EndRT { get; private set; }

		public double TotalIntensity
		{
			get { return totalIntensity; }
		}
	}
}
