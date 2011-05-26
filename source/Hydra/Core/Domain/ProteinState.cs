using System;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace Hydra.Core.Domain
{
	[DataContract]
	public class ProteinState
	{
		private string _name;
		private Experiment _parentExperiment;

		public ProteinState(Experiment parentExperiment)
		{
			_name = GetNextName(parentExperiment);
			_parentExperiment = parentExperiment;
			_parentExperiment.ProteinStates.Add(this);
		}

		[DataMember]
		[StringLengthValidator(1, 5, Ruleset = "rule", MessageTemplate = "Too long.")]
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public static bool operator ==(ProteinState proteinState1, ProteinState proteinState2)
		{
			return proteinState1.Equals(proteinState2);
		}

		public static bool operator !=(ProteinState proteinState1, ProteinState proteinState2)
		{
			return !proteinState1.Equals(proteinState2);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is ProteinState && ((ProteinState)obj).Name == this.Name;
		}

		private string GetNextName(Experiment experiment)
		{
			int maximumCount = 0;
			foreach (ProteinState proteinState in experiment.ProteinStates.Where(item => item.Name.ToLower().StartsWith("protein state")).OrderBy(item => item.Name))
			{
				int result = 0;
				if (Int32.TryParse(proteinState.Name.ToLower().Replace("protein state", string.Empty), out result))
				{
					maximumCount = result > maximumCount ? result : maximumCount;
				}
			}

			return "Protein state " + (maximumCount + 1);
		}
	}
}
