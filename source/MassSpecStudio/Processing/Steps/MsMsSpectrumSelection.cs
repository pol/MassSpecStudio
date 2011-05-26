using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using MassSpecStudio.Core;
using MassSpecStudio.Core.Domain;
using MassSpecStudio.Core.Events;
using MassSpecStudio.Core.Processing;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

namespace MassSpecStudio.Processing.Steps
{
	[Export(typeof(IMsMsSpectrumSelection))]
	public class MsMsSpectrumSelection : ViewModelBase, IMsMsSpectrumSelection
	{
		private readonly IEventAggregator eventAggregator;
		private readonly IRegionManager regionManager;
		private readonly ClickableOutputEvent clickableOutput;
		private double mzVariability;
		private double peakElutionWidth;

		[ImportingConstructor]
		public MsMsSpectrumSelection(IEventAggregator eventAggregator, IRegionManager regionManager)
		{
			this.eventAggregator = eventAggregator;
			this.regionManager = regionManager;
			clickableOutput = eventAggregator.GetEvent<ClickableOutputEvent>();
			LCPeakElutionWidth = 1;
			MZVariablility = 0.2;
		}

		[Category("MSMS Spectrum Selection")]
		[DisplayName("LC Peak Elution Width")]
		public double LCPeakElutionWidth
		{
			get { return peakElutionWidth; }
			set { peakElutionWidth = value; }
		}

		[Category("MSMS Spectrum Selection")]
		[DisplayName("MZ Range")]
		public double MZVariablility
		{
			get { return mzVariability; }
			set { mzVariability = value; }
		}

		public ProcessingStep BuildParameterList()
		{
			ProcessingStep step = new ProcessingStep(this);
			step.AddParameter("LCPeakElutionWidth", LCPeakElutionWidth);
			step.AddParameter("MZVariablility", MZVariablility);
			return step;
		}

		public IXYData Execute(Sample sample, double rt, double mz)
		{
			IXYData spectrum = sample.GetMsMsSpectrum(rt, LCPeakElutionWidth, mz, mzVariability);
			spectrum.Title = "MSMS (RT=" + rt.ToString("N2") + " LCPeakElutionWidth=" + LCPeakElutionWidth.ToString("N2") + " mz=" + mz.ToString("N2") + " MZ Range=" + mzVariability.ToString("N2") + ") - " + Path.GetFileName(sample.FileName);

			ClickableOutputEvent clickableOutput = new ClickableOutputEvent();
			clickableOutput.Click = new DelegateCommand<IXYData>(OnViewXIC);
			clickableOutput.Text = "     MSMS Spectrum found (sample=" + sample.FileName + ", x values=" + spectrum.XValues.Count + ",  y values=" + spectrum.YValues.Count + ")";
			clickableOutput.Parameter = spectrum;
			this.clickableOutput.Publish(clickableOutput);
			return spectrum;
		}

		private void OnViewXIC(IXYData value)
		{
			throw new NotImplementedException();
		}
	}
}
