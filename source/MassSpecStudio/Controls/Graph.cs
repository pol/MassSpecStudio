using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;

namespace MassSpecStudio.UI.Controls
{
	public partial class Graph : UserControl
	{
		public Graph()
		{
			InitializeComponent();
			InitializeZedGraph();
			InitializeGraphSpecificSettings();
		}

		protected bool IsGraphInitialized { get; set; }

		public void UpdateGraphControl(double[] xvals, double[] yvals)
		{
			this.ZedGraphControl.GraphPane.CurveList.Clear();

			this.ZedGraphControl.GraphPane.AddCurve(string.Empty, xvals, yvals, System.Drawing.Color.Black, ZedGraph.SymbolType.None);

			this.ZedGraphControl.AxisChange();
			this.ZedGraphControl.Refresh();
		}

		public void LoadGraph(string xAxisTitle, string curveTitle, IList<double> xvals, IList<double> yvals)
		{
			if (!IsGraphInitialized)
			{
				InitializeZedGraph();
			}

			ZedGraphControl.GraphPane.XAxis.Scale.Min = xvals[0];
			ZedGraphControl.GraphPane.XAxis.Title.Text = xAxisTitle;
			ZedGraphControl.GraphPane.CurveList.Clear();
			ZedGraphControl.GraphPane.AddCurve(curveTitle, new ZedGraph.PointPairList(xvals, yvals), Color.Black, SymbolType.None);
			ZedGraphControl.GraphPane.Title.Text = curveTitle;

			ZedGraphControl.AxisChange();
			ZedGraphControl.Refresh();
		}

		public void Refresh(string curveTitle, double[] xvals, double[] yvals)
		{
			ZedGraphControl.GraphPane.CurveList.Clear();
			ZedGraphControl.GraphPane.AddCurve(curveTitle, xvals, yvals, Color.Black, SymbolType.None);
			ZedGraphControl.GraphPane.Title.Text = curveTitle;
			ZedGraphControl.AxisChange();
			ZedGraphControl.Refresh();
		}

		public void InitializeZedGraph()
		{
			DoInitializeZedGraph();
			IsGraphInitialized = true;
		}

		protected void DoInitializeZedGraph()
		{
			this.ZedGraphControl.GraphPane.XAxis.Title.FontSpec.Size = 10;
			this.ZedGraphControl.GraphPane.XAxis.MinorTic.IsOpposite = false;
			this.ZedGraphControl.GraphPane.YAxis.MinorTic.IsOpposite = false;
			this.ZedGraphControl.GraphPane.XAxis.MajorTic.IsOpposite = false;
			this.ZedGraphControl.GraphPane.YAxis.MajorTic.IsOpposite = false;
			this.ZedGraphControl.GraphPane.Legend.IsVisible = false;
			this.ZedGraphControl.GraphPane.XAxis.MajorTic.IsInside = false;
			this.ZedGraphControl.GraphPane.YAxis.MajorTic.IsInside = false;
			this.ZedGraphControl.GraphPane.XAxis.MinorTic.IsInside = false;
			this.ZedGraphControl.GraphPane.YAxis.MinorTic.IsInside = false;
			this.ZedGraphControl.GraphPane.XAxis.Scale.FontSpec.Size = 9;
			this.ZedGraphControl.GraphPane.XAxis.Scale.Format = "0.0";
			this.ZedGraphControl.GraphPane.Border.IsVisible = false;
			this.ZedGraphControl.GraphPane.Chart.Border.IsVisible = false;
			this.ZedGraphControl.GraphPane.Title.Text = string.Empty;
			this.ZedGraphControl.GraphPane.YAxis.Scale.MaxAuto = true;
			this.ZedGraphControl.GraphPane.Border.IsVisible = false;
			this.ZedGraphControl.GraphPane.Y2Axis.IsVisible = false;
			this.ZedGraphControl.GraphPane.X2Axis.IsVisible = false;
			this.ZedGraphControl.IsShowPointValues = true;
			this.ZedGraphControl.GraphPane.IsFontsScaled = false;
			this.ZedGraphControl.GraphPane.TitleGap = 0;
			this.ZedGraphControl.GraphPane.YAxis.Title.FontSpec.Size = 10;
			this.ZedGraphControl.GraphPane.XAxis.Title.FontSpec.Size = 10;
			this.ZedGraphControl.GraphPane.YAxis.Scale.FontSpec.Size = 10;
			this.ZedGraphControl.GraphPane.XAxis.Scale.FontSpec.Size = 10;
			this.ZedGraphControl.GraphPane.Title.FontSpec.Size = 11;
			this.ZedGraphControl.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.ZedGraphControl.GraphPane.XAxis.Scale.MaxGrace = 0;
			this.ZedGraphControl.GraphPane.XAxis.Scale.MinGrace = 0;
			this.ZedGraphControl.AxisChange();
			this.ZedGraphControl.Update();
		}

		private void ZedGraphControl1_Load(object sender, EventArgs e) 
		{
			InitializeZedGraph();
		}

		private void InitializeGraphSpecificSettings()
		{
			this.ZedGraphControl.IsZoomOnMouseCenter = false;
			this.ZedGraphControl.IsEnableVZoom = false;
			this.ZedGraphControl.IsEnableVPan = false;
			this.ZedGraphControl.IsEnableHEdit = false;
			this.ZedGraphControl.IsEnableVEdit = false;

			this.ZedGraphControl.GraphPane.XAxis.Title.Text = "time";
			this.ZedGraphControl.GraphPane.YAxis.Title.Text = "Intensity";
			this.ZedGraphControl.PointValueFormat = "0.00";

			this.ZedGraphControl.MouseDoubleClick += ZedGraphControl_MouseDoubleClick;
		}

		private void ZedGraphControl_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			GraphPane pane = ZedGraphControl.MasterPane.FindChartRect(e.Location);

			if (pane != null)
			{
				pane.ZoomStack.PopAll(pane);
				Refresh();
			}
		}
	}
}
