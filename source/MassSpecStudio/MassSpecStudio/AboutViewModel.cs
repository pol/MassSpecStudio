
namespace MassSpecStudio
{
	public class AboutViewModel
	{
		public string Version
		{
			get
			{
				System.Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
				return version.Major + "." + version.Minor + "." + version.Revision + "." + version.Build;
			}
		}
	}
}
