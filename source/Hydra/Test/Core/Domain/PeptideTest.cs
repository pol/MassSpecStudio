using Hydra.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hydra.Test.Core.Domain
{
	[TestClass]
	public class PeptideTest
	{
		private Peptide peptide;

		[TestInitialize]
		public void TestInitialize()
		{
			peptide = new Peptide("SAMPLE");
		}

		[TestMethod]
		public void MyTestMethod()
		{
			Assert.AreEqual("SAMPLE", peptide.Sequence);
			Assert.AreEqual(0, peptide.FragmentIonList.Count);
		}

		[TestMethod]
		public void Properties()
		{
			peptide.Id = 1;
			peptide.Period = 2;
			peptide.AminoAcidStart = 3;
			peptide.AminoAcidStop = 4;
			peptide.AmideHydrogenTotal = 5;
			peptide.AminoAcidTotal = 6;
			peptide.ChargeState = 7;
			peptide.DeuteriumDistributionRightPadding = 8;
			peptide.DeuteriumDistributionThreshold = 9;
			peptide.MonoIsotopicMass = 10;
			peptide.MsThreshold = 11;
			peptide.Notes = "12";
			peptide.PeaksInCalculation = 13;
			peptide.ProteinSource = "14";
			peptide.RT = 15;
			peptide.RtVariance = 16;
			peptide.Sequence = "SAM";
			peptide.XicAdjustment = 17;
			peptide.XicMass1 = 18;
			peptide.XicMass2 = 19;
			peptide.XicPeakPickerOption = Hydra.Core.XicPeakPickerOption.MostIntenseWithinEntireXic;
			peptide.XicSelectionWidth = 20;

			Assert.AreEqual(1, peptide.Id);
			Assert.AreEqual(2, peptide.Period);
			Assert.AreEqual(3, peptide.AminoAcidStart);
			Assert.AreEqual(4, peptide.AminoAcidStop);
			Assert.AreEqual(5, peptide.AmideHydrogenTotal);
			Assert.AreEqual(6, peptide.AminoAcidTotal);
			Assert.AreEqual(7, peptide.ChargeState);
			Assert.AreEqual(8, peptide.DeuteriumDistributionRightPadding);
			Assert.AreEqual(9, peptide.DeuteriumDistributionThreshold);
			Assert.AreEqual(10, peptide.MonoIsotopicMass);
			Assert.AreEqual(11, peptide.MsThreshold);
			Assert.AreEqual("12", peptide.Notes);
			Assert.AreEqual(13, peptide.PeaksInCalculation);
			Assert.AreEqual("14", peptide.ProteinSource);
			Assert.AreEqual(15, peptide.RT);
			Assert.AreEqual(16, peptide.RtVariance);
			Assert.AreEqual("SAM", peptide.Sequence);
			Assert.AreEqual(17, peptide.XicAdjustment);
			Assert.AreEqual(18, peptide.XicMass1);
			Assert.AreEqual(19, peptide.XicMass2);
			Assert.AreEqual(Hydra.Core.XicPeakPickerOption.MostIntenseWithinEntireXic, peptide.XicPeakPickerOption);
			Assert.AreEqual(20, peptide.XicSelectionWidth);
		}

		[TestMethod]
		public void ToStringTest()
		{
			Properties();

			Assert.AreEqual("3-4", peptide.ToString());
		}
	}
}
