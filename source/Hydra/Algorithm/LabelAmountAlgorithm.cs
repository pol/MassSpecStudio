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
	[Export(typeof(LabelAmountAlgorithm))]
	public class LabelAmountAlgorithm : AlgorithmBase, IAlgorithm
	{
		private readonly IServiceLocator _serviceLocator;
		private int currentProgress = 1;
		private int progressFinish = 0;
		private string _currentMessage;
		private IXicSelection _xicSelection;
		private IChromatographicPeakDetection _chromatographicPeakDetection;
		private XicPeakPicker _xicPeakPicker;
		private ISpectrumSelection _spectrumSelection;
		private ISpectralPeakDetection _spectralPeakDetection;
		private IsotopicProfileFinder _isotopicProfileFinder;
		private LabelAmountCalculator _labelAmountCalculator;
		private DeuterationResultGenerator _deuterationResultGenerator;
		private ISmoothing _xicSmoothing;
		private ISmoothing _spectrumSmoothing;
		private IEventAggregator _eventAggregator;

		[ImportingConstructor]
		public LabelAmountAlgorithm(
			IEventAggregator eventAggregator,
			IXicSelection xicSelection,
			IChromatographicPeakDetection chromatographicPeakDetection,
			XicPeakPicker xicPeakPicker,
			ISpectrumSelection spectrumSelection,
			ISpectralPeakDetection spectralPeakDetection,
			IsotopicProfileFinder isotopicProfileFinder,
			LabelAmountCalculator labelAmountCalculator,
			ISmoothing xicSmoothing,
			ISmoothing spectrumSmoothing,
			DeuterationResultGenerator deuterationResultGenerator,
			IServiceLocator serviceLocator)
			: base()
		{
			_eventAggregator = eventAggregator;
			_xicSelection = xicSelection;
			_chromatographicPeakDetection = chromatographicPeakDetection;
			_spectrumSelection = spectrumSelection;
			_spectralPeakDetection = spectralPeakDetection;
			_xicPeakPicker = xicPeakPicker;
			_isotopicProfileFinder = isotopicProfileFinder;
			_labelAmountCalculator = labelAmountCalculator;
			_xicSmoothing = xicSmoothing;
			_spectrumSmoothing = spectrumSmoothing;
			_deuterationResultGenerator = deuterationResultGenerator;
			_serviceLocator = serviceLocator;

			ProcessingSteps.Add(_xicSelection);
			ProcessingSteps.Add(_xicSmoothing);
			ProcessingSteps.Add(_chromatographicPeakDetection);
			ProcessingSteps.Add(_xicPeakPicker);
			ProcessingSteps.Add(_spectrumSelection);
			ProcessingSteps.Add(_spectrumSmoothing);
			ProcessingSteps.Add(_spectralPeakDetection);
			ProcessingSteps.Add(_isotopicProfileFinder);
			ProcessingSteps.Add(_labelAmountCalculator);
		}

		public override string Name
		{
			get { return "Label Amount Algorithm"; }
		}

		public override void Execute(BackgroundWorker worker, ExperimentBase experimentBase)
		{
			currentProgress = 1;
			System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
			stopWatch.Start();
			Experiment experiment = experimentBase as Experiment;
			_eventAggregator.GetEvent<ClearOutputEvent>().Publish(null);
			_eventAggregator.GetEvent<OutputEvent>().Publish("------ Processing Started (" + DateTime.Now.ToString() + ") ------ " + DateTime.Now);

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
							RunResult runResult = new RunResult(peptide, run);

							try
							{
								OutputText("-PEPTIDE '" + peptide.Sequence + "'");
								ReportProgress(worker, "Processing " + Path.GetFileName(run.FileName) + " (" + proteinState.Name + ")");

								ReportStepProgress(worker, " 1: XIC Selection", "XIC Selection");
								IXYData xicData = _xicSelection.Execute(run, peptide.XicMass1);

								ReportStepProgress(worker, " 2: XIC Smoothing", "XIC Smoothing");
								xicData = _xicSmoothing.Execute(xicData);

								ReportStepProgress(worker, " 3: Chromatographic Peak Detection", "Chromatographic Peak Detection");
								IList<ChromatographicPeak> xicPeakList = _chromatographicPeakDetection.Execute(xicData);

								ReportStepProgress(worker, " 4: XIC Peak Picking", "XIC Peak Picking");
								ChromatographicPeak xicPeak = _xicPeakPicker.Execute(xicPeakList, peptide);

								runResult.CachedXicPeak = xicPeak;
								runResult.CachedXicPeakList = xicPeakList;
								runResult.CachedXic = xicData;

								if (xicPeak != null)
								{
									double rt1 = xicPeak.Rt + peptide.XicAdjustment - (peptide.XicSelectionWidth / 2);
									double rt2 = xicPeak.Rt + peptide.XicAdjustment + (peptide.XicSelectionWidth / 2);
									OutputText("     Calculated retention time window for spectrum selection is " + rt1 + "-" + rt2 + "(Peptide=" + peptide.Sequence + ", XicAdjustment=" + peptide.XicAdjustment + ", XicSelectionWidth=" + peptide.XicSelectionWidth + ")");

									ReportStepProgress(worker, " 5: Spectrum Selection", "Spectrum Selection");
									IXYData spectralData = _spectrumSelection.Execute(run, rt1, rt2, peptide.MonoIsotopicMass);

									ReportStepProgress(worker, " 6: MS Smoothing", "MS Smoothing");
									spectralData = _spectrumSmoothing.Execute(spectralData);

									ReportStepProgress(worker, " 7: Spectral Peak Detection", "Spectral Peak Detection");
									IList<MSPeak> msPeakList = _spectralPeakDetection.Execute(spectralData);

									runResult.CachedSpectrum = spectralData;
									runResult.CachedMSPeakList = msPeakList;

									ReportStepProgress(worker, " 8: Isotopic Profile Finder", "Isotopic Profile Finder");
									_isotopicProfileFinder.Execute(runResult, msPeakList);

									ReportStepProgress(worker, " 9: Label Amount Calculation", "Label Amount Calculation");
									_labelAmountCalculator.Execute(runResult);
								}
							}
							catch (Exception ex)
							{
								runResult.Note = ex.Message;
								OutputText("ERROR:");
								OutputText(ex.Message);
								OutputText(ex.StackTrace);
							}

							result.RunResults.Add(runResult);
							currentProgress++;
						}

						run.SetDataProvider(_serviceLocator.GetAllInstances<IDataProvider>().Where(item => item.TypeId == experiment.DataProviderType).First());
					}
				}

				ReportProgress(worker, "Generating Deuteration Results");
				_deuterationResultGenerator.Execute(experiment, result);

				experiment.IsProcessed = true;

				experiment.Results.Add(result);

				stopWatch.Stop();
				OutputText("------ Processing Completed (" + DateTime.Now.ToString() + ") - Duration=" + stopWatch.Elapsed.ToString() + " ------");
			}
			catch (Exception ex)
			{
				ReportError(ex, stopWatch);

				return;
			}

			result.AlgorithmUsed = new MassSpecStudio.Core.Domain.Algorithm(this);
			RecentAlgorithms.Write(result.AlgorithmUsed);

			// Publish event with new result so we can create a ResultViewModel for it and display the results.
			_eventAggregator.GetEvent<ResultAddedEvent>().Publish(result);
			_eventAggregator.GetEvent<ViewResultsEvent>().Publish(result);
			_eventAggregator.GetEvent<StatusUpdateEvent>().Publish("Processing Succeeded");
		}

		public RunResult ReExecute(BackgroundWorker worker, Result result, RunResult runResult)
		{
			_eventAggregator.GetEvent<ClearOutputEvent>().Publish(null);
			_eventAggregator.GetEvent<OutputEvent>().Publish("------ Reprocessing Started (" + DateTime.Now.ToString() + ") ------ " + DateTime.Now);
			System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
			stopWatch.Start();
			currentProgress = 1;
			progressFinish = 6;
			ReportProgress(worker, "Reprocessing");
			try
			{
				double rt1 = runResult.CachedXicPeak.Rt + runResult.ActualXicAdjustment - (runResult.ActualXicSelectionWidth / 2);
				double rt2 = runResult.CachedXicPeak.Rt + runResult.ActualXicAdjustment + (runResult.ActualXicSelectionWidth / 2);
				OutputText("     Calculated retention time window for spectrum selection is " + rt1 + "-" + rt2 + "(Peptide=" + runResult.Peptide.Sequence + ", XicAdjustment=" + runResult.ActualXicAdjustment + ", XicSelectionWidth=" + runResult.ActualXicSelectionWidth + ")");

				ReportStepProgress(worker, " 1: Spectrum Selection", "Spectrum Selection");
				IXYData spectralData = _spectrumSelection.Execute(runResult.Run, rt1, rt2, runResult.Peptide.MonoIsotopicMass);
				ReportStepProgress(worker, " 2: MS Smoothing", "MS Smoothing");
				spectralData = _spectrumSmoothing.Execute(spectralData);
				runResult.CachedSpectrum = spectralData;

				ReportStepProgress(worker, " 3: Spectral Peak Detection", "Spectral Peak Detection");
				IList<MSPeak> msPeakList = _spectralPeakDetection.Execute(spectralData);
				runResult.CachedMSPeakList = msPeakList;

				ReportStepProgress(worker, " 4: Isotopic Profile Finder", "Isotopic Profile Finder");
				_isotopicProfileFinder.Execute(runResult, msPeakList);
				_labelAmountCalculator.PeaksInCalcMode = Core.PeaksInLabelCalculationMode.Manual;
				ReportStepProgress(worker, " 5: Label Amount Calculation", "Label Amount Calculation");
				_labelAmountCalculator.Execute(runResult);

				ReportProgress(worker, "Generating Deuteration Results");
				_deuterationResultGenerator.Execute(runResult.Run.Experiment, result);

				_eventAggregator.GetEvent<StatusUpdateEvent>().Publish("Processing Succeeded");
			}
			catch (Exception ex)
			{
				ReportError(ex, stopWatch);
				return null;
			}

			return runResult;
		}

		private void ReportError(Exception ex, System.Diagnostics.Stopwatch stopWatch)
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
		}

		private void ReportProgress(BackgroundWorker worker, string message)
		{
			_currentMessage = message;
			int progress = 0;
			try
			{
				progress = Convert.ToInt32(((double)currentProgress / (double)progressFinish) * 100);
			}
			catch
			{
				// Do Nothing
			}
			worker.ReportProgress(progress, message);
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
