using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Hydra.Core.Domain;
using Hydra.Modules.Project.WizardViews;
using MassSpecStudio.Core;
using MassSpecStudio.Core.Domain;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;

namespace Hydra.Modules.Project
{
	public class CreateProjectHelper
	{
		private readonly BackgroundWorker worker;
		private readonly ProjectBase project;

		public CreateProjectHelper(BackgroundWorker worker, ProjectBase project)
		{
			this.worker = worker;
			this.project = project;
		}

		public static WizardViewModel CreateController(Experiment experiment, IRegionManager regionManager, IServiceLocator serviceLocator)
		{
			if (experiment.ExperimentTypeObject is TemplatedHydraExperimentType)
			{
				return new HydraTemplateWizardController(regionManager, serviceLocator, experiment);
			}
			else
			{
				return new HydraExperimentWizardController(regionManager, serviceLocator, experiment);
			}
		}

		public static void ConvertExperimentReferencesToExperiments(ProjectBase project, IServiceLocator serviceLocator)
		{
			project.Experiments.Clear();
			IEnumerable<IExperimentType> experimentTypes = serviceLocator.GetAllInstances<IExperimentType>();
			foreach (MassSpecStudio.Core.Domain.ProjectBase.ExperimentReference experimentReference in project.ExperimentReferences)
			{
				IExperimentType experimentType = experimentTypes.Where(item => item.ExperimentType == experimentReference.ExperimentType).FirstOrDefault();
				project.Experiments.Add(new Experiment(experimentReference.Name, project, experimentType));
			}
		}

		public void CreateProjectDirectoryStructure()
		{
			project.CreateDirectoryStructure();
		}

		public void CopyDataFiles()
		{
			Experiment experiment = project.Experiments.First() as Experiment;
			string experimentDirectory = experiment.Directory;

			int index = 1;
			foreach (Run run in experiment.Runs)
			{
				string fileName = Path.GetFileName(run.FileName);
				string expectedFileLocation = Path.Combine(experimentDirectory, @"Data\" + fileName);

				int progress = Convert.ToInt32(((double)index / (double)experiment.Runs.Count) * 100.0);
				progress = progress == 0 ? 1 : progress;

				if (!File.Exists(expectedFileLocation))
				{
					worker.ReportProgress(progress, "Copying " + fileName);

					File.Copy(run.FileName, expectedFileLocation);
					run.FileName = DirectoryHelper.GetRelativePath(expectedFileLocation, experimentDirectory);
					run.FullPath = expectedFileLocation;
				}
				else
				{
					worker.ReportProgress(progress, "Scanning " + fileName);
				}
				index++;
			}
		}

		public void ReadPeptides()
		{
			Experiment experiment = (Experiment)project.Experiments.First();
			worker.ReportProgress(50, "Reading peptide information");
			if (File.Exists(experiment.PeptideFilePendingImport))
			{
				experiment.Peptides.Add(experiment.PeptideFilePendingImport);
			}
		}
	}
}
