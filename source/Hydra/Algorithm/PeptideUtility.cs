using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Hydra.Core.Domain;
using MassSpecStudio.Core.Domain;
using MwtWinDll;

namespace Hydra.Processing.Algorithm
{
	public class PeptideUtility
	{
		private MolecularWeightCalculator mw = new MolecularWeightCalculator();

		public static int GetNumberOfAmideHydrogens(string sequence)
		{
			int total = 0;

			foreach (char c in sequence)
			{
				// everything except proline.  note... sequence validation to be done elsewhere 
				Regex r = new Regex("[A-OQ-Z]");
				Match m = r.Match(Convert.ToString(c, CultureInfo.InvariantCulture));
				if (m.Success)
				{
					total++;
				}
			}

			// if there are n residues, there are n-1 amide hydrogens
			total -= 1;
			return total;
		}

		public static double CalculateMR(double mz, int z)
		{
			double mr = (mz * z) - (z * 1.00794);
			return mr;
		}

		public static string CleanupPeptideSequence(string sequence)
		{
			// will deal with peptides like F.SAMPLER.G
			if (sequence.Contains(@"."))
			{
				string[] s = sequence.Split('.');

				// in this case, only one '.' so will return a null
				if (s.GetLength(0) == 2)
				{
					return null;
				}
				else
					if (s.GetLength(0) == 3)
					{
						// this is the proper form. will return the middle segment
						return s[1];
					}
					else
					{
						// all other cases
						return null;
					}
			}
			else
			{
				// simply return the sequence if no '.' are present
				return sequence;
			}
		}
		
		public static bool IsPeptideSequenceValid(string sequence)
		{
			foreach (char c in sequence)
			{
				// these one letter amino acid symbols are valid possibilities  
				Regex r = new Regex("[ARNDCEQGHILKMFPSTWYV]");
				Match m = r.Match(Convert.ToString(c, CultureInfo.InvariantCulture));

				// can't get a match so sequence is not valid. 
				if (m.Success == false)
				{
					return false;
				}
			}
			return true;
		}

		// TODO: This method is redudant with the one below.  Collapse it so it uses the method below.
		public void GetIsotopicProfile(RunResult result, bool addProton, int numberOfIsotopesInProfile, bool addNTerminalProteon, bool addCTerminalFreeAcid)
		{
			Peptide peptide = result.Peptide;
			if (IsPeptideSequenceValid(peptide.Sequence))
			{
				// the dimensions change once used by the MolecularWeightCalculator
				double[,] isotopeData = new double[100, 2];

				string threeLetterCodedPeptide = GetPeptideThreeLetterCode(peptide.Sequence, false, addNTerminalProteon, addCTerminalFreeAcid, string.Empty);

				if (addProton)
				{
					threeLetterCodedPeptide += "H";
				}

				string resultsString = string.Empty;
				int numResults = 0;
				mw.ComputeIsotopicAbundances(ref threeLetterCodedPeptide, (short)peptide.ChargeState, ref resultsString, ref isotopeData, ref numResults, string.Empty, string.Empty, string.Empty, string.Empty);

				for (int i = 1; i < isotopeData.GetLength(0); i++)
				{
					MSPeak peak = new MSPeak();
					peak.MZ = isotopeData[i, 0];
					peak.Intensity = isotopeData[i, 1];
					result.TheoreticalIsotopicPeakList.Add(peak);
				}

				if (result.TheoreticalIsotopicPeakList.Count < numberOfIsotopesInProfile)
				{
					for (int i = result.TheoreticalIsotopicPeakList.Count; i < numberOfIsotopesInProfile; i++)
					{
						result.TheoreticalIsotopicPeakList.Add(new MSPeak(0, 0, 0));
					}
				}
			}
		}

		public IList<MSPeak> GetIsotopicProfile(string peptideSequence, int chargeState, bool addProton, int numberOfIsotopesInProfile, bool addNTerminalProteon, bool addCTerminalFreeAcid, string tag)
		{
			if (IsPeptideSequenceValid(peptideSequence))
			{
				// the dimensions change once used by the MolecularWeightCalculator
				double[,] isotopeData = new double[100, 2];
				string resultsString = string.Empty;
				int numResults = 0;

				IList<MSPeak> theoreticalIsotopicProfilePeaks = new List<MSPeak>();
				string threeLetterCodedPeptide = GetPeptideThreeLetterCode(peptideSequence, false, addNTerminalProteon, addCTerminalFreeAcid, string.Empty);

				if (addProton)
				{
					threeLetterCodedPeptide += "H";
				}
				threeLetterCodedPeptide = threeLetterCodedPeptide + tag;

				mw.ComputeIsotopicAbundances(ref threeLetterCodedPeptide, (short)chargeState, ref resultsString, ref isotopeData, ref numResults, string.Empty, string.Empty, string.Empty, string.Empty);

				for (int i = 1; i < isotopeData.GetLength(0); i++)
				{
					MSPeak peak = new MSPeak();
					peak.MZ = isotopeData[i, 0];
					peak.Intensity = isotopeData[i, 1];
					theoreticalIsotopicProfilePeaks.Add(peak);
				}

				if (theoreticalIsotopicProfilePeaks.Count < numberOfIsotopesInProfile)
				{
					for (int i = theoreticalIsotopicProfilePeaks.Count; i < numberOfIsotopesInProfile; i++)
					{
						theoreticalIsotopicProfilePeaks.Add(new MSPeak(0, 0, 0));
					}
				}
				return theoreticalIsotopicProfilePeaks;
			}
			return null;
		}

		public string GetPeptideThreeLetterCode(string sequence, bool addProton, bool addNTerminalProton, bool addFreeAcid, string tag)
		{
			System.Text.StringBuilder sb = new StringBuilder();
			if (addNTerminalProton)
			{
				sb.Append("H");
			}
			foreach (char c in sequence)
			{
				bool convertToThreeLetterCode = true;
				string testStr = Convert.ToString(c, CultureInfo.InvariantCulture);
				string threeLetterCode = mw.GetAminoAcidSymbolConversion(testStr, convertToThreeLetterCode);
				sb.Append(threeLetterCode);
			}
			if (addFreeAcid)
			{
				sb.Append("OH");
			}
			if (addProton)
			{
				sb.Append("H");
			}

			if (tag != null)
			{
				sb.Append(tag);
			}
			return sb.ToString();
		}

		public void GetIsotopicProfileForFragmentIon(RunResult result, int numberOfIsotopesInProfile)
		{
			if (IsPeptideSequenceValid(result.Peptide.Sequence))
			{
				// the dimensions change once used by the MolecularWeightCalculator
				double[,] isotopeData = new double[100, 2];

				string threeLetterCodedPeptide;
				switch (result.FragmentIon.FragmentIonType)
				{
					case Hydra.Core.FragmentIonType.Parent:
						threeLetterCodedPeptide = GetPeptideThreeLetterCode(result.FragmentIon.Sequence, true, true, true, string.Empty);
						break;
					case Hydra.Core.FragmentIonType.BFragment:
						threeLetterCodedPeptide = GetPeptideThreeLetterCode(result.FragmentIon.Sequence, true, false, false, string.Empty);
						break;
					case Hydra.Core.FragmentIonType.YFragment:
						threeLetterCodedPeptide = GetPeptideThreeLetterCode(result.FragmentIon.Sequence, true, true, true, string.Empty);
						break;
					case Hydra.Core.FragmentIonType.AFragment:
						throw new System.ArgumentException("Code not yet written for a-ions");
					case Hydra.Core.FragmentIonType.BMinusH2O:
						threeLetterCodedPeptide = GetPeptideThreeLetterCode(result.FragmentIon.Sequence, true, false, false, ">H2O");

						break;
					case Hydra.Core.FragmentIonType.YMinusH2O:
						threeLetterCodedPeptide = GetPeptideThreeLetterCode(result.FragmentIon.Sequence, true, true, true, ">H2O");

						break;
					case Hydra.Core.FragmentIonType.BMinusNH3:
						threeLetterCodedPeptide = GetPeptideThreeLetterCode(result.FragmentIon.Sequence, true, false, false, ">NH3");

						break;
					case Hydra.Core.FragmentIonType.YMinusNH3:
						threeLetterCodedPeptide = GetPeptideThreeLetterCode(result.FragmentIon.Sequence, true, true, true, ">NH3");

						break;
					default:
						throw new System.ArgumentException("Invalid fragment ion Type");
				}

				string resultsString = string.Empty;
				int numResults = 0;
				mw.ComputeIsotopicAbundances(ref threeLetterCodedPeptide, (short)result.FragmentIon.ChargeState, ref resultsString, ref isotopeData, ref numResults, string.Empty, string.Empty, string.Empty, string.Empty);

				for (int i = 1; i < isotopeData.GetLength(0); i++)
				{
					MSPeak peak = new MSPeak();
					peak.MZ = isotopeData[i, 0];
					peak.Intensity = isotopeData[i, 1];
					result.TheoreticalIsotopicPeakList.Add(peak);
				}

				if (result.TheoreticalIsotopicPeakList.Count < numberOfIsotopesInProfile)
				{
					for (int i = result.TheoreticalIsotopicPeakList.Count; i < numberOfIsotopesInProfile; i++)
					{
						result.TheoreticalIsotopicPeakList.Add(new MSPeak(0, 0, 0));
					}
				}
			}
		}
	}
}