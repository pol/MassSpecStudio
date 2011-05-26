using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Hydra.Modules.Project.WizardViews
{
	[Export]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	public partial class DataProviderView : UserControl
	{
		public DataProviderView()
		{
			InitializeComponent();
		}

		public DataProviderViewModel ViewModel
		{
			get { return DataContext as DataProviderViewModel; }
		}
	}
}

