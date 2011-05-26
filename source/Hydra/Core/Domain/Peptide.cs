using System.Collections.ObjectModel;
using System.Globalization;
using System.Runtime.Serialization;

namespace Hydra.Core.Domain
{
	[DataContract]
	public sealed class Peptide
	{
		private int id;
		private int aminoAcidStart;
		private int aminoAcidStop;
		private string sequence;
		private double monoIsotopicMass;
		private int chargeState;
		private double rt;
		private double xicMass1;
		private double xicMass2;
		private int amideHydrogenTotal;
		private int aminoAcidTotal;
		private string proteinSource;
		private int peaksInCalculation;
		private string notes;
		private double rtVariance = 0.5;
		private double xicAdjustment;
		private double xicSelectionWidth;
		private int period;
		private XicPeakPickerOption xicPeakPickerOption;
		private double msThreshold;
		private ObservableCollection<FragmentIon> fragmentIonList;

		public Peptide()
		{
			fragmentIonList = new ObservableCollection<FragmentIon>();
		}

		public Peptide(string sequence)
			: this()
		{
			this.sequence = sequence;
		}

		[DataMember]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[DataMember]
		public int Period
		{
			get { return period; }
			set { period = value; }
		}

		[DataMember]
		public int AminoAcidStart
		{
			get { return aminoAcidStart; }
			set { aminoAcidStart = value; }
		}

		[DataMember]
		public int AminoAcidStop
		{
			get { return aminoAcidStop; }
			set { aminoAcidStop = value; }
		}

		[DataMember]
		public string Sequence
		{
			get { return sequence; }
			set { sequence = value; }
		}

		[DataMember]
		public double MonoIsotopicMass
		{
			get { return monoIsotopicMass; }
			set { monoIsotopicMass = value; }
		}

		[DataMember]
		public int ChargeState
		{
			get { return chargeState; }
			set { chargeState = value; }
		}

		[DataMember]
		public double RT
		{
			get { return rt; }
			set { rt = value; }
		}

		[DataMember]
		public int AmideHydrogenTotal
		{
			get { return amideHydrogenTotal; }
			set { amideHydrogenTotal = value; }
		}

		[DataMember]
		public int AminoAcidTotal
		{
			get { return aminoAcidTotal; }
			set { aminoAcidTotal = value; }
		}

		[DataMember]
		public double XicMass1
		{
			get { return xicMass1; }
			set { xicMass1 = value; }
		}

		[DataMember]
		public double XicMass2
		{
			get { return xicMass2; }
			set { xicMass2 = value; }
		}

		[DataMember]
		public string ProteinSource
		{
			get { return proteinSource; }
			set { proteinSource = value; }
		}

		[DataMember]
		public int PeaksInCalculation
		{
			get { return peaksInCalculation; }
			set { peaksInCalculation = value; }
		}

		[DataMember]
		public string Notes
		{
			get { return notes; }
			set { notes = value; }
		}

		[DataMember]
		public double RtVariance
		{
			get { return rtVariance; }
			set { rtVariance = value; }
		}

		[DataMember]
		public double XicAdjustment
		{
			get { return xicAdjustment; }
			set { xicAdjustment = value; }
		}

		[DataMember]
		public double XicSelectionWidth
		{
			get { return xicSelectionWidth; }
			set { xicSelectionWidth = value; }
		}

		[DataMember]
		public XicPeakPickerOption XicPeakPickerOption
		{
			get { return xicPeakPickerOption; }
			set { xicPeakPickerOption = value; }
		}

		[DataMember]
		public double MsThreshold
		{
			get { return msThreshold; }
			set { msThreshold = value; }
		}

		[DataMember]
		public ObservableCollection<FragmentIon> FragmentIonList
		{
			get { return fragmentIonList; }
			set { fragmentIonList = value; }
		}

		// between 0 and 100% (relative intensity)
		[DataMember]
		public double DeuteriumDistributionThreshold { get; set; }

		// between 0 and 100 pads
		[DataMember]
		public int DeuteriumDistributionRightPadding { get; set; }

		public override string ToString()
		{
			return this.AminoAcidStart.ToString(CultureInfo.InvariantCulture) + "-" + this.AminoAcidStop.ToString(CultureInfo.InvariantCulture);
		}
	}
}
