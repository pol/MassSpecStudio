using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using Hydra.Core;
using Hydra.Core.Domain;
using MassSpecStudio.Core;
using MassSpecStudio.Core.Domain;
using MassSpecStudio.Core.Events;
using MassSpecStudio.Core.Processing;
using Microsoft.Practices.Prism.Events;

namespace Hydra.Processing.Algorithm.Steps
{
	[Export]
	public class XicPeakPicker : ViewModelBase, IProcessingStep
	{
		private Hydra.Core.XicPeakPickerOption _xicPeakPickerOption = Hydra.Core.XicPeakPickerOption.MostIntenseWithinRtVariation;
		private OutputEvent _output;

		[ImportingConstructor]
		public XicPeakPicker(IEventAggregator eventAggregator)
		{
			_output = eventAggregator.GetEvent<OutputEvent>();
		}

		[DisplayName("XIC Peak Picker Option")]
		[Category("XIC Peak Picker")]
		public XicPeakPickerOption XicPeakPickerOption
		{
			get
			{
				return _xicPeakPickerOption;
			}

			set
			{
				_xicPeakPickerOption = value;
				NotifyPropertyChanged(() => XicPeakPickerOption);
			}
		}

		public ProcessingStep BuildParameterList()
		{
			ProcessingStep step = new ProcessingStep(this);
			step.AddParameter("XicPeakPickerOption", XicPeakPickerOption);
			return step;
		}

		public ChromatographicPeak Execute(IList<ChromatographicPeak> peakList, Peptide peptide)
		{
			_output.Publish("     XIC Peak Picking Started (XICPeakPickerOption=" + XicPeakPickerOption + ")");

			// this next section might be better suited as a task called 'save Chromatogram xy data'
			double rtVariation = peptide.RtVariance;
			double rtTarget = peptide.RT;

			if (_xicPeakPickerOption == Hydra.Core.XicPeakPickerOption.MostIntenseWithinEntireXic)
			{
				ChromatographicPeak largestPeak = FindLargestPeak(peakList);
				if (largestPeak == null)
				{
					_output.Publish("     No peaks present within the chromatographic peak list, so no peak could be selected.");
					return null;
				}
				_output.Publish("     Largest Peak Found: RT=" + largestPeak.Rt + ", PeakHeight=" + largestPeak.PeakHeight + ", PeakWidth=" + largestPeak.PeakWidth + ")");
				return largestPeak;
			}
			else if (_xicPeakPickerOption == Hydra.Core.XicPeakPickerOption.MostIntenseWithinRtVariation || _xicPeakPickerOption == Hydra.Core.XicPeakPickerOption.ClosestToRTWithinRTVariation)
			{
				List<ChromatographicPeak> chromPeaksWithinRtVariation = GetPeaksWithinRtVariation(peakList, rtTarget - rtVariation, rtTarget + rtVariation);
				if (chromPeaksWithinRtVariation.Count == 0)
				{
					_output.Publish("     No peaks found within RT windows (" + (rtTarget - rtVariation) + " - " + (rtTarget + rtVariation) + ")");
					return null;
				}

				if (_xicPeakPickerOption == Hydra.Core.XicPeakPickerOption.MostIntenseWithinRtVariation)
				{
					ChromatographicPeak largestPeak = FindLargestPeak(chromPeaksWithinRtVariation);
					_output.Publish("     Most Intense Peak Found: RT=" + largestPeak.Rt + ", PeakHeight=" + largestPeak.PeakHeight + ", PeakWidth=" + largestPeak.PeakWidth + ")");
					return largestPeak;
				}
				else if (_xicPeakPickerOption == Hydra.Core.XicPeakPickerOption.ClosestToRTWithinRTVariation)
				{
					ChromatographicPeak closestPeak = GetPeakClosestToRT(chromPeaksWithinRtVariation, rtTarget);
					_output.Publish("     Closest RT Peak Found: RT=" + closestPeak.Rt + ", PeakHeight=" + closestPeak.PeakHeight + ", PeakWidth=" + closestPeak.PeakWidth + ")");
					return closestPeak;
				}
			}
			return null;
		}

		private static ChromatographicPeak FindLargestPeak(IList<ChromatographicPeak> chromPeakList)
		{
			bool foundPeak = false;
			ChromatographicPeak targetPeak = new ChromatographicPeak(0, 0, 0, 0);

			foreach (ChromatographicPeak peak in chromPeakList)
			{
				if (peak.PeakArea > targetPeak.PeakArea)
				{
					targetPeak = peak;
					foundPeak = true;
				}

				// this is a bit of a hack; DeconTools doesn't report a PeakArea so must use peakHeight
				if (peak.PeakArea == 0 && peak.PeakHeight > 0 && peak.PeakWidth > 0)
				{
					if (peak.PeakHeight > targetPeak.PeakHeight)
					{
						targetPeak = peak;
						foundPeak = true;
					}
				}
			}
			if (foundPeak)
			{
				return targetPeak;
			}
			else
			{
				return null;
			}
		}

		private static ChromatographicPeak GetPeakClosestToRT(IList<ChromatographicPeak> chromPeakList, double targetRT)
		{
			bool foundPeak = false;
			ChromatographicPeak targetPeak = new ChromatographicPeak(0, 0, 0, 0);

			foreach (ChromatographicPeak peak in chromPeakList)
			{
				if (Math.Abs(peak.Rt - targetRT) < Math.Abs(targetPeak.Rt - targetRT))
				{
					targetPeak = peak;
					foundPeak = true;
				}
			}
			if (foundPeak)
			{
				return targetPeak;
			}
			else
			{
				return null;
			}
		}

		private static List<ChromatographicPeak> GetPeaksWithinRtVariation(IList<ChromatographicPeak> chromPeakList, double rtLower, double rtUpper)
		{
			List<ChromatographicPeak> peaksWithinVariation = new List<ChromatographicPeak>();

			foreach (ChromatographicPeak peak in chromPeakList)
			{
				if (peak.Rt >= rtLower && peak.Rt <= rtUpper)
				{
					peaksWithinVariation.Add(peak);
				}
			}
			return peaksWithinVariation;
		}
	}
}
