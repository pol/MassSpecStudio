using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using AvalonDock;
using MassSpecStudio.Core;
using MassSpecStudio.Core.Events;
using MassSpecStudio.Modules.Project.Views;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;

namespace MassSpecStudio
{
	/// <summary>
	/// Interaction logic for Shell.xaml
	/// </summary>
	[Export]
	public sealed partial class Shell : Window
	{
		private readonly IEventAggregator _aggregator;
		private readonly IDocumentManager _documentManager;
		private readonly IRegionManager _regionManager;
		private readonly IServiceLocator _serviceLocator;
		private readonly ShellViewModel _viewModel;

		[ImportingConstructor]
		public Shell(IRegionManager regionManager, IServiceLocator serviceLocator, IEventAggregator aggregator, IDocumentManager documentManager, ShellViewModel viewModel)
		{
			System.Windows.Application.Current.Dispatcher.UnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(Dispatcher_UnhandledException);

			_viewModel = viewModel;
			_aggregator = aggregator;
			_documentManager = documentManager;
			_regionManager = regionManager;
			_serviceLocator = serviceLocator;
			InitializeComponent();
			if (Properties.Settings.Default.WindowState == "Maximized")
			{
				WindowState = System.Windows.WindowState.Maximized;
			}
			else
			{
				WindowState = System.Windows.WindowState.Normal;
				if (Properties.Settings.Default.WindowWidth > 600 && Properties.Settings.Default.WindowHeight > 400)
				{
					Width = Properties.Settings.Default.WindowWidth;
					Height = Properties.Settings.Default.WindowHeight;
				}
				Top = Properties.Settings.Default.WindowTop;
				Left = Properties.Settings.Default.WindowLeft;
			}

			_aggregator.GetEvent<ProjectOpeningEvent>().Subscribe(OnOpening);
		}

		public ShellViewModel ViewModel
		{
			get { return _viewModel; }
		}

		public IDocumentManager DocumentManager
		{
			get { return _documentManager; }
		}

		public void LoadLayout()
		{
			if (!string.IsNullOrEmpty(Properties.Settings.Default.UserTheme))
			{
				if (Properties.Settings.Default.UserTheme == "Black" ||
					Properties.Settings.Default.UserTheme == "Generic")
				{
					ChangeTheme(Properties.Settings.Default.UserTheme);
				}
				else
				{
					ChangeTheme("Generic");
				}
			}
			else
			{
				ChangeTheme("Generic");
			}
		}

		public void CloseProject()
		{
			while (_dockingManager.Documents.Count > 0)
			{
				_dockingManager.Documents.Last().Close();
			}

			_regionManager.Display("DocumentRegion", typeof(StartPageView), _serviceLocator);
		}

		public string GetSaveFileName()
		{
			return null;
		}

		private void OnShowDockableContent(object sender, RoutedEventArgs e)
		{
			var selectedContent = ((System.Windows.Controls.MenuItem)e.OriginalSource).DataContext as DockableContent;

			if (selectedContent != null)
			{
				if (selectedContent.State != DockableContentState.Docked)
				{
					// show content as docked content
					selectedContent.Show(_dockingManager, AnchorStyle.Left);
				}

				selectedContent.Activate();
			}
		}

		private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Properties.Settings.Default.WindowState = WindowState.ToString();
			Properties.Settings.Default.WindowTop = Top;
			Properties.Settings.Default.WindowLeft = Left;
			Properties.Settings.Default.WindowWidth = Width;
			Properties.Settings.Default.WindowHeight = Height;

			string executingDirectory = Path.GetDirectoryName(Assembly.GetAssembly(typeof(Bootstrapper)).Location);
			_dockingManager.SaveLayout(Path.Combine(executingDirectory, @"layout.xml"));
			Properties.Settings.Default.Save();
			Core.Properties.Settings.Default.Save();

			if (DocumentManager.IsProjectOpen)
			{
				MessageBoxResult result = System.Windows.MessageBox.Show(this, "Do you want to save all changes made to this project?", "Save", MessageBoxButton.YesNo);
				if (result == MessageBoxResult.Yes)
				{
					DocumentManager.Save.Execute(null);
				}
			}
		}

		private void ChangeCustomTheme(object sender, RoutedEventArgs e)
		{
			string theme = (string)((System.Windows.Controls.MenuItem)sender).Tag;
			ChangeTheme(theme);
		}

		private void ChangeTheme(string theme)
		{
			System.Windows.Application.Current.Resources.MergedDictionaries.Clear();
			ResourceDictionary rd1 = (ResourceDictionary)System.Windows.Application.LoadComponent(new Uri("/MassSpecStudio;component/Themes/" + theme + ".xaml", UriKind.Relative));
			System.Windows.Application.Current.Resources.MergedDictionaries.Add(rd1);

			ResourceDictionary rd2 = (ResourceDictionary)System.Windows.Application.LoadComponent(new Uri("/WPG;component/Themes/" + (theme == "Black" ? "ExpressionBlend" : "VisualStudio") + ".xaml", UriKind.Relative));
			System.Windows.Application.Current.Resources.MergedDictionaries.Add(rd2);

			string uri = "/AvalonDock.Themes;component/themes/" + (theme == "Black" ? "ExpressionDark" : "dev2010") + ".xaml";
			ThemeFactory.ChangeTheme(new Uri(uri, UriKind.RelativeOrAbsolute));

			Properties.Settings.Default.UserTheme = theme;
		}

		private void ClickExtensionManager(object sender, RoutedEventArgs e)
		{
			_aggregator.GetEvent<OpenExtensionManagerEvent>().Publish(string.Empty);
		}

		private void OpenProject(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openDialog = new OpenFileDialog();
			openDialog.Filter = "(*.mssproj)|*.mssproj";
			DialogResult result = openDialog.ShowDialog();

			if (result == System.Windows.Forms.DialogResult.OK)
			{
				_documentManager.Open(openDialog.FileName);
			}
		}

		private void OpenRecentProject(object sender, RoutedEventArgs e)
		{
			var selectedProject = ((System.Windows.Controls.MenuItem)e.OriginalSource).DataContext.ToString();

			if (File.Exists(selectedProject))
			{
				_documentManager.Open(selectedProject);
			}
		}

		private void OnOpening(object value)
		{
			// Close all documents
			CloseProject();
		}

		private void CloseProject(object sender, RoutedEventArgs e)
		{
			// TODO: Ask if dirty to save.
			_documentManager.CloseProject();
			CloseProject();
			_aggregator.GetEvent<ProjectClosedEvent>().Publish(null);
		}

		private void Exit(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void OnAbout(object sender, RoutedEventArgs e)
		{
			About aboutDialog = new About();
			aboutDialog.Owner = this;
			aboutDialog.ShowDialog();
		}

		private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs args)
		{
			ExceptionWindow window = new ExceptionWindow(args.Exception);
			window.ShowDialog();
			args.Handled = true;
		}
	}
}
