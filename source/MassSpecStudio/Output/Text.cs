
namespace MassSpecStudio.Modules.Output
{
	public class Text : IOutput
	{
		public Text(string value)
		{
			Value = value;
		}

		public string Value { get; set; }
	}
}
