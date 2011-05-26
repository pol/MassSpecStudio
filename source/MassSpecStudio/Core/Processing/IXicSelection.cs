using MassSpecStudio.Core.Domain;

namespace MassSpecStudio.Core.Processing
{
	public interface IXicSelection : IProcessingStep
	{
		IXYData Execute(Sample sample, double mass1);
	}
}
