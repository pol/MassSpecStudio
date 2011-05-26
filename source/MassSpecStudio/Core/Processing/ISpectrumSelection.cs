using MassSpecStudio.Core.Domain;

namespace MassSpecStudio.Core.Processing
{
	public interface ISpectrumSelection : IProcessingStep
	{
		IXYData Execute(Sample sample, double startTime, double stopTime, double monoIsotopicMass);
	}
}
