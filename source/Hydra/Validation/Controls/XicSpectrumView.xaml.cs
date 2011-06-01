using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using Hydra.Processing.Algorithm;
using MassSpecStudio.Core.Domain;
using ZedGraph;

namespace Hydra.Modules.Validation.Controls
{
	/// <summary>
	/// Interaction logic for XicSpectrumView.xaml
	/// </summary>
	public partial class XicSpectrumView : UserControl
	{
		// Using a DependencyProperty as the backing store for Result.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ResultProperty =
			DependencyProperty.Register("Result", typeof(ValidationWrapper), typeof(XicSpectrumView), new UIPropertyMetadata(ResultChanged));

		private ValidationWrapper result;

		public XicSpectrumView()
		{
			InitializeComponent();
		}

		public ValidationWrapper Result
		{
			get { return (ValidationWrapper)GetValue(ResultProperty); }
			set { SetValue(ResultProperty, value); }
		}

		public static void ResultChanged(DependencyObject dp, DependencyPropertyChangedEventArgs args)
		{
			XicSpectrumView control = dp as XicSpectrumView;
			if (args.NewValue != null)
			{
				control.result = args.NewValue as ValidationWrapper;
				control.UpdateGraphs();
				control.xicController.ItemsSource = control.result.XicPeaks;
				control.xicController.SelectedItem = control.result.SelectedXicPeak;
				control.msController.ItemsSource = control.result.MSPeaks;
			}
		}

		public void SetResult(ValidationWrapper result)
		{
			this.result = result;
			UpdateXICGraph();
			UpdateMSGraph();
		}

		public void UpdateGraphs()
		{
			UpdateXICGraph();
			UpdateMSGraph();
		}

		private void UpdateXICGraph()
		{
			this.xic.ZedGraphControl.GraphPane.CurveList.Clear();

			if (result.IsResultBasedOnFragment)
			{
				this.xic.ZedGraphControl.GraphPane.AddCurve(string.Empty, new double[] { 1, 2, 3, 4, 5 }, new double[] { 1, 1, 1, 1, 1 }, System.Drawing.Color.Black, ZedGraph.SymbolType.None);
				this.xic.ZedGraphControl.GraphPane.Title.Text = "** No XIC for fragment ions **";
			}
			else
			{
				IXYData cachedXic = null;

				////if (result.XICTimeValuesAreTheSameForEachXIC)
				////{
				////    xvals = result.Run.XICTimeValues;
				////}
				////else
				////{
				if (result.CachedXic != null)
				{
					cachedXic = result.CachedXic;
				}
				////}

				if (cachedXic == null || cachedXic.Count == 0)
				{
					this.xic.ZedGraphControl.GraphPane.AddCurve(string.Empty, new double[] { 1, 2, 3, 4, 5 }, new double[] { 1, 1, 1, 1, 1 }, System.Drawing.Color.Black, ZedGraph.SymbolType.None);
					this.xic.ZedGraphControl.GraphPane.Title.Text = "No XIC found";
				}
				else
				{
					this.xic.ZedGraphControl.GraphPane.AddCurve(string.Empty, new ZedGraph.PointPairList(cachedXic.XValues, cachedXic.YValues), System.Drawing.Color.Black, ZedGraph.SymbolType.None);
					this.xic.ZedGraphControl.GraphPane.Title.Text = "XIC of  " + result.Peptide.XicMass1.ToString("0.000") + " - " + result.Peptide.XicMass2.ToString("0.000");
				}
			}

			this.xic.ZedGraphControl.AxisChange();
			this.xic.ZedGraphControl.Refresh();
		}

		private void UpdateMSGraph()
		{
			msController.ItemsSource = result.MSPeaks;

			this.spectrum.ZedGraphControl.GraphPane.CurveList.Clear();
			this.spectrum.ZedGraphControl.GraphPane.XAxis.Title.Text = "m/z";

			IList<double> xvals = result.CachedSpectrum != null ? result.CachedSpectrum.XValues : null;
			IList<double> yvals = result.CachedSpectrum != null ? result.CachedSpectrum.YValues : null;

			if (!result.IsResultBasedOnFragment /*&& !IsMSDataStoredInResult()*/ && result.CachedSpectrum == null)
			{
				try
				{
					IXYData xydata = ReprocessAndGetMSValues();
					xvals = xydata.XValues;
					yvals = xydata.YValues;
				}
				catch (Exception)
				{
					xvals = null;
					yvals = null;
				}
			}

			if (xvals != null && xvals.Count > 0)
			{
				if (result.IsResultBasedOnFragment)
				{
					this.spectrum.ZedGraphControl.GraphPane.AddCurve(string.Empty, new ZedGraph.PointPairList(xvals, yvals), System.Drawing.Color.Black, ZedGraph.SymbolType.None);
					this.spectrum.ZedGraphControl.GraphPane.Title.Text = "PeptideID= " + result.FragmentIon.Peptide.Id.ToString() + "; Fragment= " + result.FragmentIon.ToString() + " (m/z " + result.FragmentIon.MZ.ToString("0.00") + ")";
				}
				else
				{
					if (result.CachedXicPeak == null)
					{
						this.spectrum.ZedGraphControl.GraphPane.AddCurve(string.Empty, new double[] { 1, 2, 3, 4, 5 }, new double[] { 1, 1, 1, 1, 1 }, System.Drawing.Color.Black, ZedGraph.SymbolType.None);
						this.spectrum.ZedGraphControl.GraphPane.Title.Text = "PeptideID: " + result.Peptide.Id.ToString() + "; No MS Data";
					}
					else
					{
						double rt1 = result.CachedXicPeak.Rt + result.ActualXicAdjustment - (result.ActualXicSelectionWidth / 2);
						double rt2 = result.CachedXicPeak.Rt + result.ActualXicAdjustment + (result.ActualXicSelectionWidth / 2);

						LineItem graphdata = this.spectrum.ZedGraphControl.GraphPane.AddCurve(string.Empty, new ZedGraph.PointPairList(xvals, yvals), System.Drawing.Color.Black, ZedGraph.SymbolType.None);
						this.spectrum.ZedGraphControl.GraphPane.Title.Text = "PeptideID: " + result.Peptide.Id.ToString() + "; RT= " + result.CachedXicPeak.Rt.ToString("0.000") + "\nMS from " + rt1.ToString("0.000") + " - " + rt2.ToString("0.000") + "\n" + result.Peptide.Sequence + "(" + result.Peptide.MonoIsotopicMass.ToString("0.00") + ", " + ChargeSign(result.Peptide.ChargeState) + result.Peptide.ChargeState + ")";
					}
				}

				if (result.ShowTheoreticalValues)
				{
					this.spectrum.ZedGraphControl.GraphPane.Y2Axis.IsVisible = true;

					List<double> msPeakListMZvals = new List<double>();
					List<double> msPeakListIntensities = new List<double>();

					foreach (MSPeak peak in result.TheoreticalIsotopicPeakList)
					{
						msPeakListMZvals.Add(peak.MZ);
						msPeakListIntensities.Add(peak.Intensity / 100);
					}

					BarItem theoreticalBars = this.spectrum.ZedGraphControl.GraphPane.AddBar("test", msPeakListMZvals.ToArray(), msPeakListIntensities.ToArray(), Color.LightGray);
					theoreticalBars.IsY2Axis = true;

					this.spectrum.ZedGraphControl.GraphPane.Y2Axis.Scale.Min = 0;
					this.spectrum.ZedGraphControl.GraphPane.Y2Axis.Scale.Max = 1.1;
				}

				if (result.ShowDeuteriumDistribution)
				{
					this.spectrum.ZedGraphControl.GraphPane.Y2Axis.IsVisible = true;

					DeuterationDistributionCalculator calc = new DeuterationDistributionCalculator();

					double[] matrixyvals;
					double[] matrixxvals;

					try
					{
						calc.CalculateDeuterationDistribution(
							result.TheoreticalIsotopicPeakList,
							result.IsotopicPeakList,
							result.DeutDistTheoreticalUse == TheoreticalPeakListOptions.Threshold,
							result.DeutDistMSUse == MSPeakListOptions.Threshold,
							0,
							result.ActualDeutDistRightPadding,
							result.DeutDistTheoreticalThreshold,
							result.DeutDistTheoreticalPeaksInCalculation,
							result.ActualDeutDistThreshold,
							result.DeutDistMSPeaksInCalculation,
							out matrixxvals,
							out matrixyvals);

						result.AmountDeutFromDeutDist = calc.CalculateDeuteriumIncorporationFromDistribution(matrixxvals, matrixyvals);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
						matrixxvals = new double[] { 1, 2, 3, 4, 5 };
						matrixyvals = new double[] { 1, 1, 1, 1, 1 };
					}

					int chargeState;

					if (result.IsResultBasedOnFragment)
					{
						chargeState = result.FragmentIon.ChargeState;
					}
					else
					{
						chargeState = result.Peptide.ChargeState;
					}
					if (chargeState == 0)
					{
						chargeState = 1;
					}

					for (int i = 0; i < matrixxvals.Length; i++)
					{
						matrixxvals[i] = (Convert.ToDouble(i) / Convert.ToDouble(chargeState)) + result.IsotopicPeakList[0].MZ;
					}

					BarItem deutDist = this.spectrum.ZedGraphControl.GraphPane.AddBar("test", matrixxvals, matrixyvals, Color.SteelBlue);
					deutDist.IsY2Axis = true;

					this.spectrum.ZedGraphControl.GraphPane.Y2Axis.Scale.Min = 0;
					this.spectrum.ZedGraphControl.GraphPane.Y2Axis.Scale.Max = 1;
					this.spectrum.ZedGraphControl.GraphPane.Y2Axis.Title.FontSpec.Size = 9;
					this.spectrum.ZedGraphControl.GraphPane.Y2Axis.Title.FontSpec.IsBold = false;
					this.spectrum.ZedGraphControl.GraphPane.Y2Axis.Title.Text = "Population";
				}

				if (!result.ShowDeuteriumDistribution && !result.ShowTheoreticalValues)
				{
					this.spectrum.ZedGraphControl.GraphPane.Y2Axis.IsVisible = false;
				}

				this.spectrum.ZedGraphControl.GraphPane.XAxis.Scale.Min = xvals[0];
				this.spectrum.ZedGraphControl.GraphPane.XAxis.Scale.Max = xvals[xvals.Count - 1];
				this.spectrum.ZedGraphControl.GraphPane.YAxis.Scale.Min = 0;
				this.spectrum.ZedGraphControl.GraphPane.YAxis.Scale.MaxAuto = true;
			}
			else
			{
				this.spectrum.ZedGraphControl.GraphPane.AddCurve(string.Empty, new double[] { 1, 2, 3, 4, 5 }, new double[] { 1, 1, 1, 1, 1 }, System.Drawing.Color.Black, ZedGraph.SymbolType.None);
				this.spectrum.ZedGraphControl.GraphPane.Title.Text = "** No MS data **";
			}

			this.spectrum.ZedGraphControl.AxisChange();
			this.spectrum.ZedGraphControl.Refresh();
		}

		private IXYData ReprocessAndGetMSValues()
		{
			////TODO: Later
			return null;
		}

		private string ChargeSign(int p)
		{
			if (p > 0)
			{
				return "+";
			}
			else if (p < 0)
			{
				return "-";
			}
			return string.Empty;
		}

		private void XicPeakSelected(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count > 0)
			{
				ChromatographicPeak peak = (ChromatographicPeak)e.AddedItems[0];
				if (peak != result.SelectedXicPeak)
				{
					result.SelectedXicPeak = peak;
				}
			}
		}
	}
}
