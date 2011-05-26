using System.Runtime.Serialization;
using MassSpecStudio.Core;

namespace Hydra.Core.Domain
{
	[DataContract]
	public class Run : MassSpecStudio.Core.Domain.Sample
	{
		public Run(string filename, string fullPath, ProteinState proteinState, Labeling labeling, Experiment experiment, IDataProvider dataProvider)
			: base(filename, fullPath, dataProvider)
		{
			ProteinState = proteinState;
			Labeling = labeling;
			Experiment = experiment;
			experiment.Runs.Add(this);
		}

		[DataMember]
		public ProteinState ProteinState { get; set; }

		[DataMember]
		public Labeling Labeling { get; set; }

		public Experiment Experiment { get; set; }
	}
}
