using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace MassSpecStudio.Core.Domain
{
	[DataContract]
	public class ProjectBase
	{
		public ProjectBase()
		{
			ExperimentReferences = new List<ExperimentReference>();
			Experiments = new List<ExperimentBase>();
		}

		public ProjectBase(string name, string location)
			: this()
		{
			Name = name;
			Location = Path.Combine(location, name, name + ".mssproj");
		}

		[DataMember]
		public string Name { get; set; }

		public string Location { get; set; }

		public string Directory
		{
			get { return Path.GetDirectoryName(Location); }
		}

		[DataMember]
		public IList<ExperimentReference> ExperimentReferences { get; set; }

		public IList<ExperimentBase> Experiments { get; set; }

		public static ProjectBase Open(string path)
		{
			DataContractSerializer serializer = new DataContractSerializer(typeof(Core.Domain.ProjectBase));
			if (!File.Exists(path))
			{
				throw new Exception("File not found.");
			}
			using (XmlReader stream = XmlReader.Create(path))
			{
				ProjectBase result = serializer.ReadObject(stream, true) as ProjectBase;
				result.Experiments = new List<ExperimentBase>();
				result.Location = path;
				return result;
			}
		}

		public void Save()
		{
			DataContractSerializer serializer = new DataContractSerializer(typeof(Core.Domain.ProjectBase));
			using (FileStream stream = new FileStream(Location, FileMode.Create))
			{
				serializer.WriteObject(stream, this);
			}

			foreach (ExperimentBase experiment in Experiments)
			{
				experiment.Save();
			}
		}

		public void CreateDirectoryStructure()
		{
			if (!System.IO.Directory.Exists(Directory))
			{
				System.IO.Directory.CreateDirectory(Directory);
			}

			foreach (ExperimentBase experiment in Experiments)
			{
				experiment.CreateDirectoryStructure();
			}
		}

		[DataContract]
		public class ExperimentReference
		{
			public ExperimentReference(string name, string location, Guid experimentType)
			{
				Name = name;
				Location = location;
				ExperimentType = experimentType;
			}

			[DataMember]
			public string Name { get; set; }

			[DataMember]
			public Guid ExperimentType { get; set; }

			[DataMember]
			public string Location { get; set; }
		}
	}
}
