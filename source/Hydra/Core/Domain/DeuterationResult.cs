using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Hydra.Core.Domain
{
	[DataContract]
	public class DeuterationResult
	{
		private Peptide _peptide;
		private Labeling _labeling;
		private ProteinState _proteinState;
		private IList<RunResult> _replicateResults;

		public DeuterationResult(Peptide peptide, ProteinState proteinState, Labeling labeling, IList<RunResult> replicateResults)
		{
			_peptide = peptide;
			_proteinState = proteinState;
			_labeling = labeling;
			_replicateResults = replicateResults;

			DeuterationValues = new List<double>();
			DeuterationDistributedDeuterationValues = new List<double>();
			CentroidMassValues = new List<double>();

			NValue = 0;
			foreach (RunResult replicate in _replicateResults)
			{
				if (replicate != null)
				{
					this.TheoreticalCentroidMass = replicate.TheoreticalAverageMass;

					if (replicate.IsUsedInCalculations)
					{
						DeuterationValues.Add(replicate.AmountDeut);
						DeuterationDistributedDeuterationValues.Add(replicate.AmountDeutFromDeutDist);
						CentroidMassValues.Add(replicate.AverageMass);
						NValue++;
					}
				}
			}
		}

		[DataMember]
		public Peptide Peptide
		{
			get { return _peptide; }
			internal set { _peptide = value; }
		}

		[DataMember]
		public Labeling Labeling
		{
			get { return _labeling; }
			internal set { _labeling = value; }
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
			get
			{
				if (_replicateResults == null)
				{
					_replicateResults = new List<RunResult>();
				}
				return _replicateResults;
			}
		}

		public string ToString(string delimiter)
		{
			string result = string.Empty;

			result += Peptide.Sequence + delimiter;
			result += Peptide.AminoAcidStart + delimiter;
			result += Peptide.AminoAcidStop + delimiter;
			result += ProteinState.Name + delimiter;
			result += Labeling.Name + delimiter;
			result += AmountDeut + delimiter;
			result += AmountDeuterationStandardDeviation + delimiter;
			result += NValue + delimiter;
			result += CentroidMass + delimiter;
			result += CentroidMassStandardDeviation + delimiter;
			result += TheoreticalCentroidMass + delimiter;

			result += Environment.NewLine;
			return result;
		}

		public DeuterationResult Clone()
		{
			DeuterationResult clonedDeutResult = new DeuterationResult(Peptide, ProteinState, Labeling, ReplicateResults);

			clonedDeutResult.AmountDeut = AmountDeut;
			clonedDeutResult.AmountDeuterationFromDeuterationDistribution = AmountDeuterationFromDeuterationDistribution;
			clonedDeutResult.CentroidMass = CentroidMass;
			clonedDeutResult.CentroidMassStandardDeviation = CentroidMassStandardDeviation;
			clonedDeutResult.AmountDeuterationFromDeuterationDistributionStandardDeviation = AmountDeuterationFromDeuterationDistributionStandardDeviation;
			clonedDeutResult.AmountDeuterationStandardDeviation = AmountDeuterationStandardDeviation;
			return clonedDeutResult;
		}
	}
}
