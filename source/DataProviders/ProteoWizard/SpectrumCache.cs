using System.Collections.Generic;
using System.Linq;
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
			while (cache.Keys.Count > 0)
			{
				SpectrumCacheItem spectrumCacheItem = cache[cache.Keys.First()];
				cache.Remove(cache.Keys.First());
				spectrumCacheItem.Dispose();
				spectrumCacheItem = null;
			}
		}
	}
}
