using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using MassSpecStudio.Core.Domain;
using MassSpecStudio.Core.Events;
using MassSpecStudio.Core.Processing;
using Microsoft.Practices.Prism.Events;

namespace DeconTools.MassSpecStudio.Processing.Steps
{
	[Export(typeof(ISpectralPeakDetection))]
	public class SpectralPeakDetection : PeakDetectionBase, ISpectralPeakDetection
	{
		private readonly OutputEvent _output;

		[ImportingConstructor]
		public SpectralPeakDetection(IEventAggregator eventAggregator)
			: base(eventAggregator)
		{
			_output = eventAggregator.GetEvent<OutputEvent>();
		}

		[Category("Spectral Peak Detection")]
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

		[Category("Spectral Peak Detection")]
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

		public IList<MSPeak> Execute(IXYData xyData)
		{
			_output.Publish("     Spectral Peak Detection Started (PeakToBackgroundRatio=" + PeakToBackgroundRatio + ", SignalToNoiseThreshold=" + SignalToNoiseThreshold + ")");
			Decon2LS.Peaks.clsPeak[] peakList = GetDeconToolsPeaks(xyData);

			IList<MSPeak> peaks = new List<MSPeak>();
			foreach (Decon2LS.Peaks.clsPeak peak in peakList)
			{
				peaks.Add(ConvertDeconPeakToMSPeak(peak));
				_output.Publish("     Peak Found: MZ=" + peak.mdbl_mz + ", Intensity=" + peak.mdbl_intensity + ", PeakWidth=" + peak.mdbl_FWHM + ")");
			}
			_output.Publish("     " + peaks.Count + " peaks found.");
			return peaks;
		}

		private static MSPeak ConvertDeconPeakToMSPeak(Decon2LS.Peaks.clsPeak deconPeak)
		{
			MSPeak msPeak = new MSPeak();

			msPeak.Intensity = deconPeak.mdbl_intensity;
			msPeak.PeakWidth = deconPeak.mdbl_FWHM;
			msPeak.MZ = deconPeak.mdbl_mz;
			return msPeak;
		}
	}
}

