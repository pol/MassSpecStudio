using MassSpecStudio.Core.Domain;

namespace MassSpecStudio.Core.Processing
{
	public interface ISmoothing : IProcessingStep
	{
		IXYData Execute(IXYData xyData);
	}
}
