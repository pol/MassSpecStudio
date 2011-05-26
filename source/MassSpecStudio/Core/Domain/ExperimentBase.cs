using System;
using System.IO;
using System.Runtime.Serialization;

namespace MassSpecStudio.Core.Domain
{
	[DataContract]
	public class ExperimentBase
	{
		private ProjectBase project;

		public ExperimentBase(string name, ProjectBase project, IExperimentType experimentTypeObject)
		{
			Name = name;
			ExperimentTypeObject = experimentTypeObject;
			ExperimentType = experimentTypeObject.ExperimentType;
			this.project = project;
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public Guid ExperimentType { get; set; }

		public IExperimentType ExperimentTypeObject { get; set; }

		public ProjectBase Project
		{
			get { return project; }
			set { project = value; }
		}

		public string Location
		{
			get { return Path.Combine(project.Directory, Name, Name + ".mssexp"); }
		}

		public string Directory
		{
			get { return Path.GetDirectoryName(Location); }
		}

		public virtual void Save()
		{
			// Do nothing
		}

		public virtual void CreateDirectoryStructure()
		{
			if (!System.IO.Directory.Exists(Directory))
			{
				System.IO.Directory.CreateDirectory(Directory);
			}
		}
	}
}
