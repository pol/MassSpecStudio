using System.Collections.Generic;
using System.ComponentModel;
using MassSpecStudio.Core.Domain;

namespace MassSpecStudio.Core.Processing
{
	public interface IAlgorithm
	{
		string Name { get; }

		bool IsEnabled { get; }

		IList<IProcessingStep> ProcessingSteps { get; }

		void Execute(BackgroundWorker worker, ExperimentBase experiment);

		void SetParameters(Algorithm algorithmParameters);
	}
}
