using System;
using MassSpecStudio.Core.Domain;

namespace MassSpecStudio.Core
{
	public interface IExperimentType
	{
		/// <summary>
		/// Gets a Uri pointing to a XAML user control that represents the icon for this experiment type.  This icon will be visible in the New Project dialog.
		/// </summary>
		string Icon { get; }

		/// <summary>
		/// Gets or sets the friendly name of this experiment type.  This name will be visible within the New Project dialog.
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// Gets or sets the description of the experiment type.  This description will be visible within the New Project dialog.
		/// </summary>
		string Description { get; set; }

		/// <summary>
		/// Gets the unique GUID for this experiment type.  This property should be given a hard coded GUID that will uniquely identify it.
		/// </summary>
		Guid ExperimentType { get; }

		/// <summary>
		/// Creates a new experiment of this type and returns a reference to the ExperimentBase object that represents this new experiment.
		/// </summary>
		/// <param name="project">The project this experiment is associated with.</param>
		/// <param name="experimentName">The name of the experiment.</param>
		/// <returns>Returns an ExperimentBase object representing the newly created experiment.</returns>
		MassSpecStudio.Core.Domain.ExperimentBase CreateExperiment(ProjectBase project, string experimentName);

		/// <summary>
		/// Opens the experiment for the specified file path and project.
		/// </summary>
		/// <param name="path">The path to the experiment.</param>
		/// <param name="project">The project this experiment belongs to.</param>
		/// <returns>Returns the ExperimentBase object representing the opened experiment.</returns>
		ExperimentBase Open(string path, ProjectBase project);
	}
}
