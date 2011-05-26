using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace MSStudio.Modules.MainMenu.Views
{
	/// <summary>
	/// Interaction logic for MainMenuView.xaml
	/// </summary>
	[Export]
	public partial class MainMenuView : MenuItem
	{
		[ImportingConstructor]
		public MainMenuView(MainMenuViewModel viewModel)
		{
			this.DataContext = viewModel;
			InitializeComponent();
		}
	}
}
