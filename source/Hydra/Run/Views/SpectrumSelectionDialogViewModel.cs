using MassSpecStudio.Core;

namespace Hydra.Modules.Run.Views
{
	public class SpectrumSelectionDialogViewModel : ViewModelBase
	{
		public SpectrumSelectionDialogViewModel()
		{
			MZLowerOffset = -5;
			MZUpperOffset = 5;
		}

		public double StartMass { get; set; }

		public double StopMass { get; set; }

		public double MonoisotopicMass { get; set; }

		public double MZLowerOffset { get; set; }

		public double MZUpperOffset { get; set; }
	}
}
