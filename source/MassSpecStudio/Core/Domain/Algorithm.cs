using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MassSpecStudio.Core.Processing;

namespace MassSpecStudio.Core.Domain
{
	[DataContract]
	public class Algorithm
	{
		private string name;
		private string type;
		private DateTime date;
		private List<ProcessingStep> _processingSteps;

		public Algorithm(IAlgorithm algorithm)
		{
			date = DateTime.Now;
			Name = algorithm.Name;
			Type = algorithm.GetType().FullName;
			_processingSteps = new List<ProcessingStep>();

			foreach (IProcessingStep processingStep in algorithm.ProcessingSteps)
			{
				_processingSteps.Add(processingStep.BuildParameterList());
			}
		}

		[DataMember]
		public List<ProcessingStep> ProcessingSteps
		{
			get { return _processingSteps; }
			set { _processingSteps = value; }
		}

		[DataMember]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		[DataMember]
		public string Type
		{
			get { return type; }
			set { type = value; }
		}

		[DataMember]
		public DateTime Date
		{
			get { return date; }
			set { date = value; }
		}
	}
}
