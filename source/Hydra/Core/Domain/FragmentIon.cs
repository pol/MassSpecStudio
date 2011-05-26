using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;

namespace Hydra.Core.Domain
{
	[DataContract]
	public class FragmentIon
	{
		private Guid _id;
		private double _mz;
		private int _chargeState;
		private int _peaksInCalculation;
		private string _sequence;
		private string _notes;
		private Peptide _peptide;
		private Hydra.Core.FragmentIonType _fragmentIonType;
		private double _msThreshold;

		public FragmentIon(string sequence, Peptide peptide)
		{
			this._sequence = sequence;
			this._peptide = peptide;
		}

		public FragmentIon()
		{
		}

		[DataMember]
		[Browsable(false)]
		public Guid Id
		{
			get { return _id; }
			set { _id = value; }
		}

		[DataMember]
		[DisplayName("Ion Series Name")]
		[Category("Common Information")]
		public int IonSeriesNumber { get; set; }     // eg) refers to the numeric part of b4++

		[DataMember]
		[Category("Common Information")]
		public string Sequence
		{
			get { return _sequence; }
			set { _sequence = value; }
		}

		[DataMember]
		[DisplayName("Type")]
		[Category("Common Information")]
		public Hydra.Core.FragmentIonType FragmentIonType
		{
			get { return _fragmentIonType; }
			set { _fragmentIonType = value; }
		}

		[Browsable(false)]
		public Peptide Peptide
		{
			get { return _peptide; }
			set { _peptide = value; }
		}

		[DataMember]
		[DisplayName("MS Threshold")]
		[Category("Common Information")]
		public double MsThreshold
		{
			get { return _msThreshold; }
			set { _msThreshold = value; }
		}

		[DataMember]
		[Category("Common Information")]
		public string Notes
		{
			get { return _notes; }
			set { _notes = value; }
		}

		[DataMember]
		[Category("Common Information")]
		public double MZ
		{
			get { return _mz; }
			set { _mz = value; }
		}

		[DataMember]
		[DisplayName("Charge State")]
		[Category("Common Information")]
		public int ChargeState
		{
			get { return _chargeState; }
			set { _chargeState = value; }
		}

		[DataMember]
		[DisplayName("Peaks In Calculation")]
		[Category("Peaks")]
		public int PeaksInCalculation
		{
			get { return _peaksInCalculation; }
			set { _peaksInCalculation = value; }
		}

		// between 0 and 100% (relative intensity)
		[DataMember]
		[DisplayName("Threshold")]
		[Category("Deuterium Distribution")]
		public double DeutDistThreshold { get; set; }

		// between 0 and 100 pads
		[DataMember]
		[DisplayName("RT Padding")]
		[Category("Deuterium Distribution")]
		public int DeutDistRightPadding { get; set; }

		public override string ToString()
		{
			return this.FragmentIonType.ToString() + this.IonSeriesNumber.ToString(CultureInfo.InvariantCulture) + GetChargeStateSymbol(this._chargeState);
		}

		private static string GetChargeStateSymbol(int chargeState)
		{
			switch (chargeState)
			{
				case 1:
					return "+";
				case 2:
					return "++";
				case 3:
					return "+++";
				case -1:
					return "-";
				case -2:
					return "--";
				case -3:
					return "---";
				default:
					return string.Empty;
			}
		}
	}
}
