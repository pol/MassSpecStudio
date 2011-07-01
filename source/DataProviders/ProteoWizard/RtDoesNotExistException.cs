using System;

namespace ProteoWizard.MassSpecStudio.DataProvider
{
	public class RtDoesNotExistException : Exception
	{
		public RtDoesNotExistException()
			: base("The specified retention time does not exist.")
		{
		}
	}
}
