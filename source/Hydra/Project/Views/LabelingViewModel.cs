using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using Hydra.Core.Domain;
using MassSpecStudio.Core;
using MassSpecStudio.UI.Controls;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;

namespace Hydra.Modules.Project.Views
{
	[DefaultProperty("Name")]
	public class LabelingViewModel : TreeViewItemBase<ProteinStateViewModel, RunViewModel>
	{
		private readonly IServiceLocator serviceLocator;
		private Labeling _labeling;
		private Experiment _rootExperiment;

		public LabelingViewModel(Labeling labeling, ProteinState proteinState, Experiment rootExperiment)
		{
			try
			{
				serviceLocator = ServiceLocator.Current;
			}
			catch (NullReferenceException)
			{
			}

			_labeling = labeling;
			_rootExperiment = rootExperiment;
			Data = labeling;
			AddRunDataAndCopyFile = new DelegateCommand<string>(OnAddRunDataAndCopyFiles);
			RemoveRunData = new DelegateCommand<RunViewModel>(OnRemoveRunData);

			IList<Run> runs = rootExperiment.Runs.Where(data => data.Labeling == labeling && data.ProteinState == proteinState).ToList();
			foreach (Run run in runs)
			{
				Children.Add(new RunViewModel(run));
				Children.Last().Parent = this;
			}
		}

		public LabelingViewModel(Labeling labeling, ProteinState proteinState, Experiment rootExperiment, IServiceLocator serviceLocator)
			: this(labeling, proteinState, rootExperiment)
		{
			this.serviceLocator = serviceLocator;
		}

		[Browsable(false)]
		public DelegateCommand<string> AddRunDataAndCopyFile { get; set; }

		[Browsable(false)]
		public DelegateCommand<RunViewModel> RemoveRunData { get; set; }

		[Browsable(false)]
		public override string Name
		{
			get
			{
				return _labeling.LabelingTime + "(" + _labeling.LabelingPercent.ToString(CultureInfo.InvariantCulture) + ")";
			}

			set
			{
			}
		}

		[Category("Common Properties")]
		[DisplayName("Percent")]
		public double LabelingPercent
		{
			get
			{
				return _labeling.LabelingPercent;
			}

			set
			{
				_labeling.LabelingPercent = value;
				NotifyPropertyChanged(() => LabelingPercent);
				NotifyPropertyChanged(() => Name);
			}
		}

		[Category("Common Properties")]
		[DisplayName("Time")]
		public double LabelingTime
		{
			get
			{
				return _labeling.LabelingTime;
			}

			set
			{
				_labeling.LabelingTime = value;
				NotifyPropertyChanged(() => LabelingTime);
				NotifyPropertyChanged(() => Name);
			}
		}

		internal Run AddRunData(string value)
		{
			Run run = new Run(value, value, Parent.ProteinState, _labeling, Parent.Parent.Experiment, serviceLocator.GetAllInstances<IDataProvider>().Where(item => item.TypeId == _rootExperiment.DataProviderType).First());

			RunViewModel runViewModel = new RunViewModel(run);
			Children.Add(runViewModel);
			runViewModel.Parent = this;
			runViewModel.IsExpanded = true;

			return run;
		}

		private void OnAddRunDataAndCopyFiles(string value)
		{
			Run run = AddRunData(value);

			ProgressDialog dialog = new ProgressDialog("Copying data file");

			dialog.RunWorkerThread(run, DoWork);
		}

		private void DoWork(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = (BackgroundWorker)sender;

			Run run = e.Argument as Run;

			string experimentDirectory = Path.GetDirectoryName(_rootExperiment.Location);

			string fileName = Path.GetFileName(run.FileName);
			string expectedFileLocation = Path.Combine(experimentDirectory, @"Data\" + fileName);

			if (!File.Exists(expectedFileLocation))
			{
				worker.ReportProgress(50, "Copying " + fileName);

				File.Copy(run.FileName, expectedFileLocation);
				run.FileName = expectedFileLocation;
			}
		}

		private void OnRemoveRunData(RunViewModel value)
		{
			if (value != null)
			{
				Children.Remove(value);
				_rootExperiment.Runs.Remove((Run)value.Data);
			}
		}
	}
}
