using System.Runtime.Serialization;

namespace MassSpecStudio.Core.Domain
{
	[DataContract]
	public class XYPoint
	{
		public XYPoint(double xValue, double yValue)
		{
			XValue = xValue;
			YValue = yValue;
		}

		[DataMember]
		public double XValue { get; set; }

		[DataMember]
		public double YValue { get; set; }

		[DataMember]
		public int NumberOfDuplicates { get; set; }
	}
}
