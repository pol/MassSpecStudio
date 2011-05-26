using System.Runtime.Serialization;

namespace MassSpecStudio.Core.Domain
{
	[DataContract]
	public class ProcessingParameter
	{
		private string name;
		private string value;

		public ProcessingParameter(string name, string value)
		{
			this.name = name;
			this.value = value;
		}

		[DataMember]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		[DataMember]
		public string Value
		{
			get { return value; }
			set { this.value = value; }
		}
	}
}
