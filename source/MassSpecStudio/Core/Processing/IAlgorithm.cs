using System.Collections.Generic;
using System.ComponentModel;
using MassSpecStudio.Core.Domain;

namespace MassSpecStudio.Core.Processing
{
	public interface IAlgorithm
	{
		/// <summary>
		/// Gets the friendly name of the algorithm for use in user interfaces.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets a value indicating whether this algorithm can be used to process the currently opened project's data.
		/// </summary>
		bool IsEnabled { get; }

		/// <summary>
		/// Gets the list of processing steps that are used within this processing algorithm.
		/// </summary>
		IList<IProcessingStep> ProcessingSteps { get; }

		/// <summary>
		/// Executes the processing algorithm for the specified experiment.  The background worker thread allows processing to occur in the background while the UI can report progress.
		/// </summary>
		/// <param name="worker">The backgrouond worker thread.</param>
		/// <param name="experiment">The experiment to process.</param>
		void Execute(BackgroundWorker worker, ExperimentBase experiment);

		/// <summary>
		/// This method is used to set the parameters of the algorithm to the parameter values included within the algorithm parameters obejct.
		/// </summary>
		/// <param name="algorithmParameters">The algorithm parameters to set the algorithm to.</param>
		void SetParameters(Algorithm algorithmParameters);
	}
}
