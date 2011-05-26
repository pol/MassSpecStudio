
namespace ProteoWizard.MassSpecStudio.DataProvider
{
	public class SpectrumCacheItem
	{
		public SpectrumCacheItem(BinarySpectrum spectrum)
		{
			Spectrum = spectrum;
		}

		public BinarySpectrum Spectrum { get; set; }
	}
}
