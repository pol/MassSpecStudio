using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using Hydra.Core.Domain;
using MassSpecStudio.Core;
using MassSpecStudio.Core.Domain;
using MassSpecStudio.Core.Events;
using MassSpecStudio.Core.Processing;
using Microsoft.Practices.Prism.Events;

namespace Hydra.Processing.Algorithm.Steps
{
	[Export]
	public class IsotopicProfileFinder : ViewModelBase, IProcessingStep
	{
		private readonly OutputEvent _output;
		private double massVariability = 0.1;
		private double peakWidthMinimum = 0.0;
		private double peakWidthMaximum = 0.1;
		private double intensityThreshold = 0;
		private int peakNumberMaximum = 10;
		private PeptideUtility peptideUtility = new PeptideUtility();
		private Hydra.Core.MSPeakSelectionOption _msPeakSelectionOption = Core.MSPeakSelectionOption.ClosestToMzWithinMzVariation;

		// TODO: Move Peptide Utility and MS Utility into constructor to simplify unit tests.
		[ImportingConstructor]
		public IsotopicProfileFinder(IEventAggregator eventAggregator)
		{
			_output = eventAggregator.GetEvent<OutputEvent>();
		}

		[Category("Isotopic Profile Finder")]
		[DisplayName("Mass Variability")]
		public double MassVariability
		{
			get
			{
				return massVariability;
			}

			set
			{
				massVariability = value;
				NotifyPropertyChanged(() => MassVariability);
			}
		}

		[Category("Isotopic Profile Finder")]
		[DisplayName("Peak Width Minimum")]
		public double PeakWidthMinimum
		{
			get
			{
				return peakWidthMinimum;
			}

			set
			{
				peakWidthMinimum = value;
				NotifyPropertyChanged(() => PeakWidthMinimum);
			}
		}

		[Category("Isotopic Profile Finder")]
		[DisplayName("Peak Width Maximum")]
		public double PeakWidthMaximum
		{
			get
			{
				return peakWidthMaximum;
			}

			set
			{
				peakWidthMaximum = value;
				NotifyPropertyChanged(() => PeakWidthMaximum);
			}
		}

		[Category("Isotopic Profile Finder")]
		[DisplayName("Intensity Threshold")]
		public double IntensityThreshold
		{
			get
			{
				return intensityThreshold;
			}

			set
			{
				intensityThreshold = value;
				NotifyPropertyChanged(() => IntensityThreshold);
			}
		}

		[Category("Isotopic Profile Finder")]
		[DisplayName("Peak Number Maximum")]
		public int PeakNumberMaximum
		{
			get
			{
				return peakNumberMaximum;
			}

			set
			{
				peakNumberMaximum = value;
				NotifyPropertyChanged(() => PeakNumberMaximum);
			}
		}

		[Category("Isotopic Profile Finder")]
		[DisplayName("MS Peak Selection Option")]
		public Hydra.Core.MSPeakSelectionOption MSPeakSelectionOption
		{
			get
			{
				return _msPeakSelectionOption;
			}

			set
			{
				_msPeakSelectionOption = value;
				NotifyPropertyChanged(() => MSPeakSelectionOption);
			}
		}

		public ProcessingStep BuildParameterList()
		{
			ProcessingStep step = new ProcessingStep(this);
			step.AddParameter("MassVariability", MassVariability);
			step.AddParameter("PeakWidthMaximum", PeakWidthMaximum);
			step.AddParameter("PeakWidthMinimum", PeakWidthMinimum);
			step.AddParameter("IntensityThreshold", IntensityThreshold);
			step.AddParameter("PeakNumberMaximum", PeakNumberMaximum);
			step.AddParameter("MSPeakSelectionOption", MSPeakSelectionOption);
			return step;
		}

		public void Execute(RunResult result, IList<MSPeak> msPeakList)
		{
			_output.Publish("     Isotopic Profile Finder Started (MassRange=" + MassVariability + ", PeakWidthMinimum=" + PeakWidthMinimum + ", PeakWidthMaximum=" + PeakWidthMaximum + ", IntensityThreshold=" + IntensityThreshold + ", PeakNumberMaximum=" + PeakNumberMaximum + ", MSPeakSelectionOption=" + MSPeakSelectionOption + ")");
			double monoIsotopicMass = result.Peptide.MonoIsotopicMass;
			int chargeState = result.Peptide.ChargeState;

			double massStep = 1 / Convert.ToDouble(chargeState);
			List<double> targetMasses = new List<double>();
			int msPeakIndex = 0;

			// built list of target masses
			string targetedMasses = "     Targeted isotopic peak masses: ";
			for (int i = 0; i < peakNumberMaximum; i++)
			{
				targetMasses.Add(monoIsotopicMass + (massStep * i));
				targetedMasses += targetMasses.Last().ToString() + ", ";
			}
			_output.Publish(targetedMasses);

			result.InitializePeakLists();

			//----------- first will find the monoisotopic peak ----------------------------
			MSPeak monoIsotopicPeak;
			try
			{
				monoIsotopicPeak = MSUtility.GetBestMSPeak(targetMasses[0], msPeakList, msPeakIndex, this.MassVariability, this.PeakWidthMinimum, this.PeakWidthMaximum, this.IntensityThreshold, this.MSPeakSelectionOption, out msPeakIndex);
				if (monoIsotopicPeak != null)
				{
					result.IsotopicPeakList.Add(monoIsotopicPeak);
					_output.Publish("     Monoisotopic Peak Found (MZ=" + monoIsotopicPeak.MZ + ", Intensity=" + monoIsotopicPeak.Intensity + ", " + monoIsotopicPeak.PeakWidth + ")");
				}
			}
			catch (NullReferenceException ex)
			{
				_output.Publish("ERROR: " + ex.Message);
			}
			catch (Exception)
			{
				// Do Nothing
			}

			// means that only the monoisotopic peak was requested
			if (targetMasses.Count < 2)
			{
				////return isotopicProfilePeaklist;
			}

			// start at the second peak of the target masses.  Loop through the target masses
			// and try to find a MS peak for each target mass. If no peak can be found, will break out of the
			// loop and return the MS peak list. 
			_output.Publish("     Identifying Isotopic Peak List:");
			for (int i = 1; i < targetMasses.Count; i++)
			{
				MSPeak selectedMSPeak;
				try
				{
					selectedMSPeak = MSUtility.GetBestMSPeak(targetMasses[i], msPeakList, msPeakIndex, this.MassVariability, this.PeakWidthMinimum, this.PeakWidthMaximum, this.IntensityThreshold, MSPeakSelectionOption, out msPeakIndex);
				}
				catch (Exception ex)
				{
					_output.Publish("ERROR: " + ex.Message);
					break;
				}

				if (selectedMSPeak != null)
				{
					result.IsotopicPeakList.Add(selectedMSPeak);
					_output.Publish("     Peak Found: (MZ=" + selectedMSPeak.MZ + ", Intensity=" + selectedMSPeak.Intensity + ", PeakWidth=" + selectedMSPeak.PeakWidth + ")");
				}
			}
			_output.Publish("     " + Math.Max(0, result.IsotopicPeakList.Count - 1) + " peaks found.");

			if (result.IsResultBasedOnFragment)
			{
				_output.Publish("     Calculating Theoretical Isotopic Peak List (based on fragment sequences)");
				peptideUtility.GetIsotopicProfileForFragmentIon(result, 20);
			}
			else
			{
				_output.Publish("     Calculating Theoretical Isotopic Peak List (based on peptide sequence)");

				// TODO: addProton set to false from true (from meeting with Dave).
				peptideUtility.GetIsotopicProfile(result, false, 20, true, true);
			}

			foreach (MSPeak peak in result.TheoreticalIsotopicPeakList)
			{
				_output.Publish("     Calculated Peak (MZ=" + peak.MZ + ", Intensity=" + peak.Intensity + ", PeakWidth=" + peak.PeakWidth + ")");
			}
			_output.Publish("     " + result.TheoreticalIsotopicPeakList.Count + " peaks found.");
		}
	}
}
