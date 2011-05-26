using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace Hydra.Core.Domain
{
	[DataContract]
	public class Labeling
	{
		private Experiment _parentExperiment;

		public Labeling(Experiment parentExperiment)
		{
			SetNextPercentAndTime(parentExperiment);
			_parentExperiment = parentExperiment;
			_parentExperiment.Labeling.Add(this);
		}

		[DataMember]
		[RangeValidator(0, RangeBoundaryType.Inclusive, 100000, RangeBoundaryType.Inclusive)]
		public double LabelingTime
		{
			get;
			set;
		}

		[DataMember]
		[RangeValidator(0, RangeBoundaryType.Inclusive, 100, RangeBoundaryType.Inclusive)]
		public double LabelingPercent
		{
			get;
			set;
		}

		public string Name
		{
			get { return LabelingTime + " (" + LabelingPercent + ")"; }
		}

		public Experiment Experiment
		{
			get { return _parentExperiment; }
			set { _parentExperiment = value; }
		}

		public static bool operator ==(Labeling labeling1, Labeling labeling2)
		{
			return ((object)labeling1 == null && (object)labeling2 == null) || labeling1.Equals(labeling2);
		}

		public static bool operator !=(Labeling labeling1, Labeling labeling2)
		{
			if ((object)labeling1 == null && (object)labeling2 == null)
			{
				return false;
			}
			else if ((object)labeling1 == null && (object)labeling2 != null)
			{
				return true;
			}
			else
			{
				return !labeling1.Equals(labeling2);
			}
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is Labeling && ((Labeling)obj).LabelingTime == this.LabelingTime && ((Labeling)obj).LabelingPercent == this.LabelingPercent;
		}

		private void SetNextPercentAndTime(Experiment experiment)
		{
			Labeling maxLabelingPercentage = experiment.Labeling.OrderByDescending(item => item.LabelingPercent).FirstOrDefault();
			if (maxLabelingPercentage != null)
			{
				if ((maxLabelingPercentage.LabelingPercent + 10) > 100)
				{
					LabelingPercent = 100;
				}
				else
				{
					LabelingPercent = maxLabelingPercentage.LabelingPercent + 10;
				}
				LabelingTime = maxLabelingPercentage.LabelingTime + 1;
			}
			else
			{
				LabelingPercent = 20;
				LabelingTime = 1;
			}
		}
	}
}
