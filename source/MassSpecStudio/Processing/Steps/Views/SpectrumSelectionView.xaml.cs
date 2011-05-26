using AvalonDock;
using MassSpecStudio.Core.Domain;
using MassSpecStudio.Core.Extensions;

namespace MassSpecStudio.Processing.Steps.Views
{
	/// <summary>
	/// Interaction logic for SpectrumSelectionView.xaml
	/// </summary>
	public partial class SpectrumSelectionView : DocumentContent
	{
		public SpectrumSelectionView(IXYData xyData)
		{
			InitializeComponent();

			graphView.LoadGraph("m/z", "Spectrum", xyData.XValues, xyData.YValues);
			listView.ItemsSource = xyData.GetXYPairs();
		}
	}
}
