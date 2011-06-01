using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Hydra.Core.Domain;
using Hydra.Core.Events;
using Hydra.Processing.Algorithm;
using MassSpecStudio.Core;
using MassSpecStudio.Core.Events;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.ViewModel;

namespace Hydra.Modules.Validation.Views
{
	public class ManualValidationViewModel : NotificationObject
	{
		private Result result;
		private ObservableCollection<ValidationWrapper> currentRunResults;
		private IList<ValidationWrapper> allRunResults;
		private ValidationWrapper selectedRunResult;
		private Run selectedRun;
		private IEventAggregator eventAggregator;
		private LabelAmountAlgorithm labelAmountAlgorithm;
		private IDocumentManager documentManager;

		public ManualValidationViewModel(Result result, IDocumentManager documentManager, IEventAggregator eventAggregator, LabelAmountAlgorithm labelAmountAlgorithm)
		{
			this.eventAggregator = eventAggregator;
			this.result = CloneResultIfNecessary(result);
			this.result = result;
			this.labelAmountAlgorithm = labelAmountAlgorithm;
			this.documentManager = documentManager;

			currentRunResults = new ObservableCollection<ValidationWrapper>();
			allRunResults = (from data in result.RunResults
							 select new ValidationWrapper(data)).ToList();
			foreach (ValidationWrapper runResult in allRunResults)
			{
				runResult.ReprocessingEvent += HandleReprocessingEvent;
				runResult.UpdateEvent += HandleUpdateEvent;
			}

			eventAggregator.GetEvent<PeptideSelectedEvent>().Subscribe(OnPeptideSelected);

			ApplyChanges = new DelegateCommand(OnApplyChanges);
		}

		public DelegateCommand ApplyChanges { get; set; }

		public EventHandler UpdateGraphs { get; set; }

		public EventHandler Reprocess { get; set; }

		public ObservableCollection<ValidationWrapper> CurrentRunResults
		{
			get { return currentRunResults; }
		}

		public ValidationWrapper SelectedRunResult
		{
			get
			{
				return selectedRunResult;
			}

			set
			{
				selectedRunResult = value;
				if (selectedRunResult != null)
				{
					selectedRun = selectedRunResult.RunResult.Run;
				}
				RaisePropertyChanged(() => SelectedRunResult);
				eventAggregator.GetEvent<ObjectSelectionEvent>().Publish(selectedRunResult);
			}
		}

		public void Unsubscribe()
		{
			eventAggregator.GetEvent<PeptideSelectedEvent>().Unsubscribe(OnPeptideSelected);
		}

		public void RefreshPropertiesPanel()
		{
			eventAggregator.GetEvent<ObjectSelectionEvent>().Publish(SelectedRunResult);
		}

		public void ReprocessSelectedResult(BackgroundWorker worker)
		{
			labelAmountAlgorithm.SetParameters(result.AlgorithmUsed);
			labelAmountAlgorithm.ReExecute(worker, result, selectedRunResult.RunResult);
		}

		private void OnApplyChanges()
		{
			eventAggregator.GetEvent<StatusUpdateEvent>().Publish("Peptide list updated.");
			Experiment experiment = (Experiment)documentManager.Experiments.FirstOrDefault();
			if (experiment != null)
			{
				foreach (ValidationWrapper validation in allRunResults)
				{
					Peptide peptide = experiment.Peptides.PeptideCollection.Where(item => item.MonoIsotopicMass == validation.Peptide.MonoIsotopicMass && item.ChargeState == validation.Peptide.ChargeState).FirstOrDefault();
					peptide.PeaksInCalculation = validation.ActualPeaksInCalculation;
					peptide.DeuteriumDistributionThreshold = validation.ActualDeutDistThreshold;
					peptide.DeuteriumDistributionRightPadding = validation.ActualDeutDistRightPadding;
					peptide.RT = validation.SelectedXicPeak.Rt;

					peptide.XicAdjustment = validation.ActualXicAdjustment;
					peptide.XicSelectionWidth = validation.ActualXicSelectionWidth;
				}
			}
		}

		private void OnPeptideSelected(Peptide peptide)
		{
			if (peptide != null)
			{
				CurrentRunResults.Clear();
				var runResults = (from data in allRunResults
								  where data.Peptide.MonoIsotopicMass == peptide.MonoIsotopicMass && data.Peptide.RT == peptide.RT
								  select data).ToList();

				foreach (ValidationWrapper runResult in runResults)
				{
					CurrentRunResults.Add(runResult);
				}

				if (CurrentRunResults.Count > 0)
				{
					if (selectedRun == null)
					{
						SelectedRunResult = CurrentRunResults.First();
					}
					else
					{
						SelectedRunResult = CurrentRunResults.FirstOrDefault(item => item.RunResult.Run.FileName == selectedRun.FileName);
					}
				}
			}
		}

		private void HandleReprocessingEvent(object sender, EventArgs e)
		{
			eventAggregator.GetEvent<StatusUpdateEvent>().Publish("Reprocessing");

			if (Reprocess != null)
			{
				Reprocess(this, null);
			}

			selectedRunResult.UpdateValues();
			if (UpdateGraphs != null)
			{
				UpdateGraphs(this, null);
			}

			eventAggregator.GetEvent<StatusUpdateEvent>().Publish("Ready");
			eventAggregator.GetEvent<DisplayPeptidesWindowEvent>().Publish(result);
		}

		private void HandleUpdateEvent(object sender, EventArgs e)
		{
			if (UpdateGraphs != null)
			{
				UpdateGraphs(this, null);
			}
		}

		private Result CloneResultIfNecessary(Result incomingResult)
		{
			if (incomingResult.IsManuallyValidated)
			{
				return incomingResult;
			}
			else
			{
				Result newResult = incomingResult.Clone("Validated Results");
				newResult.IsManuallyValidated = true;
				eventAggregator.GetEvent<ResultAddedEvent>().Publish(newResult);
				return newResult;
			}
		}
	}
}
