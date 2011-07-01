using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MassSpecStudio.Core;
using MassSpecStudio.Core.Domain;
using pwiz.CLI.msdata;

namespace ProteoWizard.MassSpecStudio.DataProvider
{
	public class XYBinaryData : IXYData, IDisposable
	{
		private BinaryData _xValues;
		private BinaryData _yValues;
		private TimeUnit timeUnit;
		private string _title;

		public XYBinaryData(BinaryData xValues, BinaryData yValues)
		{
			_xValues = xValues;
			_yValues = yValues;
		}

		[DataMember]
		public string Title
		{
			get { return _title; }
			set { _title = value; }
		}

		public virtual IList<double> XValues
		{
			get { return _xValues; }
			private set { _xValues = value as BinaryData; }
		}

		public virtual IList<double> YValues
		{
			get { return _yValues; }
			private set { _yValues = value as BinaryData; }
		}

		public int Count
		{
			get { return _xValues.Count; }
		}

		[DataMember]
		public TimeUnit TimeUnit
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
			for (int i = 0; i < _xValues.Count; i++)
			{
				if (_xValues[i] == x)
				{
					return _yValues[i];
				}
			}
			return 0.0;
		}

		public void Dispose()
		{
			_xValues.Dispose();
			_yValues.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}
