using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using MassSpecStudio.Core;
using MassSpecStudio.Core.Domain;
using Microsoft.Practices.ServiceLocation;

namespace Hydra.Core.Domain
{
	[DataContract]
	public class Experiment : ExperimentBase
	{
		private IList<ProteinState> _proteinStates;
		private IList<Result> _results;
		private IList<Labeling> _labeling;
		private IList<Run> _runs;
		private Peptides _peptides;
		private Guid dataProviderType;
		private IDataProvider dataProvider;
		private string _peptideFileLocation;

		public Experiment(string name, ProjectBase project, IExperimentType experimentType)
			: base(name, project, experimentType)
		{
			_peptideFileLocation = @"Data\Peptides.xml";
			_peptides = new Peptides(Path.Combine(Path.GetDirectoryName(Location), PeptidesFileLocation));
		}

		[DataMember]
		public Guid DataProviderType
		{
			get { return dataProviderType; }

			set { dataProviderType = value; }
		}

		public IDataProvider DataProvider
		{
			get
			{
				return dataProvider;
			}

			set
			{
				dataProvider = value;
				dataProviderType = dataProvider.TypeId;
			}
		}

		[DataMember]
		public IList<ProteinState> ProteinStates
		{
			get
			{
				if (_proteinStates == null)
				{
					_proteinStates = new List<ProteinState>();
				}
				return _proteinStates;
			}
		}

		[DataMember]
		public IList<Labeling> Labeling
		{
			get
			{
				if (_labeling == null)
				{
					_labeling = new List<Labeling>();
				}
				return _labeling;
			}
		}

		[DataMember]
		public IList<Run> Runs
		{
			get
			{
				if (_runs == null)
				{
					_runs = new List<Run>();
				}
				return _runs;
			}
		}

		[DataMember]
		public string PeptidesFileLocation
		{
			get
			{
				return _peptideFileLocation;
			}

			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					_peptideFileLocation = value;
				}
			}
		}

		public string PeptideFilePendingImport { get; set; }

		public Peptides Peptides
		{
			get { return _peptides; }
			private set { _peptides = value; }
		}

		public IList<Result> Results
		{
			get
			{
				if (_results == null)
				{
					_results = new List<Result>();
				}
				return _results;
			}

			private set
			{
				_results = value;
			}
		}

		public bool IsProcessed { get; set; }

		public static Experiment Open(string path, ProjectBase parentProject, IServiceLocator serviceLocator)
		{
			DataContractSerializer serializer = new DataContractSerializer(typeof(Experiment));
			using (XmlReader stream = XmlReader.Create(path))
			{
				Experiment experiment = serializer.ReadObject(stream, true) as Experiment;
				experiment.Project = parentProject;
				if (parentProject == null)
				{
					string directory = Path.GetDirectoryName(path);
					experiment.Peptides = Peptides.Open(Path.Combine(directory, experiment.PeptidesFileLocation));
				}
				else
				{
					experiment.Peptides = Peptides.Open(Path.Combine(experiment.Directory, experiment.PeptidesFileLocation));
				}
				if (parentProject != null)
				{
					experiment.Results = Result.OpenDirectory(experiment);
				}
				IDataProvider dataProvider = serviceLocator.GetAllInstances<IDataProvider>().Where(item => item.TypeId == experiment.DataProviderType).FirstOrDefault();
				if (dataProvider == null)
				{
					throw new Exception("Data provider specified in this experiment was not found.");
				}

				experiment.DataProvider = dataProvider;
				foreach (Run run in experiment.Runs)
				{
					run.Experiment = experiment;
				}
				return experiment;
			}
		}

		public IList<Run> GetRunsByProteinState(ProteinState proteinState)
		{
			return (from data in Runs
					where data.ProteinState == proteinState
					select data).ToList();
		}

		public RunResult GetRunResult(Result result, Run run, Peptide peptide)
		{
			return (from data in result.RunResults
					where data.Run.FileName == run.FileName && (data.Peptide.MonoIsotopicMass == peptide.MonoIsotopicMass && data.Peptide.RT == peptide.RT && data.Peptide.ChargeState == peptide.ChargeState)
					select data).FirstOrDefault();
		}

		public override void Save()
		{
			DataContractSerializer serializer = new DataContractSerializer(typeof(Experiment));
			using (FileStream stream = new FileStream(Location, FileMode.Create))
			{
				serializer.WriteObject(stream, this);
			}
			if (_peptides != null)
			{
				_peptides.Save();
			}
			foreach (Result result in Results)
			{
				result.Save();
			}
		}

		public override void CreateDirectoryStructure()
		{
			base.CreateDirectoryStructure();

			System.IO.Directory.CreateDirectory(Path.Combine(Directory, "Data"));
			System.IO.Directory.CreateDirectory(Path.Combine(Directory, "Results"));
		}
	}
}
