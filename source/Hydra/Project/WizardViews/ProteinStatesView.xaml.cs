using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Hydra.Modules.Project.WizardViews
{
	/// <summary>
	/// Interaction logic for ProteinStatesView.xaml
	/// </summary>
	[Export]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	public partial class ProteinStatesView : UserControl
	{
		public ProteinStatesView()
		{
			InitializeComponent();
		}
	}
}
