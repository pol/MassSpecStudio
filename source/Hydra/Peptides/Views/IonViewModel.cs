using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MassSpecStudio.Core;
using ProteinCalc;

namespace Hydra.Modules.Peptides.Views
{
	public class IonViewModel : ViewModelBase
	{
		private Core.Domain.Peptide _peptide;
		private ObservableCollection<Core.Domain.FragmentIon> _fragmentList;
		private bool _isChecked;

		public FragmentIon Ion { get; set; }

		public bool IsChecked
		{
			get
			{
				return _isChecked;
			}

			set
			{
				if (value != _isChecked)
				{
					_isChecked = value;
					if (value)
					{
						FragmentCheck(Ion);
					}
					else
					{
						FragmentUnchecked(Ion);
					}
				}
				NotifyPropertyChanged(() => IsChecked);
			}
		}

		public static IList<IonViewModel> CreateList(IList<B_Ion> fragmentIons, ObservableCollection<Core.Domain.FragmentIon> fragmentList, Core.Domain.Peptide peptide)
		{
			IList<IonViewModel> ionList = new List<IonViewModel>();
			foreach (FragmentIon ion in fragmentIons)
			{
				ionList.Add(new IonViewModel() { Ion = ion, _isChecked = fragmentList.Where(data => data.MZ == ion.MonoIsotopicMass).Any(), _fragmentList = fragmentList, _peptide = peptide });
			}
			return ionList;
		}

		public static IList<IonViewModel> CreateList(IList<Y_Ion> fragmentIons, ObservableCollection<Core.Domain.FragmentIon> fragmentList, Core.Domain.Peptide peptide)
		{
			IList<IonViewModel> ionList = new List<IonViewModel>();
			foreach (FragmentIon ion in fragmentIons)
			{
				ionList.Add(new IonViewModel() { Ion = ion, _isChecked = fragmentList.Where(data => data.MZ == ion.MonoIsotopicMass).Any(), _fragmentList = fragmentList, _peptide = peptide });
			}
			return ionList;
		}

		private void FragmentCheck(FragmentIon sourceFragmentIon)
		{
			if (!_fragmentList.Where(ion => ion.MZ == sourceFragmentIon.MonoIsotopicMass).Any())
			{
				Core.Domain.FragmentIon fragmentIon = new Core.Domain.FragmentIon();

				fragmentIon.ChargeState = sourceFragmentIon.ChargeState;
				fragmentIon.Peptide = _peptide;
				fragmentIon.IonSeriesNumber = sourceFragmentIon.SeriesNumber;

				switch (sourceFragmentIon.FragmentIonType)
				{
					case "b":
						fragmentIon.FragmentIonType = Core.FragmentIonType.BFragment;
						break;
					case "y":
						fragmentIon.FragmentIonType = Core.FragmentIonType.YFragment;
						break;
					case "parent":
						fragmentIon.FragmentIonType = Core.FragmentIonType.Parent;
						break;
					default:
						fragmentIon.FragmentIonType = Core.FragmentIonType.Undefined;
						break;
				}

				fragmentIon.Sequence = sourceFragmentIon.Sequence;
				fragmentIon.MZ = sourceFragmentIon.MonoIsotopicMass;
				fragmentIon.PeaksInCalculation = 2;
				fragmentIon.Id = Guid.NewGuid();

				_fragmentList.Add(fragmentIon);
			}
		}

		private void FragmentUnchecked(FragmentIon sourceFragmentIon)
		{
			Core.Domain.FragmentIon fragmentIon = _fragmentList.Where(ion => ion.MZ == sourceFragmentIon.MonoIsotopicMass).FirstOrDefault();

			if (fragmentIon != null)
			{
				_fragmentList.Remove(fragmentIon);
			}
		}
	}
}
