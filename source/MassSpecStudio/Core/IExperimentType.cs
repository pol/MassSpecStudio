using System;
using MassSpecStudio.Core.Domain;

namespace MassSpecStudio.Core
{
	public interface IExperimentType
	{
		string Icon { get; }

		string Name { get; set; }

		string Description { get; set; }

		Guid ExperimentType { get; }

		MassSpecStudio.Core.Domain.ExperimentBase CreateExperiment(ProjectBase project, string experimentName);

		ExperimentBase Open(string path, ProjectBase project);
	}
}
