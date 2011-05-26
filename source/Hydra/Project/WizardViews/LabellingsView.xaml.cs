using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Hydra.Modules.Project.WizardViews
{
	/// <summary>
	/// Interaction logic for LabellingsView.xaml
	/// </summary>
	[Export]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	public partial class LabellingsView : UserControl
	{
		public LabellingsView()
		{
			InitializeComponent();
		}
	}
}
