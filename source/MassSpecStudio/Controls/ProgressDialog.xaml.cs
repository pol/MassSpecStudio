using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MassSpecStudio.UI.Controls
{
	/// <summary>
	/// Interaction logic for ProgressDialog.xaml
	/// </summary>
	public partial class ProgressDialog : Window
	{
		/// <summary>
		/// The background worker which handles asynchronous invocation
		/// of the worker method.
		/// </summary>
		private readonly BackgroundWorker worker;

		/// <summary>
		/// The timer to be used for automatic progress bar updated.
		/// </summary>
		private readonly DispatcherTimer progressTimer;

		/// <summary>
		/// The UI culture of the thread that invokes the dialog.
		/// </summary>
		private CultureInfo culture;

		/// <summary>
		/// If set, the interval in which the progress bar
		/// gets incremented automatically.
		/// </summary>
		private int? autoIncrementInterval = null;

		/// <summary>
		/// Whether background processing was cancelled by the user.
		/// </summary>
		private bool cancelled = false;

		/// <summary>
		/// Defines the size of a single increment of the progress bar.
		/// Defaults to 5.
		/// </summary>
		private int progressBarIncrement = 5;

		/// <summary>
		/// Provides an exception that occurred during the asynchronous
		/// operation on the worker thread. Defaults to null, which
		/// indicates that no exception occurred at all.
		/// </summary>
		private Exception error = null;

		/// <summary>
		/// The result, if assigned to the <see cref="DoWorkEventArgs.Result"/>
		/// property by the worker method.
		/// </summary>
		private object result = null;

		private DoWorkEventHandler workerCallback;

		public ProgressDialog(string dialogText)
			: this()
		{
			DialogText = dialogText;
		}

		public ProgressDialog()
		{
			InitializeComponent();

			// init the timer
			progressTimer = new DispatcherTimer(DispatcherPriority.SystemIdle, Dispatcher);
			progressTimer.Tick += OnProgressTimer_Tick;

			// init background worker
			worker = new BackgroundWorker();
			worker.WorkerReportsProgress = true;
			worker.WorkerSupportsCancellation = true;

			worker.DoWork += Worker_DoWork;
			worker.ProgressChanged += Worker_ProgressChanged;
			worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
		}

		public string DialogText
		{
			get { return dialogMessage.Text; }
			set { dialogMessage.Text = value; }
		}

		public bool IsCancellingEnabled
		{
			get { return cancel.IsVisible; }
			set { cancel.Visibility = value ? Visibility.Visible : Visibility.Collapsed; }
		}

		public bool Cancelled
		{
			get { return cancelled; }
		}

		public int? AutoIncrementInterval
		{
			get
			{
				return autoIncrementInterval;
			}

			set
			{
				if (value.HasValue && value < 100)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				autoIncrementInterval = value;
			}
		}

		public int ProgressBarIncrement
		{
			get { return progressBarIncrement; }
			set { progressBarIncrement = value; }
		}

		public Exception Error
		{
			get { return error; }
		}

		public object Result
		{
			get { return result; }
		}

		public bool ShowProgressBar
		{
			get { return progressBar.Visibility == Visibility.Visible; }
			set { progressBar.Visibility = value ? Visibility.Visible : Visibility.Hidden; }
		}

		public void UpdateProgress(int progress)
		{
			if (!Dispatcher.CheckAccess())
			{
				// switch to UI thread
				Dispatcher.BeginInvoke(DispatcherPriority.Background, (SendOrPostCallback)delegate { UpdateProgress(progress); }, null);
				return;
			}

			// validate range
			if (progress < progressBar.Minimum || progress > progressBar.Maximum)
			{
				string msg = "Only values between {0} and {1} can be assigned to the progress bar.";
				msg = String.Format(msg, progressBar.Minimum, progressBar.Maximum);
				throw new ArgumentOutOfRangeException("progress", progress, msg);
			}

			// set the progress bar's value
			progressBar.SetValue(ProgressBar.ValueProperty, progress);
		}

		public bool RunWorkerThread(DoWorkEventHandler workHandler)
		{
			return RunWorkerThread(null, workHandler);
		}

		public bool RunWorkerThread(object argument, DoWorkEventHandler workHandler)
		{
			if (autoIncrementInterval.HasValue)
			{
				// run timer to increment progress bar
				progressTimer.Interval = TimeSpan.FromMilliseconds(autoIncrementInterval.Value);
				progressTimer.Start();
			}

			// store the UI culture
			culture = CultureInfo.CurrentUICulture;

			// store reference to callback handler and launch worker thread
			workerCallback = workHandler;
			worker.RunWorkerAsync(argument);

			// display modal dialog (blocks caller)
			return ShowDialog() ?? false;
		}

		public void UpdateStatus(object status)
		{
			Dispatcher.BeginInvoke(DispatcherPriority.Background, (SendOrPostCallback)delegate { statusLabel.SetValue(ContentProperty, status); }, null);
		}

		public DispatcherOperation BeginInvoke(Delegate method, DispatcherPriority priority)
		{
			return Dispatcher.BeginInvoke(priority, method);
		}

		public object Invoke(Delegate method, DispatcherPriority priority)
		{
			return Dispatcher.Invoke(priority, method);
		}

		private void OnProgressTimer_Tick(object sender, EventArgs e)
		{
			int threshold = 100 + progressBarIncrement;
			progressBar.Value = (progressBar.Value + progressBarIncrement) % threshold;
		}

		private void Worker_DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{
				// make sure the UI culture is properly set on the worker thread
				Thread.CurrentThread.CurrentUICulture = culture;

				// invoke the callback method with the designated argument
				workerCallback(sender, e);
			}
			catch (Exception)
			{
				// disable cancelling and rethrow the exception
				Dispatcher.BeginInvoke(DispatcherPriority.Normal, (SendOrPostCallback)delegate { cancel.SetValue(Button.IsEnabledProperty, false); }, null);

				throw;
			}
		}

		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			cancel.IsEnabled = false;
			worker.CancelAsync();
			cancelled = true;
		}

		private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			if (!Dispatcher.CheckAccess())
			{
				// run on UI thread
				ProgressChangedEventHandler handler = Worker_ProgressChanged;
				Dispatcher.Invoke(DispatcherPriority.SystemIdle, handler, new object[] { sender, e }, null);
				return;
			}

			if (e.ProgressPercentage != int.MinValue)
			{
				progressBar.Value = e.ProgressPercentage;
			}

			statusLabel.Content = e.UserState;
		}

		private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (!Dispatcher.CheckAccess())
			{
				// run on UI thread
				RunWorkerCompletedEventHandler handler = Worker_RunWorkerCompleted;
				Dispatcher.Invoke(DispatcherPriority.SystemIdle, handler, new object[] { sender, e }, null);
				return;
			}

			if (e.Error != null)
			{
				error = e.Error;
			}
			else if (!e.Cancelled)
			{
				// assign result if there was neither exception nor cancel
				result = e.Result;
			}

			// update UI in case closing the dialog takes a moment
			progressTimer.Stop();
			progressBar.Value = progressBar.Maximum;
			cancel.IsEnabled = false;

			// set the dialog result, which closes the dialog
			DialogResult = error == null && !e.Cancelled;
		}
	}
}