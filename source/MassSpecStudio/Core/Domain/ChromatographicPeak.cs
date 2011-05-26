using System.Runtime.Serialization;

namespace MassSpecStudio.Core.Domain
{
	[DataContract]
	public class ChromatographicPeak
	{
		private double _rt;
		private double _peakArea;
		private double _peakHeight;
		private double _peakWidth;

		public ChromatographicPeak()
		{
		}

		public ChromatographicPeak(double rt, double peakArea, double peakHeight, double peakWidth)
		{
			this._peakArea = peakArea;
			this._rt = rt;
			this._peakHeight = peakHeight;
			this._peakWidth = peakWidth;
		}

		[DataMember]
		public double Rt
		{
			get { return _rt; }
			set { _rt = value; }
		}

		[DataMember]
		public double PeakArea
		{
			get { return _peakArea; }
			set { _peakArea = value; }
		}

		[DataMember]
		public double PeakHeight
		{
			get { return _peakHeight; }
			set { _peakHeight = value; }
		}

		[DataMember]
		public double PeakWidth
		{
			get { return _peakWidth; }
			set { _peakWidth = value; }
		}
	}
}
