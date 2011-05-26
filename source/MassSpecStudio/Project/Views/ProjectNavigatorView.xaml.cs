using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AvalonDock;
using System.ComponentModel.Composition;

namespace MSStudio.Module.Project.Views
{
	/// <summary>
	/// Interaction logic for ProjectView.xaml
	/// </summary>
	[Export]
	public partial class ProjectNavigatorView : DockableContent
	{
		[ImportingConstructor]
		public ProjectNavigatorView(ProjectNavigatorViewModel viewModel)
		{
			this.DataContext = viewModel;
			InitializeComponent();
		}
	}
}
