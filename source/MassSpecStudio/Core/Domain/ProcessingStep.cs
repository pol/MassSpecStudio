using System.Collections.Generic;
using System.Runtime.Serialization;
using MassSpecStudio.Core.Processing;

namespace MassSpecStudio.Core.Domain
{
	[DataContract]
	public class ProcessingStep
	{
		private string name;
		private string type;
		private List<ProcessingParameter> parameters;

		public ProcessingStep(IProcessingStep processingStep)
		{
			Name = processingStep.GetType().Name;
			Type = processingStep.GetType().FullName;
			parameters = new List<ProcessingParameter>();
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
		public List<ProcessingParameter> Parameters
		{
			get { return parameters; }
			set { parameters = value; }
		}

		public void AddParameter(string key, object value)
		{
			Parameters.Add(new ProcessingParameter(key, value.ToString()));
		}
	}
}
