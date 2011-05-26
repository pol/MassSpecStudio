using AvalonDock;
using MassSpecStudio.Core.Domain;
using MassSpecStudio.Core.Extensions;

namespace MassSpecStudio.Processing.Steps.Views
{
	/// <summary>
	/// Interaction logic for XicSelection.xaml
	/// </summary>
	public partial class XicSelectionView : DocumentContent
	{
		public XicSelectionView(IXYData xyData)
		{
			InitializeComponent();

			graphView.LoadGraph("Time", "XIC", xyData.XValues, xyData.YValues);
			listView.ItemsSource = xyData.GetXYPairs();
		}
	}
}
