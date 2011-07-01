using System;

namespace ProteoWizard.MassSpecStudio.DataProvider
{
	public class SpectrumCacheItem : IDisposable
	{
		public SpectrumCacheItem(BinarySpectrum spectrum)
		{
			Spectrum = spectrum;
		}

		public BinarySpectrum Spectrum { get; set; }

		public void Dispose()
		{
			Spectrum.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}
