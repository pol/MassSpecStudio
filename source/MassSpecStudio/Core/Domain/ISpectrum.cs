
namespace MassSpecStudio.Core.Domain
{
	public interface ISpectrum : IXYData
	{
		double StartRT { get; }

		double EndRT { get; }

		double TotalIntensity
		{
			get;
		}
	}
}
