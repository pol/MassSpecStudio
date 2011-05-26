using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using MassSpecStudio.Core;
using MassSpecStudio.Core.Domain;
using MassSpecStudio.Core.Events;
using MassSpecStudio.Core.Processing;
using MassSpecStudio.Processing.Steps.Views;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

namespace DeconTools.MassSpecStudio.Processing.Steps
{
	[Export(typeof(ISmoothing))]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	public class SavitzkyGolaySmoothing : ViewModelBase, ISmoothing
	{
		private readonly IRegionManager _regionManager;
		private readonly IEventAggregator _eventAggregator;
		private readonly OutputEvent _output;
		private readonly ClickableOutputEvent _clickableOutput;
		private bool enabled;
		private short leftParam;
		private short rightParam;
		private short order;
		private bool executeDisabled = false;

		[ImportingConstructor]
		public SavitzkyGolaySmoothing(IEventAggregator eventAggregator, IRegionManager regionManager)
		{
			_eventAggregator = eventAggregator;
			_output = eventAggregator.GetEvent<OutputEvent>();
			_clickableOutput = eventAggregator.GetEvent<ClickableOutputEvent>();
			_regionManager = regionManager;
			Enabled = true;
			LeftParam = 3;
			RightParam = 3;
			Order = 2;
		}

		public SavitzkyGolaySmoothing()
		{
			executeDisabled = true;
		}

		[Category("Savitzky Golay Smoothing")]
		[DisplayName("Enabled")]
		public bool Enabled
		{
			get
			{
				return enabled;
			}

			set
			{
				enabled = value;
				NotifyPropertyChanged(() => Enabled);
			}
		}

		[Category("Savitzky Golay Smoothing")]
		[DisplayName("Left Parameter")]
		public short LeftParam
		{
			get
			{
				return leftParam;
			}

			set
			{
				leftParam = value;
				NotifyPropertyChanged(() => LeftParam);
			}
		}

		[Category("Savitzky Golay Smoothing")]
		[DisplayName("Right Parameter")]
		public short RightParam
		{
			get
			{
				return rightParam;
			}

			set
			{
				rightParam = value;
				NotifyPropertyChanged(() => RightParam);
			}
		}

		[Category("Savitzky Golay Smoothing")]
		[DisplayName("Order")]
		public short Order
		{
			get
			{
				return order;
			}

			set
			{
				order = value;
				NotifyPropertyChanged(() => Order);
			}
		}

		public ProcessingStep BuildParameterList()
		{
			ProcessingStep step = new ProcessingStep(this);
			step.AddParameter("Enabled", Enabled);
			step.AddParameter("LeftParam", LeftParam);
			step.AddParameter("RightParam", RightParam);
			step.AddParameter("Order", Order);
			return step;
		}

		public IXYData Execute(IXYData xyData)
		{
			if (Enabled && !executeDisabled)
			{
				_output.Publish("     Savitzky Golay Smoothing Started (LeftParam=" + LeftParam + ", RightParam=" + RightParam + ", Order=" + order + ")");
				float[] xvals = new float[xyData.XValues.Count];
				float[] yvals = new float[xyData.YValues.Count];

				xvals = xyData.XValues.Select(item => (float)item).ToArray();
				yvals = xyData.YValues.Select(item => (float)item).ToArray();

				DeconEngine.Utils.SavitzkyGolaySmooth(this.leftParam, this.rightParam, this.order, ref xvals, ref yvals);

				IXYData smoothedXYData = new XYData(xvals.Select(item => (double)item).ToList(), yvals.Select(item => (double)item).ToList());
				smoothedXYData.Title = "Smoothed " + xyData.Title;

				_clickableOutput.Publish(new ClickableOutputEvent()
					{
						Parameter = smoothedXYData,
						Text = "     Smoothing completed",
						Click = new DelegateCommand<IXYData>(OnViewXIC)
					});
				return smoothedXYData;
			}
			return xyData;
		}

		private void OnViewXIC(IXYData value)
		{
			if (value.Title.ToLower().Contains(" xic"))
			{
				XicSelectionView view = new XicSelectionView(value);
				view.Title = value.Title;

				_regionManager.AddToRegion("DocumentRegion", view);
				view.Activate();
			}
			else
			{
				SpectrumSelectionView view = new SpectrumSelectionView(value);
				view.Title = value.Title;

				_regionManager.AddToRegion("DocumentRegion", view);
				view.Activate();
			}
		}

		private float[] ConvertDoubleArrayToFloat(IList<double> doubleArray)
		{
			float[] floatArray = new float[doubleArray.Count];
			for (int i = 0; i < doubleArray.Count; i++)
			{
				floatArray[i] = (float)doubleArray[i];
			}
			return floatArray;
		}

		private IList<double> ConvertFloatArrayToDouble(float[] floatArray)
		{
			IList<double> returnArray = new List<double>();
			for (int i = 0; i < floatArray.Length; i++)
			{
				returnArray.Add((double)floatArray[i]);
			}
			return returnArray;
		}
	}
}
