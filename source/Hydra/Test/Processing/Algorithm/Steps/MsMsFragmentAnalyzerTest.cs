using System;
using Hydra.Core.Domain;
using Hydra.Processing.Algorithm.Steps;
using MassSpecStudio.Core.Domain;
using MassSpecStudio.Core.Events;
using Microsoft.Practices.Prism.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Hydra.Test.Processing.Algorithm.Steps
{
	[TestClass]
	public class MsMsFragmentAnalyzerTest
	{
		private MsMsFragmentAnalyzer msmsFragmentAnalyzer;
		private Mock<IEventAggregator> mockEventAggregator;
		private OutputEvent outputEvent;
		private bool publishCalled = false;
		private int messageCount = 0;

		[TestInitialize]
		public void MyTestInitialize()
		{
			mockEventAggregator = new Mock<IEventAggregator>();

			outputEvent = new OutputEvent();
			mockEventAggregator.Setup(mock => mock.GetEvent<OutputEvent>()).Returns(outputEvent);
			msmsFragmentAnalyzer = new MsMsFragmentAnalyzer(mockEventAggregator.Object);
		}

		[TestMethod]
		public void ExecuteWithFragment()
		{
			Peptide peptide = CreatePeptide();
			RunResult runResult = new RunResult(peptide.FragmentIonList[0], null);
			outputEvent.Subscribe(OnPublish);

			msmsFragmentAnalyzer.Execute(runResult, GenerateXYData());

			AssertPeptide(peptide);

			Assert.AreEqual(0, runResult.IsotopicPeakList.Count);
			Assert.AreEqual(151.58971, Math.Round(runResult.AverageMass, 5));
			Assert.AreEqual(152.1611, Math.Round(runResult.TheoreticalAverageMass, 5));
			Assert.AreEqual(-1.14279, Math.Round(runResult.AmountDeut, 5));
			Assert.AreEqual(true, runResult.IsUsedInCalculations);
			Assert.AreEqual(0, Math.Round(runResult.AmountDeutFromDeutDist, 5));
			Assert.AreEqual(true, publishCalled);
			Assert.AreEqual(46, messageCount);
		}

		[TestMethod]
		public void ExecuteWithPeptide()
		{
			Peptide peptide = CreatePeptide();
			RunResult runResult = new RunResult(peptide, null);
			runResult.FragmentIon = peptide.FragmentIonList[0];
			outputEvent.Subscribe(OnPublish);

			msmsFragmentAnalyzer.Execute(runResult, GenerateXYData());

			AssertPeptide(peptide);

			Assert.AreEqual(0, runResult.IsotopicPeakList.Count);
			Assert.AreEqual(151.58971, Math.Round(runResult.AverageMass, 5));
			Assert.AreEqual(850.9536, Math.Round(runResult.TheoreticalAverageMass, 5));
			Assert.AreEqual(-1.14279, Math.Round(runResult.AmountDeut, 5));
			Assert.AreEqual(true, runResult.IsUsedInCalculations);
			Assert.AreEqual(0, Math.Round(runResult.AmountDeutFromDeutDist, 5));
			Assert.AreEqual(true, publishCalled);
			Assert.AreEqual(46, messageCount);
		}

		private void OnPublish(string actualValue)
		{
			publishCalled = true;
			messageCount++;
			switch (messageCount - 1)
			{
				case 0:
					Assert.AreEqual("     MSMS Fragment Analyzer", actualValue);
					break;
				case 1:
					Assert.AreEqual("     Executing Spectral Peak Detection", actualValue);
					break;
				case 2:
					Assert.AreEqual("     Spectral Peak Detection Started (PeakToBackgroundRatio=2, SignalToNoiseThreshold=3)", actualValue);
					break;
				case 3:
					Assert.AreEqual("     0 peaks found.", actualValue);
					break;
				case 4:
					Assert.AreEqual("     Executing Isotopic Profile Finder", actualValue);
					break;
				case 5:
					Assert.AreEqual("     Isotopic Profile Finder Started (MassRange=0.1, PeakWidthMinimum=0, PeakWidthMaximum=0.5, IntensityThreshold=0, PeakNumberMaximum=20, MSPeakSelectionOption=MostIntenseWithinMzVariation)", actualValue);
					break;
			}
		}

		private void AssertPeptide(Peptide peptide)
		{
			Assert.AreEqual(849.47, peptide.MonoIsotopicMass);
			Assert.AreEqual(1, peptide.ChargeState);
			Assert.AreEqual(5, peptide.PeaksInCalculation);
			Assert.AreEqual(0, peptide.MsThreshold);
			Assert.AreEqual(2, peptide.DeuteriumDistributionRightPadding);
			Assert.AreEqual(1, peptide.DeuteriumDistributionThreshold);
		}

		private IXYData GenerateXYData()
		{
			return new XYData(new double[] { 1, 2, 3, 4, 5 }, new double[] { 2, 2, 2, 2, 2 });
		}

		private Peptide CreatePeptide()
		{
			Peptides peptides = Peptides.Open(Properties.Settings.Default.PeptideTestFile2);

			////Peptide peptide = new Peptide("SAMPLER");
			////peptide.MonoIsotopicMass = 500;
			////peptide.ChargeState = 1;
			////peptide.FragmentIonList.Add(new FragmentIon("SA", peptide) { FragmentIonType = Hydra.Core.FragmentIonType.BFragment });
			////peptide.FragmentIonList.Add(new FragmentIon("MP", peptide) { FragmentIonType = Hydra.Core.FragmentIonType.BFragment });
			////return peptide;
			return peptides.PeptideCollection[0];
		}
	}
}
