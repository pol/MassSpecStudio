using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace MassSpecStudio.Core.Domain
{
	[DataContract]
	public class XYData : IXYData
	{
		private IList<double> _xValues;
		private IList<double> _yValues;

		private Dictionary<double, XYPoint> _xyPoints;
		private MassSpecStudio.Core.TimeUnit timeUnit;
		private string _title;

		public XYData(IList<double> xValues, IList<double> yValues)
		{
			_xValues = xValues;
			_yValues = yValues;
			_xyPoints = new Dictionary<double, XYPoint>();

			if (_xValues != null)
			{
				for (int i = 0; i < _xValues.Count; i++)
				{
					_xyPoints.Add(XValues[i], new XYPoint(XValues[i], YValues[i]));
				}
			}
		}

		public XYData(List<XYPoint> xyPoints)
		{
			_xyPoints = new Dictionary<double, XYPoint>();

			_xValues = new double[xyPoints.Count];
			_yValues = new double[xyPoints.Count];

			for (int i = 0; i < xyPoints.Count; i++)
			{
				_xValues[i] = xyPoints[i].XValue;
				_yValues[i] = xyPoints[i].YValue;
				_xyPoints.Add(xyPoints[i].XValue, xyPoints[i]);
			}
		}

		[DataMember]
		public string Title
		{
			get { return _title; }
			set { _title = value; }
		}

		[DataMember]
		public IList<double> XValues
		{
			get { return _xValues; }
			private set { _xValues = value; }
		}

		[DataMember]
		public IList<double> YValues
		{
			get { return _yValues; }
			private set { _yValues = value; }
		}

		public int Count
		{
			get { return _xValues.Count; }
		}

		[DataMember]
		public MassSpecStudio.Core.TimeUnit TimeUnit
		{
			get { return timeUnit; }
			set { timeUnit = value; }
		}

		public XYPoint GetXYPair(int index)
		{
			return new XYPoint(_xValues[index], _yValues[index]);
		}

		public double GetYValue(double x)
		{
			if (_xyPoints.Keys.Contains(x))
			{
				return _xyPoints[x].YValue;
			}
			return 0.0;
		}
	}
}
