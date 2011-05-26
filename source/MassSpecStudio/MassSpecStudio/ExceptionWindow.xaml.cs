using System;
using System.Windows;

namespace MassSpecStudio
{
	/// <summary>
	/// Interaction logic for ExceptionWindow.xaml
	/// </summary>
	public partial class ExceptionWindow : Window
	{
		public ExceptionWindow(Exception ex)
		{
			Message = "Error:" + Environment.NewLine + ex.Message + Environment.NewLine;
			Message += Environment.NewLine + "Stack Trace:" + Environment.NewLine + ex.StackTrace + Environment.NewLine;
			if (ex.InnerException != null)
			{
				Message += Environment.NewLine + "Inner Exception:" + Environment.NewLine + ex.InnerException.Message + Environment.NewLine;
				Message += Environment.NewLine + "Stack Trace:" + Environment.NewLine + ex.InnerException.StackTrace + Environment.NewLine;
			}

			InitializeComponent();
		}

		public string Message { get; set; }

		private void OnExit(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown(1);
		}
	}
}
