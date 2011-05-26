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
	[Export(typeof(ISpectrumSelection))]
	public class SpectrumSelection : ViewModelBase, ISpectrumSelection
	{
		private readonly IEventAggregator _eventAggregator;
		private readonly IRegionManager _regionManager;
		private readonly ClickableOutputEvent _clickableOutput;
		private double _mzLowerOffset = -5;
		private double _mzUpperOffset = 5;

		[ImportingConstructor]
		public SpectrumSelection(IEventAggregator eventAggregator, IRegionManager regionManager)
		{
			_eventAggregator = eventAggregator;
			_regionManager = regionManager;
			_clickableOutput = _eventAggregator.GetEvent<ClickableOutputEvent>();
		}

		[Category("Spectrum Selection")]
		[DisplayName("MZ Lower Offset")]
		public double MZLowerOffset
		{
			get
			{
				return _mzLowerOffset;
			}

			set
			{
				_mzLowerOffset = value;
				NotifyPropertyChanged(() => MZLowerOffset);
			}
		}

		[Category("Spectrum Selection")]
		[DisplayName("MZ Upper Offset")]
		public double MZUpperOffset
		{
			get
			{
				return _mzUpperOffset;
			}

			set
			{
				_mzUpperOffset = value;
				NotifyPropertyChanged(() => MZUpperOffset);
			}
		}

		public ProcessingStep BuildParameterList()
		{
			ProcessingStep step = new ProcessingStep(this);
			step.AddParameter("MZLowerOffset", MZLowerOffset);
			step.AddParameter("MZUpperOffset", MZUpperOffset);
			return step;
		}

		public IXYData Execute(Sample sample, double startTime, double stopTime, double monoIsotopicMass)
		{
			IXYData spectrum = sample.GetSpectrum(startTime, stopTime, monoIsotopicMass + MZLowerOffset, monoIsotopicMass + MZUpperOffset, 0);
			spectrum.Title = "MS (RT=" + startTime.ToString("N2") + "-" + stopTime.ToString("N2") + " Mass=" + monoIsotopicMass.ToString("N4") + ") - " + Path.GetFileName(sample.FileName);

			ClickableOutputEvent clickableOutput = new ClickableOutputEvent();
			clickableOutput.Click = new DelegateCommand<IXYData>(OnViewXIC);
			clickableOutput.Text = "     Spectrum found (sample=" + sample.FileName + ", x values=" + spectrum.XValues.Count + ",  y values=" + spectrum.YValues.Count + ")";
			clickableOutput.Parameter = spectrum;
			_clickableOutput.Publish(clickableOutput);
			return spectrum;
		}

		private void OnViewXIC(IXYData value)
		{
			ManagedContent view = _regionManager.FindExistingView("DocumentRegion", typeof(SpectrumSelectionView), value.Title);
			if (view == null)
			{
				view = new SpectrumSelectionView(value);
				view.Title = value.Title;
				_regionManager.AddToRegion("DocumentRegion", view);
			}
			view.Show();
			view.Activate();
		}
	}
}
