using MassSpecStudio.Core.Domain;

namespace ProteoWizard.MassSpecStudio.DataProvider
{
	public class TicCache
	{
		private IXYData cache;

		public void Set(IXYData tic)
		{
			cache = tic;
		}

		public IXYData Read()
		{
			return cache;
		}

		public bool IsInCache()
		{
			return cache != null;
		}
	}
}
