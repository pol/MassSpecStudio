using System.Collections.Generic;
using MassSpecStudio.Core.Domain;

namespace MassSpecStudio.Core.Processing
{
	public interface ISpectralPeakDetection : IProcessingStep
	{
		IList<MSPeak> Execute(IXYData xyData);
	}
}
