using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hydra.Core.Domain;
using Hydra.Core.Events;
using MassSpecStudio.Core;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;

namespace Hydra.Modules.Results.Views
{
	public class ResultsViewModel : ViewModelBase
	{
		private Result _result;
		private IEventAggregator eventAggregator;

		public ResultsViewModel(Result result, IEventAggregator eventAggregator)
		{
			_result = result;
			Title = _result.Name;
			this.eventAggregator = eventAggregator;
			ManualValidation = new DelegateCommand<object>(OnManualValidation);
		}

		public DelegateCommand<object> ManualValidation { get; set; }

		public string Title { get; set; }

		public IList<DeuterationResult> DeuterationResults
		{
			get
			{
				if (_result.DeuterationResults != null)
				{
					return _result.DeuterationResults.
								OrderBy(item => item.Labeling.Name).
								OrderBy(item => item.ProteinState.Name).
								ToList();
				}
				return null;
			}
		}

		public MassSpecStudio.Core.Domain.Algorithm AlgorithmUsed
		{
			get { return _result.AlgorithmUsed; }
		}

		public void Refresh()
		{
			NotifyPropertyChanged(() => DeuterationResults);
		}

		public void SaveExcel(string filename)
		{
			SaveReplicatesExcel(filename);
			SaveSummaryExcel(filename);
		}

		private void SaveReplicatesExcel(string filename)
		{
			string csvExportText = CreateReplicateResultHeader(",");
			foreach (RunResult runResult in _result.RunResults)
			{
				csvExportText += runResult.ToString(",");
			}
			File.WriteAllText(filename, csvExportText);
		}

		private void SaveSummaryExcel(string filename)
		{
			string csvExportText = CreateSummaryResultHeader(",");
			foreach (DeuterationResult result in DeuterationResults)
			{
				csvExportText += result.ToString(",");
			}
			File.WriteAllText(filename.Replace(Path.GetExtension(filename), string.Empty) + " Summary.csv", csvExportText);
		}

		private string CreateSummaryResultHeader(string delimiter)
		{
			string result = string.Empty;
			result += "Sequence" + delimiter;
			result += "Start" + delimiter;
			result += "Stop" + delimiter;
			result += "Protein State" + delimiter;
			result += "Labeling" + delimiter;
			result += "AmountDeut" + delimiter;
			result += "AmountDeut SD" + delimiter;
			result += "NValue" + delimiter;
			result += "Centroid Mass" + delimiter;
			result += "Centroid Mass SD" + delimiter;
			result += "Theoretical Centroid Mass" + delimiter;
			result += Environment.NewLine;

			return result;
		}

		private string CreateReplicateResultHeader(string delimiter)
		{
			string result = string.Empty;
			result += "ID" + delimiter;
			result += "ProteinSource" + delimiter;
			result += "AminoAcidStart" + delimiter;
			result += "AminoAcidStop" + delimiter;
			result += "Sequence" + delimiter;
			result += "MonoIsotopicMass" + delimiter;
			result += "Z" + delimiter;
			result += "Period" + delimiter;
			result += "RT" + delimiter;
			result += "RTvariance" + delimiter;
			result += "XicMass1" + delimiter;
			result += "XicMass2" + delimiter;
			result += "XicAdjustment" + delimiter;
			result += "XicSelectionWidth" + delimiter;
			result += "XicPeakPickerOption" + delimiter;
			result += "MsThreshold" + delimiter;
			result += "PeaksInCalculation" + delimiter;
			result += "Notes" + delimiter;
			result += "Protein State" + delimiter;
			result += "Labeling" + delimiter;
			result += "AmountDeut" + delimiter;
			result += "Average Mass" + delimiter;
			result += "Theoretical Average Mass" + delimiter;
			result += "Centroid Mass" + delimiter;
			result += "Theoretical Centroid Mass" + delimiter;
			result += Environment.NewLine;

			return result;
		}

		private void OnManualValidation(object value)
		{
			eventAggregator.GetEvent<LoadManualValidationEvent>().Publish(_result);
		}
	}
}
