using System.Runtime.Serialization;

namespace MassSpecStudio.Core.Domain
{
	[DataContract]
	public class MSPeak
	{
		private double _mz;
		private double _peakWidth;
		private double _intensity;

		public MSPeak()
		{
		}

		public MSPeak(double mz, double intensity, double peakWidth)
		{
			this._mz = mz;
			this._peakWidth = peakWidth;
			this._intensity = intensity;
		}

		[DataMember]
		public double MZ
		{
			get { return _mz; }
			set { _mz = value; }
		}

		[DataMember]
		public double PeakWidth
		{
			get { return _peakWidth; }
			set { _peakWidth = value; }
		}

		[DataMember]
		public double Intensity
		{
			get { return _intensity; }
			set { _intensity = value; }
		}
	}
}
