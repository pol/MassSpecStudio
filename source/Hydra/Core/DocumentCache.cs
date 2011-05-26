using Hydra.Core.Domain;

namespace Hydra.Core
{
	public static class DocumentCache
	{
		public static object ProjectViewModel { get; set; }

		public static Experiment Experiment { get; set; }
	}
}
