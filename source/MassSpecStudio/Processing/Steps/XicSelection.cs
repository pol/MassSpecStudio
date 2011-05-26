using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using AvalonDock;
using MassSpecStudio.Core;
using MassSpecStudio.Core.Domain;
using MassSpecStudio.Core.Events;
using MassSpecStudio.Core.Processing;
using MassSpecStudio.Processing.Steps.Views;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

namespace MassSpecStudio.Processing.Steps
{
	[Export(typeof(IXicSelection))]
	public class XicSelection : ViewModelBase, IXicSelection
	{
		private readonly IEventAggregator _eventAggregator;
		private readonly IRegionManager _regionManager;
		private readonly ClickableOutputEvent _clickableOutput;
		private double _mzVar = 0.01;

		[ImportingConstructor]
		public XicSelection(IEventAggregator eventAggregator, IRegionManager regionManager)
		{
			_eventAggregator = eventAggregator;
			_regionManager = regionManager;
			_clickableOutput = _eventAggregator.GetEvent<ClickableOutputEvent>();
		}

		[Category("XIC Generator")]
		[DisplayName("MZ Range")]
		public double MzVar
		{
			get
			{
				return _mzVar;
			}

			set
			{
				_mzVar = value;
				NotifyPropertyChanged(() => MzVar);
			}
		}

		public ProcessingStep BuildParameterList()
		{
			ProcessingStep step = new ProcessingStep(this);
			step.AddParameter("MzVar", MzVar);
			return step;
		}

		public IXYData Execute(Sample sample, double mass1)
		{
			IXYData xicData = sample.GetExtractedIonChromatogram(mass1, mass1, _mzVar, Core.TimeUnit.Seconds);
			xicData.Title = "XIC (" + mass1.ToString("N4") + " mzTol=" + _mzVar + ") - " + Path.GetFileName(sample.FileName);

			ClickableOutputEvent clickableOutput = new ClickableOutputEvent();
			clickableOutput.Click = new DelegateCommand<IXYData>(OnViewXIC);
			clickableOutput.Text = "     XIC found (sample=" + sample.FileName + ", x values=" + xicData.XValues.Count + ",  y values=" + xicData.YValues.Count + ", time unit=" + xicData.TimeUnit + ")";
			clickableOutput.Parameter = xicData;
			_clickableOutput.Publish(clickableOutput);
			return xicData;
		}

		private void OnViewXIC(IXYData value)
		{
			ManagedContent view = _regionManager.FindExistingView("DocumentRegion", typeof(XicSelectionView), value.Title);
			if (view == null)
			{
				view = new XicSelectionView(value);
				view.Title = value.Title;
				_regionManager.AddToRegion("DocumentRegion", view);
			}
			view.Show();
			view.Activate();
		}
	}
}
