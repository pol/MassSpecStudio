using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using DialogResult = System.Windows.Forms.DialogResult;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;

namespace Hydra.Modules.Project.WizardViews
{
	/// <summary>
	/// Interaction logic for SelectTemplateView.xaml
	/// </summary>
	[Export]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	public partial class SelectTemplateView : UserControl
	{
		public SelectTemplateView()
		{
			InitializeComponent();
		}

		private void OnBrowse(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openDialog = new OpenFileDialog();
			openDialog.Filter = "(*.mssexp)|*.mssexp";
			DialogResult result = openDialog.ShowDialog();

			if (result == System.Windows.Forms.DialogResult.OK)
			{
				((SelectTemplateViewModel)DataContext).SelectedFileName = openDialog.FileName;
			}
		}
	}
}
