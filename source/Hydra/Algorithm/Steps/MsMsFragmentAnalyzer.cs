using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using DeconTools.MassSpecStudio.Processing.Steps;
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
	public class MsMsFragmentAnalyzer : ViewModelBase, IProcessingStep
	{
		private readonly IEventAggregator eventAggregator;
		private readonly OutputEvent output;
		private double massVariability = 0.1;
		private double peakWidthMinimum = 0.0;
		private double peakWidthMaximum = 0.5;
		private double intensityThreshold = 0;
		private int peakNumberMaximum = 20;
		private Hydra.Core.MSPeakSelectionOption _msPeakSelectionOption = Core.MSPeakSelectionOption.MostIntenseWithinMzVariation;
		private double _peakToBackgroundRatio = 2;
		private double _signalToNoiseThreshold = 3;

		// TODO: Add MSUtility to the constructor.
		[ImportingConstructor]
		public MsMsFragmentAnalyzer(IEventAggregator eventAggregator)
		{
			this.eventAggregator = eventAggregator;
			output = eventAggregator.GetEvent<OutputEvent>();
		}

		[Category("Spectral Peak Detection")]
		[DisplayName("Peak To Background Ratio")]
		public double PeakToBackgroundRatio
		{
			get
			{
				return _peakToBackgroundRatio;
			}

			set
			{
				_peakToBackgroundRatio = value;
				NotifyPropertyChanged(() => PeakToBackgroundRatio);
			}
		}

		[Category("Spectral Peak Detection")]
		[DisplayName("Signal To Noise Threshold")]
		public double SignalToNoiseThreshold
		{
			get
			{
				return _signalToNoiseThreshold;
			}

			set
			{
				_signalToNoiseThreshold = value;
				NotifyPropertyChanged(() => SignalToNoiseThreshold);
			}
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
			step.AddParameter("PeakToBackgroundRatio", PeakToBackgroundRatio);
			step.AddParameter("SignalToNoiseThreshold", SignalToNoiseThreshold);
			step.AddParameter("MassVariability", MassVariability);
			step.AddParameter("PeakWidthMinimum", PeakWidthMinimum);
			step.AddParameter("PeakWidthMaximum", PeakWidthMaximum);
			step.AddParameter("IntensityThreshold", IntensityThreshold);
			step.AddParameter("PeakNumberMaximum", PeakNumberMaximum);
			step.AddParameter("MSPeakSelectionOption", MSPeakSelectionOption);
			return step;
		}

		public void Execute(RunResult result, IXYData msmsSpectrum)
		{
			output.Publish("     MSMS Fragment Analyzer");

			PeptideUtility pu = new PeptideUtility();

			if (result.IsResultBasedOnFragment)
			{
				pu.GetIsotopicProfileForFragmentIon(result, 20);
			}
			else
			{
				result.TheoreticalIsotopicPeakList = pu.GetIsotopicProfile(result.Peptide.Sequence, result.Peptide.ChargeState, true, 20, true, true, string.Empty);
			}

			Peptide peptide = new Peptide(result.FragmentIon.Sequence);
			peptide.MonoIsotopicMass = result.FragmentIon.MZ;
			peptide.ChargeState = result.FragmentIon.ChargeState;
			peptide.PeaksInCalculation = result.FragmentIon.PeaksInCalculation;
			peptide.MsThreshold = result.FragmentIon.MsThreshold;
			peptide.DeuteriumDistributionRightPadding = result.FragmentIon.DeutDistRightPadding;
			peptide.DeuteriumDistributionThreshold = result.FragmentIon.DeutDistThreshold;

			RunResult rr = new RunResult(peptide, result.Run);
			rr.IsResultBasedOnFragment = true;
			rr.FragmentIon = result.FragmentIon;
			rr.TheoreticalIsotopicPeakList = result.TheoreticalIsotopicPeakList;

			try
			{
				output.Publish("     Executing Spectral Peak Detection");
				SpectralPeakDetection mspeakDetector = new SpectralPeakDetection(eventAggregator);
				mspeakDetector.PeakToBackgroundRatio = _peakToBackgroundRatio;
				mspeakDetector.SignalToNoiseThreshold = _signalToNoiseThreshold;
				IList<MSPeak> peakList = mspeakDetector.Execute(msmsSpectrum);

				output.Publish("     Executing Isotopic Profile Finder");
				IsotopicProfileFinder profileFinder = new IsotopicProfileFinder(eventAggregator);
				profileFinder.IntensityThreshold = intensityThreshold;
				profileFinder.MassVariability = massVariability;
				profileFinder.MSPeakSelectionOption = _msPeakSelectionOption;
				profileFinder.PeakNumberMaximum = peakNumberMaximum;
				profileFinder.PeakWidthMaximum = peakWidthMaximum;
				profileFinder.PeakWidthMinimum = peakWidthMinimum;
				profileFinder.Execute(rr, peakList);

				output.Publish("     Executing Label Amount Calculator");
				LabelAmountCalculator labelAmountCalc = new LabelAmountCalculator(eventAggregator);
				labelAmountCalc.PeaksInCalcMode = PeaksInLabelCalculationMode.Manual;
				labelAmountCalc.Mode = Mode.CalculatedMassAndExperimentalIntensity;
				labelAmountCalc.Execute(rr);
			}
			catch (Exception ex)
			{
				throw ex;
			}

			result.IsotopicPeakList = rr.IsotopicPeakList;
			output.Publish("     IsotopicPeakList Count=" + result.IsotopicPeakList.Count);
			result.AverageMass = rr.AverageMass;
			output.Publish("     AverageMass=" + result.AverageMass);
			result.TheoreticalAverageMass = MSUtility.GetAverageMassFromPeakList(result.TheoreticalIsotopicPeakList, result.ActualPeaksInCalculation);  // TODO: this is a bandaid solution
			output.Publish("     TheoreticalAverageMass=" + result.TheoreticalAverageMass);
			result.AmountDeut = rr.AmountDeut;
			output.Publish("     AmountDeut=" + result.AmountDeut);
			result.IsUsedInCalculations = rr.IsUsedInCalculations;
			output.Publish("     IsUsedInCalculations=" + result.IsUsedInCalculations);
			result.AmountDeutFromDeutDist = rr.AmountDeutFromDeutDist;
			output.Publish("     AmountDeutFromDeutDist=" + result.AmountDeutFromDeutDist);
		}
	}
}
