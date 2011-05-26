using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using Hydra.Core.Domain;
using MassSpecStudio.Core;
using MassSpecStudio.Core.Domain;
using MassSpecStudio.Core.Events;
using MassSpecStudio.Core.Processing;
using Microsoft.Practices.Prism.Events;

namespace Hydra.Processing.Algorithm.Steps
{
	public enum Mode
	{
		/// <summary>
		/// Experimental mass and intensity
		/// </summary>
		ExperimentalMassAndIntensity,

		/// <summary>
		/// Calculated mass and experimental intensity
		/// </summary>
		CalculatedMassAndExperimentalIntensity
	}

	[Export]
	public class LabelAmountCalculator : ViewModelBase, IProcessingStep
	{
		private readonly OutputEvent _output;
		private Mode mode;
		private Hydra.Core.PeaksInLabelCalculationMode peaksInCalcMode;

		// TODO: Add MSUtility to the constructor.
		[ImportingConstructor]
		public LabelAmountCalculator(IEventAggregator eventAggregator)
		{
			_output = eventAggregator.GetEvent<OutputEvent>();
			mode = Mode.ExperimentalMassAndIntensity;
		}

		[DisplayName("Mode")]
		[Category("Label Amount Calculator")]
		public Mode Mode
		{
			get
			{
				return mode;
			}

			set
			{
				mode = value;
				NotifyPropertyChanged(() => Mode);
			}
		}

		[DisplayName("Peaks In Calculation Mode")]
		[Category("Label Amount Calculator")]
		public Hydra.Core.PeaksInLabelCalculationMode PeaksInCalcMode
		{
			get
			{
				return peaksInCalcMode;
			}

			set
			{
				peaksInCalcMode = value;
				NotifyPropertyChanged(() => PeaksInCalcMode);
			}
		}

		public ProcessingStep BuildParameterList()
		{
			ProcessingStep step = new ProcessingStep(this);
			step.AddParameter("Mode", Mode);
			step.AddParameter("PeaksInCalcMode", PeaksInCalcMode);
			return step;
		}

		public void Execute(RunResult result)
		{
			_output.Publish("     Label Amount Calculator Started (Mode=" + Mode + ", PeaksInCalcMode=" + PeaksInCalcMode + ")");

			if (this.PeaksInCalcMode == Core.PeaksInLabelCalculationMode.Automatic)
			{
				result.ActualPeaksInCalculation = CalculatePeaksInCalculation(result);
				_output.Publish("     Number of Peaks In Label Calculation: " + result.ActualPeaksInCalculation + " (Automatic)");
			}
			else
			{
				_output.Publish("     Number of Peaks In Label Calculation: " + result.ActualPeaksInCalculation + " (Manual)");
			}

			if (this.mode == Mode.CalculatedMassAndExperimentalIntensity)
			{
				IList<MSPeak> relIsotopicPeakList = MSUtility.ConvertToRelativeMassIsotopicPeakList(result.IsotopicPeakList, result.Peptide.ChargeState, 0);
				IList<MSPeak> relTheoreticalIsotopicPeakList = MSUtility.ConvertToRelativeMassIsotopicPeakList(result.TheoreticalIsotopicPeakList, result.Peptide.ChargeState, 0);

				result.AverageMass = result.Peptide.MonoIsotopicMass + MSUtility.GetAverageMassFromPeakList(relIsotopicPeakList, result.ActualPeaksInCalculation);
				result.TheoreticalAverageMass = MSUtility.GetAverageMassFromPeakList(result.TheoreticalIsotopicPeakList, result.ActualPeaksInCalculation);
			}
			else
			{
				result.AverageMass = MSUtility.GetAverageMassFromPeakList(result.IsotopicPeakList, result.ActualPeaksInCalculation);
				result.TheoreticalAverageMass = MSUtility.GetAverageMassFromPeakList(result.TheoreticalIsotopicPeakList, result.ActualPeaksInCalculation);
			}

			_output.Publish("     Calculated Average Mass=" + result.AverageMass);
			_output.Publish("     Calculated Theoretical Average Mass=" + result.TheoreticalAverageMass);

			// get mass of deuterated and theoretical peptides
			double centroidMr = PeptideUtility.CalculateMR(result.AverageMass, result.Peptide.ChargeState);
			double theoreticalCentroidMr = PeptideUtility.CalculateMR(result.TheoreticalAverageMass, result.Peptide.ChargeState);
			result.CentroidMR = centroidMr;
			result.TheoreticalCentroidMR = theoreticalCentroidMr;
			_output.Publish("     Calculated Centroid MR=" + result.CentroidMR);
			_output.Publish("     Calculated Theoretical Centroid MR=" + result.TheoreticalCentroidMR);

			result.AmideHydrogenTotal = PeptideUtility.GetNumberOfAmideHydrogens(result.Peptide.Sequence);
			_output.Publish("     Calculated Amide Hydrogen Total=" + result.AmideHydrogenTotal);

			// calculate amount of deuteration
			result.AmountDeut = (result.AverageMass - result.TheoreticalAverageMass) * result.Peptide.ChargeState;
			_output.Publish("     Calculated Amount Deuteration=" + result.AmountDeut + " [AvgMass(" + result.AverageMass + ") - TheoAvgMass(" + result.TheoreticalAverageMass + "] * ChargeState(" + result.Peptide.ChargeState + ")");
			result.IsUsedInCalculations = result.AverageMass > 0;
		}

		private static int CalculatePeaksInCalculation(RunResult result)
		{
			// determine max intensity
			MSPeak mostIntensePeak = new MSPeak();

			foreach (MSPeak peak in result.IsotopicPeakList)
			{
				if (peak.Intensity > mostIntensePeak.Intensity)
				{
					mostIntensePeak = peak;
				}
			}

			bool foundPeakaboveThreshold = false;
			int returnedIndex = 0;

			for (int i = 0; i < result.IsotopicPeakList.Count; i++)
			{
				// start at Mono
				// add peaks if they are above the deutDistThreshold
				// Stop adding peaks if they are below the deutDistThreshold
				if (result.IsotopicPeakList[i].Intensity / mostIntensePeak.Intensity * 100 > result.ActualDeutDistThreshold)
				{
					foundPeakaboveThreshold = true;
				}

				if (foundPeakaboveThreshold && result.IsotopicPeakList[i].Intensity / mostIntensePeak.Intensity * 100 < result.ActualDeutDistThreshold)
				{
					returnedIndex = i;
					break;
				}
				returnedIndex = i;
			}

			return returnedIndex;
		}
	}
}
