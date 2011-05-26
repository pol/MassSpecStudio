using System.Windows.Input;

namespace MassSpecStudio.Modules.Output
{
	public class ClickableText : IOutput
	{
		public ClickableText(string value, ICommand click, object parameter)
		{
			Value = value;
			Click = click;
			Parameter = parameter;
		}

		public object Parameter { get; set; }

		public string Value { get; set; }

		public ICommand Click { get; set; }
	}
}
