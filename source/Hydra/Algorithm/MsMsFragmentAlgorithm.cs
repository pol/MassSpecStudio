using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Hydra.Core.Domain;
using Hydra.Core.Events;
using Hydra.Processing.Algorithm.Steps;
using MassSpecStudio.Core;
using MassSpecStudio.Core.Domain;
using MassSpecStudio.Core.Events;
using MassSpecStudio.Core.Processing;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;

namespace Hydra.Processing.Algorithm
{
	[Export(typeof(IAlgorithm))]
	public class MsMsFragmentAlgorithm : AlgorithmBase, IAlgorithm
	{
		private readonly IServiceLocator _serviceLocator;
		private int currentProgress = 1;
		private int progressFinish = 0;
		private string _currentMessage;
		private IMsMsSpectrumSelection msmsSpectrumSelection;
		private ISmoothing _smoothing;
		private MsMsFragmentAnalyzer msmsFragmentAnalyzer;
		private DeuterationResultGenerator deuterationResultGenerator;
		private IEventAggregator _eventAggregator;

		[ImportingConstructor]
		public MsMsFragmentAlgorithm(
			IEventAggregator eventAggregator,
			IMsMsSpectrumSelection msmsSpectrumSelection,
			ISmoothing smoothing,
			MsMsFragmentAnalyzer msmsFragmentAnalyzer,
			DeuterationResultGenerator deuterationResultGenerator,
			IServiceLocator serviceLocator)
		{
			_eventAggregator = eventAggregator;
			this.deuterationResultGenerator = deuterationResultGenerator;
			this.msmsSpectrumSelection = msmsSpectrumSelection;
			_smoothing = smoothing;
			this.msmsFragmentAnalyzer = msmsFragmentAnalyzer;
			_serviceLocator = serviceLocator;

			ProcessingSteps.Add(msmsSpectrumSelection);
			ProcessingSteps.Add(_smoothing);
			ProcessingSteps.Add(msmsFragmentAnalyzer);
		}

		public override string Name
		{
			get { return "MS/MS Fragment Analyzer"; }
		}

		public override void Execute(BackgroundWorker worker, ExperimentBase experimentBase)
		{
			currentProgress = 1;
			System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
			stopWatch.Start();
			Experiment experiment = experimentBase as Experiment;
			_eventAggregator.GetEvent<ClearOutputEvent>().Publish(null);
			_eventAggregator.GetEvent<OutputEvent>().Publish("------ Processing Started (" + DateTime.Now.ToString() + ") ------ " + DateTime.Now);
			OutputEvent output = _eventAggregator.GetEvent<OutputEvent>();

			progressFinish = experiment.Runs.Count * experiment.Peptides.PeptideCollection.Count * ProcessingSteps.Count;

			Result result = new Result("Result (" + DateTime.Now.ToString() + ")", (Experiment)experimentBase);
			try
			{
				for (int j = 0; j < experiment.ProteinStates.Count; ++j)
				{
					ProteinState proteinState = experiment.ProteinStates[j];
					OutputText("-PROTEIN STATE '" + proteinState.Name + "'");

					IList<Run> runs = experiment.GetRunsByProteinState(proteinState);
					for (int i = 0; i < runs.Count; i++)
					{
						Run run = runs[i];
						OutputText("-RUN '" + run.FileName + "'");

						// TODO: crashed here when I closed the progress bar; seems to be a threading issue
						foreach (Peptide peptide in experiment.Peptides.PeptideCollection)
						{
							OutputText("-PEPTIDE '" + peptide.Sequence + "'");
							ReportProgress(worker, "Processing " + Path.GetFileName(run.FileName) + " (" + proteinState.Name + ")");

							ReportStepProgress(worker, " 1: MSMS Spectrum Selection", "MSMS Spectrum Selection");
							IXYData msmsSpectrum = msmsSpectrumSelection.Execute(run, peptide.RT, peptide.MonoIsotopicMass);

							ReportStepProgress(worker, " 2: MSMS Smoothing", "MSMS Smoothing");
							msmsSpectrum = _smoothing.Execute(msmsSpectrum);

							ReportStepProgress(worker, " 3: MSMS Fragment Analyzer", "MSMS Fragment Analyzer");
							foreach (FragmentIon fragmentIon in peptide.FragmentIonList)
							{
								RunResult runResult = new RunResult(fragmentIon, run);
								runResult.CachedSpectrum = msmsSpectrum;
								try
								{
									msmsFragmentAnalyzer.Execute(runResult, msmsSpectrum);
								}
								catch (Exception ex)
								{
									OutputText(" ERROR: '" + ex.Message + "'");
									PeptideUtility pu = new PeptideUtility();
									pu.GetIsotopicProfileForFragmentIon(runResult, 20);
									runResult.TheoreticalAverageMass = MSUtility.GetAverageMassFromPeakList(runResult.TheoreticalIsotopicPeakList, runResult.ActualPeaksInCalculation);
								}
								result.RunResults.Add(runResult);
							}

							currentProgress++;
						}

						run.SetDataProvider(_serviceLocator.GetAllInstances<IDataProvider>().Where(item => item.GetType().Name.Contains("Proteo")).First());
					}
				}

				ReportProgress(worker, "Generating Deuteration Results");
				deuterationResultGenerator.Execute(experiment, result);

				experiment.IsProcessed = true;

				experiment.Results.Add(result);

				stopWatch.Stop();
				OutputText("------ Processing Completed (" + DateTime.Now.ToString() + ") - Duration=" + stopWatch.Elapsed.ToString() + " ------");
			}
			catch (Exception ex)
			{
				stopWatch.Stop();
				OutputText("------- ERROR: Processing Failed (" + DateTime.Now.ToString() + ") - Duration=" + stopWatch.Elapsed.ToString() + " ------");
				OutputText(ex.Message);
				OutputText(ex.StackTrace);
				if (ex.InnerException != null)
				{
					OutputText(ex.InnerException.Message);
					OutputText(ex.StackTrace);
				}
				_eventAggregator.GetEvent<StatusUpdateEvent>().Publish("Processing Failed");
				return;
			}

			result.AlgorithmUsed = new MassSpecStudio.Core.Domain.Algorithm(this);
			RecentAlgorithms.Write(result.AlgorithmUsed);

			// Publish event with new result so we can create a ResultViewModel for it and display the results.
			_eventAggregator.GetEvent<ResultAddedEvent>().Publish(result);
			_eventAggregator.GetEvent<ViewResultsEvent>().Publish(result);
			_eventAggregator.GetEvent<StatusUpdateEvent>().Publish("Processing Succeeded");
		}

		private void ReportProgress(BackgroundWorker worker, string message)
		{
			_currentMessage = message;
			worker.ReportProgress(Convert.ToInt32(((double)currentProgress / (double)progressFinish) * 100), message);
		}

		private void ReportStepProgress(BackgroundWorker worker, string logMessage, string subMessage)
		{
			OutputText(logMessage);
			ReportProgress(worker, _currentMessage + " - " + subMessage);
			currentProgress++;
		}

		private void OutputText(string value)
		{
			_eventAggregator.GetEvent<OutputEvent>().Publish(value);
		}
	}
}
