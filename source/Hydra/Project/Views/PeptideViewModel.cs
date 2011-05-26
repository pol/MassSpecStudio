using System.ComponentModel;
using Hydra.Core;
using Hydra.Core.Domain;
using MassSpecStudio.Core;

namespace Hydra.Modules.Project.Views
{
	[DefaultProperty("Sequence")]
	public class PeptideViewModel : TreeViewItemBase<PeptidesViewModel, PeptideViewModel>
	{
		private Peptide _peptide;

		public PeptideViewModel(Peptide peptide)
		{
			_peptide = peptide;
			Data = peptide;
		}

		[Browsable(false)]
		public override string Name
		{
			get
			{
				return _peptide.Sequence;
			}

			set
			{
			}
		}

		[Category("Basic Information")]
		public int Start
		{
			get
			{
				return _peptide.AminoAcidStart;
			}

			set
			{
				_peptide.AminoAcidStart = value;
				NotifyPropertyChanged(() => Start);
			}
		}

		[Category("Basic Information")]
		public int Stop
		{
			get
			{
				return _peptide.AminoAcidStop;
			}

			set
			{
				_peptide.AminoAcidStop = value;
				NotifyPropertyChanged(() => Stop);
			}
		}

		[Category("Basic Information")]
		public string Sequence
		{
			get
			{
				return _peptide.Sequence;
			}

			set
			{
				_peptide.Sequence = value;
				NotifyPropertyChanged(() => Sequence);
				NotifyPropertyChanged(() => Name);
			}
		}

		[Category("Basic Information")]
		[DisplayName("M/Z")]
		public double MZ
		{
			get
			{
				return _peptide.MonoIsotopicMass;
			}

			set
			{
				_peptide.MonoIsotopicMass = value;
				NotifyPropertyChanged(() => MZ);
			}
		}

		[Category("Basic Information")]
		[DisplayName("Z")]
		public int ChargeState
		{
			get
			{
				return _peptide.ChargeState;
			}

			set
			{
				_peptide.ChargeState = value;
				NotifyPropertyChanged(() => ChargeState);
			}
		}

		[Category("Basic Information")]
		public string Notes
		{
			get
			{
				return _peptide.Notes;
			}

			set
			{
				_peptide.Notes = value;
				NotifyPropertyChanged(() => Notes);
			}
		}

		[Category("XIC - Related")]
		public int Period
		{
			get
			{
				return _peptide.Period;
			}

			set
			{
				_peptide.Period = value;
				NotifyPropertyChanged(() => Period);
			}
		}

		[Category("XIC - Related")]
		[DisplayName("Peak Picking")]
		public XicPeakPickerOption PeakPicking
		{
			get
			{
				return _peptide.XicPeakPickerOption;
			}

			set
			{
				_peptide.XicPeakPickerOption = value;
				NotifyPropertyChanged(() => PeakPicking);
			}
		}

		[Category("XIC - Related")]
		public double RT
		{
			get
			{
				return _peptide.RT;
			}

			set
			{
				_peptide.RT = value;
				NotifyPropertyChanged(() => RT);
			}
		}

		[Category("XIC - Related")]
		[DisplayName("RT Variance")]
		public double RTVariance
		{
			get
			{
				return _peptide.RtVariance;
			}

			set
			{
				_peptide.RtVariance = value;
				NotifyPropertyChanged(() => RTVariance);
			}
		}

		[Category("XIC - Related")]
		public double Adjust
		{
			get
			{
				return _peptide.XicAdjustment;
			}

			set
			{
				_peptide.XicAdjustment = value;
				NotifyPropertyChanged(() => Adjust);
			}
		}

		[Category("XIC - Related")]
		[DisplayName("Selection Width")]
		public double SelectionWidth
		{
			get
			{
				return _peptide.XicSelectionWidth;
			}

			set
			{
				_peptide.XicSelectionWidth = value;
				NotifyPropertyChanged(() => SelectionWidth);
			}
		}

		[Category("MS - Related")]
		[DisplayName("# of Peaks")]
		public int NumberOfPeaks
		{
			get
			{
				return _peptide.PeaksInCalculation;
			}

			set
			{
				_peptide.PeaksInCalculation = value;
				NotifyPropertyChanged(() => NumberOfPeaks);
			}
		}

		[Category("Deuterium Distribution")]
		public double Threshold
		{
			get
			{
				return _peptide.DeuteriumDistributionThreshold;
			}

			set
			{
				_peptide.DeuteriumDistributionThreshold = value;
				NotifyPropertyChanged(() => Threshold);
			}
		}

		[Category("Deuterium Distribution")]
		[DisplayName("RT Padding")]
		public int RTPadding
		{
			get
			{
				return _peptide.DeuteriumDistributionRightPadding;
			}

			set
			{
				_peptide.DeuteriumDistributionRightPadding = value;
				NotifyPropertyChanged(() => RTPadding);
			}
		}
	}
}
