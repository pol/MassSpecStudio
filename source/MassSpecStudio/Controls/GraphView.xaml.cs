using System.Collections.Generic;
using System.Windows.Controls;

namespace MassSpecStudio.UI.Controls
{
	/// <summary>
	/// Interaction logic for GraphView.xaml
	/// </summary>
	public partial class GraphView : UserControl
	{
		public GraphView()
		{
			InitializeComponent();
		}

		public void LoadGraph(string xAxisTitle, string curveTitle, IList<double> xvals, IList<double> yvals)
		{
			spectrum.LoadGraph(xAxisTitle, curveTitle, xvals, yvals);
		}
	}
}
