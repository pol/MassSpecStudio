using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using MassSpecStudio.Core.Domain;

namespace Hydra.Core.Domain
{
	[DataContract]
	public class Result
	{
		private Experiment _experiment;
		private IList<RunResult> _runResults;
		private IList<DeuterationResult> _deuterationResults;
		private string _location;
		private string _name;
		private bool _isManuallyValidated;
		private Algorithm _algorithmUsed;

		public Result()
		{
			_runResults = new List<RunResult>();
			_deuterationResults = new List<DeuterationResult>();
		}

		public Result(string name, Experiment experiment)
			: this()
		{
			_experiment = experiment;
			_name = name;

			_location = "Results - " + DateTime.Now.ToString("yyyy-MM-dd hh.mm.ss") + ".xml";
		}

		[DataMember]
		public string Location
		{
			get { return _location; }
			set { _location = value; }
		}

		[DataMember]
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		[DataMember]
		public IList<RunResult> RunResults
		{
			get { return _runResults; }
			internal set { _runResults = value; }
		}

		[DataMember]
		public IList<DeuterationResult> DeuterationResults
		{
			get { return _deuterationResults; }
			internal set { _deuterationResults = value; }
		}

		[DataMember]
		public Algorithm AlgorithmUsed
		{
			get { return _algorithmUsed; }
			set { _algorithmUsed = value; }
		}

		[DataMember]
		public bool IsManuallyValidated
		{
			get { return _isManuallyValidated; }
			set { _isManuallyValidated = value; }
		}

		public static Result Open(Experiment experiment, string path)
		{
			DataContractSerializer serializer = new DataContractSerializer(typeof(Result));
			using (XmlReader stream = XmlReader.Create(Path.Combine(GetFullPath(experiment), path)))
			{
				Result result = serializer.ReadObject(stream, true) as Result;
				result.Location = path;
				result._experiment = experiment;

				foreach (RunResult runResult in result.RunResults)
				{
					runResult.Run = experiment.Runs.Where(item => item.FileName == runResult.Run.FileName).FirstOrDefault();
				}
				return result;
			}
		}

		public static IList<Result> OpenDirectory(Experiment experiment)
		{
			string[] resultFiles = Directory.GetFiles(Path.Combine(GetFullPath(experiment)), "*.xml");
			IList<Result> results = new List<Result>();
			foreach (string resultFile in resultFiles)
			{
				results.Add(Result.Open(experiment, Path.GetFileName(resultFile)));
			}
			return results;
		}

		public void Delete()
		{
			string resultPath = Path.Combine(GetFullPath(_experiment), Location);
			File.Delete(resultPath);
		}

		public void Save()
		{
			DataContractSerializer serializer = new DataContractSerializer(typeof(Result));
			using (FileStream stream = new FileStream(Path.Combine(GetFullPath(this._experiment), Location), FileMode.Create))
			{
				serializer.WriteObject(stream, this);
			}
		}

		public Result Clone(string name)
		{
			Result clonedResult = new Result(name, this._experiment);
			clonedResult.IsManuallyValidated = IsManuallyValidated;
			clonedResult.AlgorithmUsed = AlgorithmUsed;
			clonedResult.Name += " (" + DateTime.Now.ToString() + ")";
			clonedResult.Location = name + " - " + DateTime.Now.ToString("yyyy-MM-dd hh.mm.ss") + ".xml";

			foreach (RunResult runResult in _runResults)
			{
				clonedResult.RunResults.Add(runResult.Clone());
			}

			foreach (DeuterationResult deutResult in _deuterationResults)
			{
				clonedResult.DeuterationResults.Add(deutResult.Clone());
			}

			return clonedResult;
		}

		private static string GetFullPath(Experiment experiment)
		{
			return Path.Combine(Path.GetDirectoryName(experiment.Location), "Results");
		}
	}
}
