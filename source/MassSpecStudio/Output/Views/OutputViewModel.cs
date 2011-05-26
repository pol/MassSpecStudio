using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using MassSpecStudio.Core;
using MassSpecStudio.Core.Events;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;

namespace MassSpecStudio.Modules.Output.Views
{
	[Export(typeof(OutputViewModel))]
	public class OutputViewModel : ViewModelBase
	{
		private string _outputText;

		[ImportingConstructor]
		public OutputViewModel(IEventAggregator eventAggregator)
		{
			_outputText = string.Empty;
			OutputText = new ObservableCollection<IOutput>();
			eventAggregator.GetEvent<OutputEvent>().Subscribe(AddOutput, ThreadOption.UIThread);
			eventAggregator.GetEvent<ClickableOutputEvent>().Subscribe(AddClickableOutput, ThreadOption.UIThread);
			eventAggregator.GetEvent<ClearOutputEvent>().Subscribe(ClearOutput, ThreadOption.UIThread);
			Clear = new DelegateCommand<string>(ClearOutput);
		}

		public DelegateCommand<string> Clear { get; set; }

		public ObservableCollection<IOutput> OutputText { get; set; }

		private void AddOutput(string value)
		{
			OutputText.Add(new Text(value));
		}

		private void AddClickableOutput(ClickableOutputEvent value)
		{
			OutputText.Add(new ClickableText(value.Text, value.Click, value.Parameter));
		}

		private void ClearOutput(string value)
		{
			OutputText.Clear();
		}
	}
}
