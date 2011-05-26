using System.Collections.Generic;
using MassSpecStudio.Core.Domain;

namespace MassSpecStudio.Core.Processing
{
	public interface IChromatographicPeakDetection : IProcessingStep
	{
		IList<ChromatographicPeak> Execute(IXYData xyData);
	}
}
