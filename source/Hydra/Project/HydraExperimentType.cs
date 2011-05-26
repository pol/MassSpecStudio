using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Hydra.Core.Domain;
using MassSpecStudio.Core;
using MassSpecStudio.Core.Domain;
using Microsoft.Practices.ServiceLocation;

namespace Hydra.Modules.Project
{
	[Export(typeof(IExperimentType))]
	public class HydraExperimentType : IExperimentType
	{
		protected readonly IServiceLocator ServiceLocator;

		[ImportingConstructor]
		public HydraExperimentType(IServiceLocator serviceLocator)
		{
			ServiceLocator = serviceLocator;
		}

		public virtual string Icon
		{
			get { return "/MassSpecStudio;component/Images/add-item.png"; }
		}

		public virtual string Name
		{
			get { return "Hydra"; }
			set { throw new NotImplementedException(); }
		}

		public virtual string Description
		{
			get { return "Used to investigate HDX data."; }
			set { throw new NotImplementedException(); }
		}

		public virtual Guid ExperimentType
		{
			get { return new Guid("DAEC1373-5354-4622-BDCD-8BAB3BD5788B"); }
		}

		public virtual IExperimentType ExperimentTypeObject
		{
			get { return this; }
		}

		public MassSpecStudio.Core.Domain.ExperimentBase CreateExperiment(ProjectBase project, string experimentName)
		{
			CreateExperimentDirectoryStructure(Path.Combine(project.Directory, experimentName));
			return CreateExperimentFile(project, experimentName);
		}

		public void Save(Experiment experiment)
		{
			experiment.Save();
		}

		public ExperimentBase Open(string path, ProjectBase parentProject)
		{
			Experiment experiment = Experiment.Open(path, parentProject, ServiceLocator);
			foreach (Run run in experiment.Runs)
			{
				run.FullPath = Path.Combine(experiment.Directory, run.FileName);
				IDataProvider dataProvider = ServiceLocator.GetAllInstances<IDataProvider>().Where(item => item.TypeId == experiment.DataProviderType).FirstOrDefault();
				if (dataProvider == null)
				{
					throw new Exception("The data provider was not found for one or more runs in this experiment.");
				}
				run.SetDataProvider(dataProvider);
			}
			return experiment;
		}

		protected void CreateExperimentDirectoryStructure(string experimentDirectory)
		{
			CreateDirectory(experimentDirectory, "Data");
			CreateDirectory(experimentDirectory, "Results");
		}

		protected ExperimentBase CreateExperimentFile(ProjectBase project, string experimentName)
		{
			string experimentDirectory = Path.Combine(project.Directory, experimentName);
			Experiment experiment = new Experiment(experimentName, project, this);
			experiment.Save();

			return experiment;
		}

		private void CreateDirectory(string experimentDirectory, string subDirectory)
		{
			string directory = Path.Combine(experimentDirectory, subDirectory);
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}
		}
	}
}