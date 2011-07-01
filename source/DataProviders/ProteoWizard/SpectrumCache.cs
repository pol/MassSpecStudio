using System.Collections.Generic;
using Domain = MassSpecStudio.Core.Domain;

namespace ProteoWizard.MassSpecStudio.DataProvider
{
	public class SpectrumCache
	{
		private Dictionary<int, SpectrumCacheItem> cache;

		public SpectrumCache()
		{
			cache = new Dictionary<int, SpectrumCacheItem>();
		}

		public void Add(int timePoint, BinarySpectrum spectrum)
		{
			cache.Add(timePoint, new SpectrumCacheItem(spectrum));
		}

		public SpectrumCacheItem ReadRawCacheItem(int timePoint)
		{
			return cache[timePoint];
		}

		public Domain.ISpectrum Read(int timePoint)
		{
			SpectrumCacheItem cacheItem = ReadRawCacheItem(timePoint);

			if (cacheItem != null)
			{
				return cacheItem.Spectrum;
			}
			return null;
		}

		public bool IsInCache(int timePoint)
		{
			return cache.ContainsKey(timePoint);
		}

		public void Clear()
		{
			foreach (int key in cache.Keys)
			{
				cache[key].Dispose();
			}
			cache.Clear();
		}
	}
}
