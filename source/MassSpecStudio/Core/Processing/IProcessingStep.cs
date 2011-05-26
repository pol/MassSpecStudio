namespace MassSpecStudio.Core.Processing
{
	public interface IProcessingStep
	{
		Domain.ProcessingStep BuildParameterList();
	}
}