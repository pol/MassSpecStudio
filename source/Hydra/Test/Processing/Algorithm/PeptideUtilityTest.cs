using System;
using System.Collections.Generic;
using Hydra.Core;
using Hydra.Core.Domain;
using Hydra.Processing.Algorithm;
using MassSpecStudio.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hydra.Test.Processing.Algorithm
{
	[TestClass]
	public class PeptideUtilityTest
	{
		private PeptideUtility peptideUtility = new PeptideUtility();

		[TestMethod]
		public void GetNumberOfAmideHydrogens()
		{
			Assert.AreEqual(5, PeptideUtility.GetNumberOfAmideHydrogens("SAMPLER"));
		}

		[TestMethod]
		public void CalculateMR()
		{
			Assert.AreEqual(418.98412, PeptideUtility.CalculateMR(210.5, 2));
		}

		[TestMethod]
		public void CleanupPeptideSequence()
		{
			Assert.AreEqual("SAMPLER", PeptideUtility.CleanupPeptideSequence("F.SAMPLER.G"));
		}

		[TestMethod]
		public void CleanupPeptideSequenceWithInvalidSequences()
		{
			Assert.AreEqual(null, PeptideUtility.CleanupPeptideSequence("F.SAMPLER"));
			Assert.AreEqual(null, PeptideUtility.CleanupPeptideSequence("F.SAMPLER.G.T"));
			Assert.AreEqual("SAMPLER", PeptideUtility.CleanupPeptideSequence("SAMPLER"));
		}

		[TestMethod]
		public void CleanupPeptideSequenceWithNoSequence()
		{
			Assert.AreEqual(string.Empty, PeptideUtility.CleanupPeptideSequence(string.Empty));
		}

		[TestMethod]
		public void IsPeptideSequenceValid()
		{
			bool testPeptide1 = PeptideUtility.IsPeptideSequenceValid("SAMPLER");
			bool testPeptide2 = PeptideUtility.IsPeptideSequenceValid("ZAMPLER");

			Assert.AreEqual(true, testPeptide1);
			Assert.AreEqual(false, testPeptide2);
		}

		[TestMethod]
		public void IsPeptideValidSequenceLengthIsZeroException()
		{
			bool testPeptide3 = PeptideUtility.IsPeptideSequenceValid(string.Empty);
		}

		[TestMethod]
		public void GetIsotopicProfileUsingRunResult()
		{
			Peptide peptide = new Peptide("SAMPLER");
			peptide.ChargeState = 2;
			RunResult runResult = new RunResult(peptide, null);
			peptideUtility.GetIsotopicProfile(runResult, true, 20, true, true);

			Assert.AreEqual(402.711655, runResult.TheoreticalIsotopicPeakList[0].MZ);
			Assert.AreEqual(100, runResult.TheoreticalIsotopicPeakList[0].Intensity);
			Assert.AreEqual(403.211655, runResult.TheoreticalIsotopicPeakList[1].MZ);
			Assert.AreEqual(41.490242039325295, runResult.TheoreticalIsotopicPeakList[1].Intensity);
			Assert.AreEqual(20, runResult.TheoreticalIsotopicPeakList.Count);
		}

		[TestMethod]
		public void GetIsotopicProfileUsingRunResultWithInvalidSequence()
		{
			Peptide peptide = new Peptide("ZAMPLER");
			peptide.ChargeState = 2;
			RunResult runResult = new RunResult(peptide, null);
			peptideUtility.GetIsotopicProfile(runResult, true, 20, true, true);

			Assert.AreEqual(0, runResult.TheoreticalIsotopicPeakList.Count);
		}

		[TestMethod]
		public void GetIsotopicProfileUsingSequence()
		{
			IList<MSPeak> theoreticalIsotopicProfile = peptideUtility.GetIsotopicProfile("SAMPLER", 2, true, 20, true, true, string.Empty);

			Assert.AreEqual(402.711655, theoreticalIsotopicProfile[0].MZ);
			Assert.AreEqual(100, theoreticalIsotopicProfile[0].Intensity);
			Assert.AreEqual(403.211655, theoreticalIsotopicProfile[1].MZ);
			Assert.AreEqual(41.490242039325295, theoreticalIsotopicProfile[1].Intensity);
			Assert.AreEqual(20, theoreticalIsotopicProfile.Count);
		}

		[TestMethod]
		public void GetIsotopicProfileUsingSequenceWithInvalidSequence()
		{
			IList<MSPeak> theoreticalIsotopicProfile = peptideUtility.GetIsotopicProfile("ZAMPLER", 2, true, 20, true, true, string.Empty);

			Assert.AreEqual(null, theoreticalIsotopicProfile);
		}

		[TestMethod]
		public void GetPeptideThreeLetterCode()
		{
			string threeLetterCodedPeptide = peptideUtility.GetPeptideThreeLetterCode("SAMPLER", false, false, false, string.Empty);
			Assert.AreEqual("SerAlaMetProLeuGluArg", threeLetterCodedPeptide);
		}

		[TestMethod]
		public void GetPeptideThreeLetterCodeAndAddNTerminalProton()
		{
			string threeLetterCodedPeptide = peptideUtility.GetPeptideThreeLetterCode("SAMPLER", false, true, false, string.Empty);
			Assert.AreEqual("HSerAlaMetProLeuGluArg", threeLetterCodedPeptide);
		}

		[TestMethod]
		public void GetPeptideThreeLetterCodeAndAddFreeAcid()
		{
			string threeLetterCodedPeptide = peptideUtility.GetPeptideThreeLetterCode("SAMPLER", false, false, true, string.Empty);
			Assert.AreEqual("SerAlaMetProLeuGluArgOH", threeLetterCodedPeptide);
		}

		[TestMethod]
		public void GetPeptideThreeLetterCodeAndAddProton()
		{
			string threeLetterCodedPeptide = peptideUtility.GetPeptideThreeLetterCode("SAMPLER", true, false, false, string.Empty);
			Assert.AreEqual("SerAlaMetProLeuGluArgH", threeLetterCodedPeptide);
		}

		[TestMethod]
		public void GetPeptideThreeLetterCodeAndAddTag()
		{
			string threeLetterCodedPeptide = peptideUtility.GetPeptideThreeLetterCode("SAMPLER", false, false, false, "-CJ");
			Assert.AreEqual("SerAlaMetProLeuGluArg-CJ", threeLetterCodedPeptide);
		}

		[TestMethod]
		public void GetPeptideThreeLetterCodeAndAddEverything()
		{
			string threeLetterCodedPeptide = peptideUtility.GetPeptideThreeLetterCode("SAMPLER", true, true, true, "-CJ");
			Assert.AreEqual("HSerAlaMetProLeuGluArgOHH-CJ", threeLetterCodedPeptide);
		}

		[TestMethod]
		public void GetPeptideThreeLetterCodeWithEmptySequence()
		{
			string threeLetterCodedPeptide = peptideUtility.GetPeptideThreeLetterCode(string.Empty, false, false, false, string.Empty);
			Assert.AreEqual(string.Empty, threeLetterCodedPeptide);
		}

		[TestMethod]
		public void GetIsotopicProfileForParentFragmentIon()
		{
			RunResult runResult = CreateRunResult("SA", FragmentIonType.Parent);

			peptideUtility.GetIsotopicProfileForFragmentIon(runResult, 1);
			Assert.AreEqual(6, runResult.TheoreticalIsotopicPeakList.Count);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[0], 177.08753, 100);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[1], 178.08753, 7.57556);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[2], 179.08753, 1.0727);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[3], 180.08753, 0.06671);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[4], 181.08753, 0.00461);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[5], 182.08753, 0.00023);
		}

		[TestMethod]
		public void GetIsotopicProfileForBFragmentIon()
		{
			RunResult runResult = CreateRunResult("SA", FragmentIonType.BFragment);

			peptideUtility.GetIsotopicProfileForFragmentIon(runResult, 1);
			Assert.AreEqual(6, runResult.TheoreticalIsotopicPeakList.Count);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[0], 159.07696, 100);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[1], 160.07696, 7.50746);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[2], 161.07696, 0.86198);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[3], 162.07696, 0.05063);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[4], 163.07696, 0.0028);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[5], 164.07696, 0.00012);
		}

		[TestMethod]
		public void GetIsotopicProfileForYFragmentIon()
		{
			RunResult runResult = CreateRunResult("SA", FragmentIonType.YFragment);

			peptideUtility.GetIsotopicProfileForFragmentIon(runResult, 1);
			Assert.AreEqual(6, runResult.TheoreticalIsotopicPeakList.Count);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[0], 177.08753, 100);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[1], 178.08753, 7.57556);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[2], 179.08753, 1.0727);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[3], 180.08753, 0.06671);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[4], 181.08753, 0.00461);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[5], 182.08753, 0.00023);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void GetIsotopicProfileForAFragmentFragmentIon()
		{
			RunResult runResult = CreateRunResult("SA", FragmentIonType.AFragment);

			peptideUtility.GetIsotopicProfileForFragmentIon(runResult, 1);
		}

		[TestMethod]
		public void GetIsotopicProfileForBMinusH2OFragmentIon()
		{
			RunResult runResult = CreateRunResult("SA", FragmentIonType.BMinusH2O);

			peptideUtility.GetIsotopicProfileForFragmentIon(runResult, 1);
			Assert.AreEqual(6, runResult.TheoreticalIsotopicPeakList.Count);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[0], 177.08753, 100);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[1], 178.08753, 7.57556);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[2], 179.08753, 1.0727);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[3], 180.08753, 0.06671);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[4], 181.08753, 0.00461);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[5], 182.08753, 0.00023);
		}

		[TestMethod]
		public void GetIsotopicProfileForYMinusH2OFragmentIon()
		{
			RunResult runResult = CreateRunResult("SA", FragmentIonType.YMinusH2O);

			peptideUtility.GetIsotopicProfileForFragmentIon(runResult, 1);
			Assert.AreEqual(6, runResult.TheoreticalIsotopicPeakList.Count);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[0], 195.09809, 100);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[1], 196.09809, 7.64365);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[2], 197.09809, 1.28349);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[3], 198.09809, 0.08308);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[4], 199.09809, 0.00686);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[5], 200.09809, 0.00036);
		}

		[TestMethod]
		public void GetIsotopicProfileForBMinusNH3FragmentIon()
		{
			RunResult runResult = CreateRunResult("SA", FragmentIonType.BMinusNH3);

			peptideUtility.GetIsotopicProfileForFragmentIon(runResult, 1);
			Assert.AreEqual(6, runResult.TheoreticalIsotopicPeakList.Count);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[0], 176.10351, 100);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[1], 177.10351, 7.92182);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[2], 178.10351, 0.89334);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[3], 179.10351, 0.05422);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[4], 180.10351, 0.00301);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[5], 181.10351, 0.00013);
		}

		[TestMethod]
		public void GetIsotopicProfileForYMinusNH3FragmentIon()
		{
			RunResult runResult = CreateRunResult("SA", FragmentIonType.YMinusNH3);

			peptideUtility.GetIsotopicProfileForFragmentIon(runResult, 1);
			Assert.AreEqual(6, runResult.TheoreticalIsotopicPeakList.Count);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[0], 194.11408, 100);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[1], 195.11408, 7.98992);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[2], 196.11408, 1.10436);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[3], 197.11408, 0.07117);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[4], 198.11408, 0.00489);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[5], 199.11408, 0.00025);
		}

		[TestMethod]
		public void GetIsotopicProfileForFragmentIonWithInvalidSequence()
		{
			RunResult runResult = CreateRunResult("ZA", FragmentIonType.Parent);

			peptideUtility.GetIsotopicProfileForFragmentIon(runResult, 5);
			Assert.AreEqual(0, runResult.TheoreticalIsotopicPeakList.Count);
		}

		[TestMethod]
		public void GetIsotopicProfileForFragmentIonWithEtrxaIsotopesInProfile()
		{
			RunResult runResult = CreateRunResult("S", FragmentIonType.Parent);

			peptideUtility.GetIsotopicProfileForFragmentIon(runResult, 7);
			Assert.AreEqual(7, runResult.TheoreticalIsotopicPeakList.Count);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[0], 106.05042, 100);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[1], 107.05042, 3.84837);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[2], 108.05042, 0.67239);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[3], 109.05042, 0.02387);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[4], 110.05042, 0.0016);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[5], 0, 0);
			AssertMSPeak(runResult.TheoreticalIsotopicPeakList[6], 0, 0);
		}

		private RunResult CreateRunResult(string sequence, FragmentIonType fragmentIonType)
		{
			Peptide peptide = new Peptide(sequence);
			RunResult runResult = new RunResult(new FragmentIon(sequence, peptide), null);
			runResult.FragmentIon.FragmentIonType = fragmentIonType;

			return runResult;
		}

		private void AssertMSPeak(MSPeak peak, double expectedMz, double expectedIntensity)
		{
			Assert.AreEqual(expectedMz, peak.MZ);
			Assert.AreEqual(Math.Round(expectedIntensity, 5), Math.Round(peak.Intensity, 5));
		}
	}
}
