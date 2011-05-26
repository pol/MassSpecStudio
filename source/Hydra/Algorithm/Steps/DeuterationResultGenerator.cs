using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Hydra.Core.Domain;
using MassSpecStudio.Core.Domain;
using MassSpecStudio.Core.Events;
using MassSpecStudio.Core.Processing;
using Microsoft.Practices.Prism.Events;

namespace Hydra.Processing.Algorithm.Steps
{
	[Export]
	public class DeuterationResultGenerator : IProcessingStep
	{
		private readonly OutputEvent _output;

		[ImportingConstructor]
		public DeuterationResultGenerator(IEventAggregator eventAggregator)
		{
			_output = eventAggregator.GetEvent<OutputEvent>();
		}

		public ProcessingStep BuildParameterList()
		{
			ProcessingStep step = new ProcessingStep(this);
			return step;
		}

		public void Execute(Experiment experiment, Result result)
		{
			_output.Publish("- Calculating Deuteration Results");
			foreach (Peptide peptide in experiment.Peptides.PeptideCollection)
			{
				foreach (ProteinState proteinState in experiment.ProteinStates)
				{
					foreach (Labeling labeling in experiment.Labeling)
					{
						result.DeuterationResults.Add(GenerateDeuterationResult(peptide, proteinState, labeling, experiment, result));
					}
				}
			}
			_output.Publish("- Deuteration Results Calculation Complete");
		}

		private static DeuterationResult GenerateDeuterationResult(Peptide peptide, ProteinState proteinState, Labeling labeling, Experiment experiment, Result result)
		{
			List<RunResult> replicates = new List<RunResult>();

			foreach (Run run in experiment.GetRunsByProteinState(proteinState))
			{
				if (run.Labeling == labeling)
				{
					RunResult runResult = experiment.GetRunResult(result, run, peptide);
					if (runResult != null)
					{
						replicates.Add(runResult);
					}
				}
			}

			DeuterationResult deuterationResult2 = new DeuterationResult(peptide, proteinState, labeling, replicates);
			deuterationResult2.AmountDeut = Math.Round(MathUtility.GetAverage(deuterationResult2.DeuterationValues), 5);
			deuterationResult2.AmountDeuterationStandardDeviation = Math.Round(MathUtility.GetStandardDeviation(deuterationResult2.DeuterationValues), 5);
			deuterationResult2.AmountDeuterationFromDeuterationDistribution = Math.Round(MathUtility.GetAverage(deuterationResult2.DeuterationDistributedDeuterationValues), 5);
			deuterationResult2.AmountDeuterationFromDeuterationDistributionStandardDeviation = Math.Round(MathUtility.GetStandardDeviation(deuterationResult2.DeuterationDistributedDeuterationValues), 5);
			deuterationResult2.CentroidMass = Math.Round(MathUtility.GetAverage(deuterationResult2.CentroidMassValues), 5);
			deuterationResult2.CentroidMassStandardDeviation = Math.Round(MathUtility.GetStandardDeviation(deuterationResult2.CentroidMassValues), 5);
			if (replicates.Count > 0)
			{
				deuterationResult2.TheoreticalCentroidMass = Math.Round(replicates.Average(item => item.TheoreticalAverageMass), 5);
			}

			return deuterationResult2;
		}
	}
}
