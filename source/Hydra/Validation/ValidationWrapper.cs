using System;
using System.Collections.Generic;
using System.ComponentModel;
using Hydra.Core;
using Hydra.Core.Domain;
using MassSpecStudio.Core.Domain;
using Microsoft.Practices.Prism.ViewModel;

namespace Hydra.Modules.Validation
{
	public enum TheoreticalPeakListOptions
	{
		/// <summary>
		/// Threshold for the peak list
		/// </summary>
		Threshold,

		/// <summary>
		/// Peaks In Calculation
		/// </summary>
		PeaksInCalculation
	}

	public enum MSPeakListOptions
	{
		/// <summary>
		/// Right Pad for the peak list
		/// </summary>
		RighPad,

		/// <summary>
		/// Threshold for the peak list
		/// </summary>
		Threshold,

		/// <summary>
		/// Peaks In Calculation
		/// </summary>
		PeaksInCalculation
	}

	[DefaultProperty("Name")]
	[DisplayName("Result")]
	public class ValidationWrapper : NotificationObject
	{
		private RunResult runResult;
		private bool showTheoreticalValues;
		private bool showDeuteriumDistribution;
		private TheoreticalPeakListOptions deutDistTheoreticalUse;
		private MSPeakListOptions deutDistMSUse;
		private double deutDistTheoreticalThreshold;
		private int deutDistTheoreticalPeaksInCalculation;

		public ValidationWrapper(RunResult runResult)
		{
			this.runResult = runResult;

			DeutDistMSUse = MSPeakListOptions.Threshold;
			deutDistTheoreticalUse = TheoreticalPeakListOptions.Threshold;
			DeutDistTheoreticalThreshold = 1;
			DeutDistTheoreticalPeaksInCalculation = 1;
		}

		[Browsable(false)]
		public RunResult RunResult
		{
			get { return runResult; }
		}

		[Browsable(false)]
		public EventHandler ReprocessingEvent { get; set; }

		[Browsable(false)]
		public EventHandler UpdateEvent { get; set; }

		[Browsable(false)]
		public IXYData CachedXic
		{
			get { return runResult.CachedXic; }
		}

		[Browsable(false)]
		public IXYData CachedSpectrum
		{
			get { return runResult.CachedSpectrum; }
		}

		[Browsable(false)]
		public ChromatographicPeak CachedXicPeak
		{
			get { return runResult.CachedXicPeak; }
		}

		[Browsable(false)]
		public IList<MSPeak> TheoreticalIsotopicPeakList
		{
			get { return runResult.TheoreticalIsotopicPeakList; }
		}

		[Browsable(false)]
		public IList<MSPeak> IsotopicPeakList
		{
			get { return runResult.IsotopicPeakList; }
		}

		[Browsable(false)]
		public IList<ChromatographicPeak> XicPeaks
		{
			get
			{
				return RunResult.CachedXicPeakList;
			}
		}

		[Browsable(false)]
		public ChromatographicPeak SelectedXicPeak
		{
			get
			{
				return RunResult.CachedXicPeak;
			}

			set
			{
				RunResult.CachedXicPeak = value;
				RaisePropertyChanged(() => SelectedXicPeak);
				RaiseReprocessingEvent();
			}
		}

		[Browsable(false)]
		public IList<MSPeak> MSPeaks
		{
			get
			{
				return RunResult.CachedMSPeakList;
			}
		}

		[Browsable(false)]
		public string Title
		{
			get { return runResult.Peptide.Sequence + " - " + runResult.Run.ProteinState.Name + " - " + Name; }
		}

		[Category("Common")]
		public string Name
		{
			get { return runResult.Run.FileName; }
		}

		[Category("Common")]
		[DisplayName("Result Based On Fragment")]
		public bool IsResultBasedOnFragment
		{
			get { return runResult.IsResultBasedOnFragment; }
		}

		[Category("Common")]
		public Peptide Peptide
		{
			get { return runResult.Peptide; }
		}

		[Category("Common")]
		[DisplayName("Fragment Ion")]
		public FragmentIon FragmentIon
		{
			get { return runResult.FragmentIon; }
		}

		[Category("Deuteration - XIC Control")]
		[DisplayName("Actual XIC Adjustment")]
		public double ActualXicAdjustment
		{
			get
			{
				return Math.Round(runResult.ActualXicAdjustment, 3);
			}

			set
			{
				runResult.ActualXicAdjustment = value;
				RaisePropertyChanged(() => ActualXicAdjustment);
				RaiseReprocessingEvent();
			}
		}

		[Category("Deuteration - XIC Control")]
		[DisplayName("Actual XIC Selection Width")]
		public double ActualXicSelectionWidth
		{
			get
			{
				return Math.Round(runResult.ActualXicSelectionWidth, 3);
			}

			set
			{
				runResult.ActualXicSelectionWidth = value;
				RaisePropertyChanged(() => ActualXicSelectionWidth);
				RaiseReprocessingEvent();
			}
		}

		[Category("Deuteration - XIC Control")]
		[DisplayName("Actual Peaks In Calculation")]
		public int ActualPeaksInCalculation
		{
			get
			{
				return runResult.ActualPeaksInCalculation;
			}

			set
			{
				runResult.ActualPeaksInCalculation = value;
				RaisePropertyChanged(() => ActualPeaksInCalculation);
				RaiseReprocessingEvent();
			}
		}

		[Category("Deuteration")]
		[DisplayName("Amount Deuteration")]
		public double AmountDeut
		{
			get { return Math.Round(runResult.AmountDeut, Constants.SignificantDigits); }
		}

		[Category("Deuteration")]
		[DisplayName("Average Mass")]
		public double AverageMass
		{
			get { return Math.Round(runResult.AverageMass, Constants.SignificantDigits); }
		}

		[Category("Common")]
		[DisplayName("Show Theoretical Values")]
		public bool ShowTheoreticalValues
		{
			get
			{
				return showTheoreticalValues;
			}

			set
			{
				showTheoreticalValues = value;
				RaisePropertyChanged(() => ShowTheoreticalValues);
				RaiseUpdateEvent();
			}
		}

		[Category("DeutDist")]
		[DisplayName("Show Deuterium Distribution")]
		public bool ShowDeuteriumDistribution
		{
			get
			{
				return showDeuteriumDistribution;
			}

			set
			{
				showDeuteriumDistribution = value;
				RaisePropertyChanged(() => ShowDeuteriumDistribution);
				RaiseUpdateEvent();
			}
		}

		[ReadOnly(true)]
		[Category("DeutDist")]
		[DisplayName("Amount Deut (From DeutDist)")]
		public double AmountDeutFromDeutDist
		{
			get
			{
				return Math.Round(runResult.AmountDeutFromDeutDist, Constants.SignificantDigits);
			}

			set
			{
				runResult.AmountDeutFromDeutDist = value;
				RaisePropertyChanged(() => AmountDeutFromDeutDist);
			}
		}

		[Category("DeutDist - Theoretical Peak List")]
		[DisplayName("Use")]
		public TheoreticalPeakListOptions DeutDistTheoreticalUse
		{
			get
			{
				return deutDistTheoreticalUse;
			}

			set
			{
				deutDistTheoreticalUse = value;
				RaisePropertyChanged(() => DeutDistTheoreticalUse);
				RaiseUpdateEvent();
			}
		}

		[Category("DeutDist - Theoretical Peak List")]
		[DisplayName("Threshold")]
		public double DeutDistTheoreticalThreshold
		{
			get
			{
				return deutDistTheoreticalThreshold;
			}

			set
			{
				deutDistTheoreticalThreshold = value;
				RaisePropertyChanged(() => DeutDistTheoreticalThreshold);
				RaiseUpdateEvent();
			}
		}

		[Category("DeutDist - Theoretical Peak List")]
		[DisplayName("Peaks In Calculation")]
		public int DeutDistTheoreticalPeaksInCalculation
		{
			get
			{
				return deutDistTheoreticalPeaksInCalculation;
			}

			set
			{
				deutDistTheoreticalPeaksInCalculation = value;
				RaisePropertyChanged(() => DeutDistTheoreticalPeaksInCalculation);
				RaiseUpdateEvent();
			}
		}

		[Category("DeutDist - Select Isotopic Envelope")]
		[DisplayName("Use")]
		public MSPeakListOptions DeutDistMSUse
		{
			get
			{
				return deutDistMSUse;
			}

			set
			{
				deutDistMSUse = value;
				RaisePropertyChanged(() => DeutDistMSUse);
				RaiseUpdateEvent();
			}
		}

		[Category("DeutDist - Select Isotopic Envelope")]
		[DisplayName("Right Padding")]
		public int ActualDeutDistRightPadding
		{
			get
			{
				return runResult.ActualDeutDistRightPadding;
			}

			set
			{
				runResult.ActualDeutDistRightPadding = value;
				RaisePropertyChanged(() => ActualDeutDistRightPadding);
				RaiseUpdateEvent();
			}
		}

		[Category("DeutDist - Select Isotopic Envelope")]
		[DisplayName("Threshold")]
		public double ActualDeutDistThreshold
		{
			get
			{
				return runResult.ActualDeutDistThreshold;
			}

			set
			{
				runResult.ActualDeutDistThreshold = value;
				RaisePropertyChanged(() => ActualDeutDistThreshold);
				RaiseUpdateEvent();
			}
		}

		[Category("DeutDist - Select Isotopic Envelope")]
		[DisplayName("Peaks In Calculation")]
		public int DeutDistMSPeaksInCalculation
		{
			get
			{
				return runResult.ActualPeaksInCalculation;
			}

			set
			{
				runResult.ActualPeaksInCalculation = value;
				RaisePropertyChanged(() => DeutDistMSPeaksInCalculation);
				RaiseUpdateEvent();
			}
		}

		public void UpdateValues()
		{
			RaisePropertyChanged(() => AmountDeut);
			RaisePropertyChanged(() => AverageMass);
		}

		private void RaiseReprocessingEvent()
		{
			if (ReprocessingEvent != null)
			{
				ReprocessingEvent(this, null);
			}
		}

		private void RaiseUpdateEvent()
		{
			if (UpdateEvent != null)
			{
				UpdateEvent(this, null);
			}
		}
	}
}
