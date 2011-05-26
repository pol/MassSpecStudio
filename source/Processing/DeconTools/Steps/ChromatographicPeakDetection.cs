using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using MassSpecStudio.Core.Domain;
using MassSpecStudio.Core.Events;
using MassSpecStudio.Core.Processing;
using Microsoft.Practices.Prism.Events;

namespace DeconTools.MassSpecStudio.Processing.Steps
{
	[Export(typeof(IChromatographicPeakDetection))]
	public class ChromatographicPeakDetection : PeakDetectionBase, IChromatographicPeakDetection
	{
		private readonly OutputEvent _output;

		[ImportingConstructor]
		public ChromatographicPeakDetection(IEventAggregator eventAggregator)
			: base(eventAggregator)
		{
			_output = eventAggregator.GetEvent<OutputEvent>();
		}

		[Category("Chromatographic Peak Detection")]
		[DisplayName("Peak To Background Ratio")]
		public new double PeakToBackgroundRatio
		{
			get
			{
				return base.PeakToBackgroundRatio;
			}

			set
			{
				base.PeakToBackgroundRatio = value;
				NotifyPropertyChanged(() => PeakToBackgroundRatio);
			}
		}

		[Category("Chromatographic Peak Detection")]
		[DisplayName("Signal To Noise Threshold")]
		public new double SignalToNoiseThreshold
		{
			get
			{
				return base.SignalToNoiseThreshold;
			}

			set
			{
				base.SignalToNoiseThreshold = value;
				NotifyPropertyChanged(() => SignalToNoiseThreshold);
			}
		}

		public ProcessingStep BuildParameterList()
		{
			ProcessingStep step = new ProcessingStep(this);
			step.AddParameter("PeakToBackgroundRatio", PeakToBackgroundRatio);
			step.AddParameter("SignalToNoiseThreshold", SignalToNoiseThreshold);
			return step;
		}

		public IList<ChromatographicPeak> Execute(IXYData xyData)
		{
			_output.Publish("     Chromatographic Peak Detection Started (PeakToBackgroundRatio=" + PeakToBackgroundRatio + ", SignalToNoiseThreshold=" + SignalToNoiseThreshold + ")");
			Decon2LS.Peaks.clsPeak[] peakList = GetDeconToolsPeaks(xyData);

			IList<ChromatographicPeak> peaks = new List<ChromatographicPeak>();
			foreach (Decon2LS.Peaks.clsPeak peak in peakList)
			{
				peaks.Add(ConvertDeconPeakToChromatographicPeak(peak));
				_output.Publish("     Peak Found: RT=" + peak.mdbl_mz + ", PeakHeight=" + peak.mdbl_intensity + ", PeakWidth=" + peak.mdbl_FWHM + ")");
			}
			_output.Publish("     " + peaks.Count + " peaks found.");
			return peaks;
		}

		private static ChromatographicPeak ConvertDeconPeakToChromatographicPeak(Decon2LS.Peaks.clsPeak peak)
		{
			ChromatographicPeak chromatographicPeak = new ChromatographicPeak();
			chromatographicPeak.PeakArea = 0;
			chromatographicPeak.PeakHeight = peak.mdbl_intensity;
			chromatographicPeak.PeakWidth = peak.mdbl_FWHM;
			chromatographicPeak.Rt = peak.mdbl_mz;
			return chromatographicPeak;
		}
	}
}
