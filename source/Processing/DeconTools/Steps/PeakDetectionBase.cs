using System.Collections.Generic;
using MassSpecStudio.Core;
using MassSpecStudio.Core.Domain;
using MassSpecStudio.Core.Events;
using Microsoft.Practices.Prism.Events;

namespace DeconTools.MassSpecStudio.Processing.Steps
{
	public abstract class PeakDetectionBase : ViewModelBase
	{
		private double _peakToBackgroundRatio = 2;
		private double _signalToNoiseThreshold = 3;

		public PeakDetectionBase(IEventAggregator eventAggregator)
		{
			EventAggregator = eventAggregator;
			Output = EventAggregator.GetEvent<OutputEvent>();
		}

		public double PeakToBackgroundRatio
		{
			get { return _peakToBackgroundRatio; }
			set { _peakToBackgroundRatio = value; }
		}

		public double SignalToNoiseThreshold
		{
			get { return _signalToNoiseThreshold; }
			set { _signalToNoiseThreshold = value; }
		}

		protected IEventAggregator EventAggregator { get; set; }

		protected OutputEvent Output { get; set; }

		protected Decon2LS.Peaks.clsPeak[] GetDeconToolsPeaks(IXYData xyData)
		{
			float[] xData = ConvertDoubleArrayToFloat(xyData.XValues);
			float[] yData = ConvertDoubleArrayToFloat(xyData.YValues);

			Decon2LS.Peaks.clsPeakProcessor peakProcessor = new Decon2LS.Peaks.clsPeakProcessor();
			Decon2LS.Peaks.clsPeakProcessorParameters parameters = new Decon2LS.Peaks.clsPeakProcessorParameters();

			parameters.PeakBackgroundRatio = PeakToBackgroundRatio;
			parameters.SignalToNoiseThreshold = SignalToNoiseThreshold;
			parameters.PeakFitType = Decon2LS.Peaks.PEAK_FIT_TYPE.QUADRATIC;

			peakProcessor.SetOptions(parameters);
			Decon2LS.Peaks.clsPeak[] peakList = new Decon2LS.Peaks.clsPeak[1];
			peakProcessor.DiscoverPeaks(ref xData, ref yData, ref peakList, 0, float.MaxValue);
			return peakList;
		}

		private float[] ConvertDoubleArrayToFloat(IList<double> doubleArray)
		{
			float[] floatArray = new float[doubleArray.Count];
			for (int i = 0; i < doubleArray.Count; i++)
			{
				floatArray[i] = (float)doubleArray[i];
			}
			return floatArray;
		}
	}
}
