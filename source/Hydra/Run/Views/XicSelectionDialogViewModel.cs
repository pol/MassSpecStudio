using MassSpecStudio.Core;

namespace Hydra.Modules.Run.Views
{
	public class XicSelectionDialogViewModel : ViewModelBase
	{
		public XicSelectionDialogViewModel()
		{
			Mass = 300.0;
			MzTolerance = 0.01;
		}

		public double Mass { get; set; }

		public double MzTolerance { get; set; }
	}
}
