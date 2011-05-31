namespace MassSpecStudio.Core.Processing
{
	public interface IProcessingStep
	{
		/// <summary>
		/// Builds a list of ProcessingStep objects that contain all the ProcessStep's parameter values.  This list is used for persistence of processing parameters.
		/// </summary>
		/// <returns>Returns a list of processing steps with their assoicated parameters values.</returns>
		Domain.ProcessingStep BuildParameterList();
	}
}