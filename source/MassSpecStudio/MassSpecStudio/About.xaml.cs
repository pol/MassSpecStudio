using System.Windows;

namespace MassSpecStudio
{
	/// <summary>
	/// Interaction logic for About.xaml
	/// </summary>
	public partial class About : Window
	{
		public About()
		{
			DataContext = new AboutViewModel();
			InitializeComponent();
		}
	}
}
