using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Hydra.Core.Domain
{
	[DataContract]
	public class ProteinStateDeuterationResult
	{
		private IList<RunResult> _replicateResults;
		private ProteinState _proteinState;

		public ProteinStateDeuterationResult()
		{
			DeuterationValues = new List<double>();
			DeuterationDistributedDeuterationValues = new List<double>();

			CentroidMassValues = new List<double>();
			_replicateResults = new List<RunResult>();
		}

		public ProteinStateDeuterationResult(IList<RunResult> replicates, ProteinState proteinState)
			: this()
		{
			_replicateResults = replicates;
			_proteinState = proteinState;

			foreach (RunResult rr in this.ReplicateResults)
			{
				if (rr != null)
				{
					this.TheoreticalCentroidMass = rr.TheoreticalAverageMass;

					if (rr.IsUsedInCalculations)
					{
						DeuterationValues.Add(rr.AmountDeut);
						DeuterationDistributedDeuterationValues.Add(rr.AmountDeutFromDeutDist);
						CentroidMassValues.Add(rr.AverageMass);
					}
				}
			}

			this.NValue = DeuterationValues.Count;
		}

		[DataMember]
		public ProteinState ProteinState
		{
			get { return _proteinState; }
			internal set { _proteinState = value; }
		}

		[DataMember]
		public double AmountDeut { get; set; }

		[DataMember]
		public double AmountDeuterationFromDeuterationDistribution { get; set; }

		[DataMember]
		public double CentroidMass { get; set; }

		[DataMember]
		public double CentroidMassStandardDeviation { get; set; }

		[DataMember]
		public double AmountDeuterationFromDeuterationDistributionStandardDeviation { get; set; }

		[DataMember]
		public double AmountDeuterationStandardDeviation { get; set; }

		[DataMember]
		public IList<double> DeuterationValues { get; set; }

		[DataMember]
		public IList<double> DeuterationDistributedDeuterationValues { get; set; }

		[DataMember]
		public IList<double> CentroidMassValues { get; set; }

		[DataMember]
		public double TheoreticalCentroidMass { get; set; }

		[DataMember]
		public int NValue { get; set; }

		public IList<RunResult> ReplicateResults
		{
			get { return _replicateResults; }
		}
	}
}
