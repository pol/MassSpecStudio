using System.Collections.Generic;
using Hydra.Core;
using Hydra.Core.Domain;
using Hydra.Core.Provider;
using Hydra.DataProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hydra.Test.DataProvider
{
	[TestClass]
	public class CsvPeptideDataProviderTest
	{
		private IPeptideDataProvider peptideDataProvider;

		[TestInitialize]
		public void TestInitialize()
		{
			peptideDataProvider = new CsvPeptideDataProvider();
		}

		[TestMethod]
		public void Read()
		{
			IList<Peptide> peptides = peptideDataProvider.Read(Properties.Settings.Default.PeptideTestFile1);

			Assert.AreEqual(7, peptides.Count);
			AssertPeptide(peptides[0], 0, 103, 113, 0, 2, 2, 1, 7, 647.83, 0, "testnote7", 3, 0, "tubulinA", 3.95, 0.5, "YARGHYTIGKE", 0, 647.83, 647.83, XicPeakPickerOption.ClosestToRTWithinRTVariation, 0.1);
		}

		private void AssertPeptide(
			Peptide peptide,
			int amideHydrogenTotal,
			double aminoAcidStart,
			double aminoAcidStop, 
			int aminoAcidTotal,
			int chargeState,
			int deuteriumDistributionRightPadding,
			double deuteriumDistributionThreshold,
			int id, 
			double monoIsotopicMass,
			double msThreshold,
			string notes,
			int peaksInCalculation, 
			int period, 
			string proteinSource,
			double rt, 
			double rtVariance,
			string sequence, 
			double xicAdjustment,
			double xicMass1,
			double xicMass2, 
			XicPeakPickerOption xicPeakPickerOption,
			double xicSelectionWidth)
		{
			Assert.AreEqual(amideHydrogenTotal, peptide.AmideHydrogenTotal);
			Assert.AreEqual(aminoAcidStart, peptide.AminoAcidStart);
			Assert.AreEqual(aminoAcidStop, peptide.AminoAcidStop);
			Assert.AreEqual(aminoAcidTotal, peptide.AminoAcidTotal);
			Assert.AreEqual(chargeState, peptide.ChargeState);
			Assert.AreEqual(deuteriumDistributionRightPadding, peptide.DeuteriumDistributionRightPadding);
			Assert.AreEqual(deuteriumDistributionThreshold, peptide.DeuteriumDistributionThreshold);
			Assert.AreEqual(id, peptide.Id);
			Assert.AreEqual(monoIsotopicMass, peptide.MonoIsotopicMass);
			Assert.AreEqual(msThreshold, peptide.MsThreshold);
			Assert.AreEqual(notes, peptide.Notes);
			Assert.AreEqual(peaksInCalculation, peptide.PeaksInCalculation);
			Assert.AreEqual(period, peptide.Period);
			Assert.AreEqual(proteinSource, peptide.ProteinSource);
			Assert.AreEqual(rt, peptide.RT);
			Assert.AreEqual(rtVariance, peptide.RtVariance);
			Assert.AreEqual(sequence, peptide.Sequence);
			Assert.AreEqual(xicAdjustment, peptide.XicAdjustment);
			Assert.AreEqual(xicMass1, peptide.XicMass1);
			Assert.AreEqual(xicMass2, peptide.XicMass2);
			Assert.AreEqual(xicPeakPickerOption, peptide.XicPeakPickerOption);
			Assert.AreEqual(xicSelectionWidth, peptide.XicSelectionWidth);
		}
	}
}
