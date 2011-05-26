using MassSpecStudio.Core.Domain;

namespace MassSpecStudio.Core.Processing
{
	public interface IMsMsSpectrumSelection : IProcessingStep
	{
		IXYData Execute(Sample sample, double precursorMass, double mz);
	}
}
