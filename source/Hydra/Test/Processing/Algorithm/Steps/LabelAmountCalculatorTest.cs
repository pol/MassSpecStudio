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
	public class LabelAmountCalculatorTest
	{
		private LabelAmountCalculator labelAmountCalculator;
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
			labelAmountCalculator = new LabelAmountCalculator(mockEventAggregator.Object);
		}

		[TestMethod]
		public void ProcessingParameterValues()
		{
			labelAmountCalculator.Mode = Mode.CalculatedMassAndExperimentalIntensity;
			Assert.AreEqual(Mode.CalculatedMassAndExperimentalIntensity, labelAmountCalculator.Mode);
			labelAmountCalculator.PeaksInCalcMode = Hydra.Core.PeaksInLabelCalculationMode.Manual;
			Assert.AreEqual(Hydra.Core.PeaksInLabelCalculationMode.Manual, labelAmountCalculator.PeaksInCalcMode);
		}

		[TestMethod]
		public void ExecuteWithAutomaticPeaksInLabelCalculationWithRelativeMz()
		{
			Peptide peptide = CreatePeptide();
			RunResult runResult = new RunResult(peptide, null);
			GenerateIsotopicPeakList(runResult);
			GenerateTheoreticalIsotopicPeakList(runResult);
			outputEvent.Subscribe(OnPublishWithAuto);

			labelAmountCalculator.PeaksInCalcMode = Hydra.Core.PeaksInLabelCalculationMode.Automatic;
			labelAmountCalculator.Mode = Mode.CalculatedMassAndExperimentalIntensity;
			labelAmountCalculator.Execute(runResult);

			Assert.AreEqual(500.4375, Math.Round(runResult.AverageMass, 5));
			Assert.AreEqual(500.38875, Math.Round(runResult.TheoreticalAverageMass, 5));
			Assert.AreEqual(499.42956, Math.Round(runResult.CentroidMR, 5));
			Assert.AreEqual(499.38081, Math.Round(runResult.TheoreticalCentroidMR, 5));
			Assert.AreEqual(5, runResult.AmideHydrogenTotal);
			Assert.AreEqual(0.04875, Math.Round(runResult.AmountDeut, 5));
			Assert.AreEqual(true, runResult.IsUsedInCalculations);
			Assert.AreEqual(true, publishCalled);
			Assert.AreEqual(8, messageCount);
		}

		[TestMethod]
		public void ExecuteWithAutomaticPeaksInLabelCalculationWithoutRelativeMz()
		{
			Peptide peptide = CreatePeptide();
			RunResult runResult = new RunResult(peptide, null);
			GenerateIsotopicPeakList(runResult);
			GenerateTheoreticalIsotopicPeakList(runResult);
			outputEvent.Subscribe(OnPublishWithAutoWithoutRelativeMZ);

			labelAmountCalculator.PeaksInCalcMode = Hydra.Core.PeaksInLabelCalculationMode.Automatic;
			labelAmountCalculator.Mode = Mode.ExperimentalMassAndIntensity;
			labelAmountCalculator.Execute(runResult);

			Assert.AreEqual(500.4375, Math.Round(runResult.AverageMass, 5));
			Assert.AreEqual(500.38875, Math.Round(runResult.TheoreticalAverageMass, 5));
			Assert.AreEqual(499.42956, Math.Round(runResult.CentroidMR, 5));
			Assert.AreEqual(499.38081, Math.Round(runResult.TheoreticalCentroidMR, 5));
			Assert.AreEqual(5, runResult.AmideHydrogenTotal);
			Assert.AreEqual(0.04875, Math.Round(runResult.AmountDeut, 5));
			Assert.AreEqual(true, runResult.IsUsedInCalculations);
			Assert.AreEqual(true, publishCalled);
			Assert.AreEqual(8, messageCount);
		}

		[TestMethod]
		public void ExecuteWithManualPeaksInLabelCalculationWithRelativeMz()
		{
			Peptide peptide = CreatePeptide();
			RunResult runResult = new RunResult(peptide, null);
			GenerateIsotopicPeakList(runResult);
			GenerateTheoreticalIsotopicPeakList(runResult);
			runResult.ActualPeaksInCalculation = 5;
			outputEvent.Subscribe(OnPublishWithManual);

			labelAmountCalculator.PeaksInCalcMode = Hydra.Core.PeaksInLabelCalculationMode.Manual;
			labelAmountCalculator.Mode = Mode.CalculatedMassAndExperimentalIntensity;
			labelAmountCalculator.Execute(runResult);

			Assert.AreEqual(500.45342, Math.Round(runResult.AverageMass, 5));
			Assert.AreEqual(500.42480, Math.Round(runResult.TheoreticalAverageMass, 5));
			Assert.AreEqual(499.44548, Math.Round(runResult.CentroidMR, 5));
			Assert.AreEqual(499.41686, Math.Round(runResult.TheoreticalCentroidMR, 5));
			Assert.AreEqual(5, runResult.AmideHydrogenTotal);
			Assert.AreEqual(0.02861, Math.Round(runResult.AmountDeut, 5));
			Assert.AreEqual(true, runResult.IsUsedInCalculations);
			Assert.AreEqual(true, publishCalled);
			Assert.AreEqual(8, messageCount);
		}

		[TestMethod]
		public void ExecuteWithManualPeaksInLabelCalculationWithoutRelativeMz()
		{
			Peptide peptide = CreatePeptide();
			RunResult runResult = new RunResult(peptide, null);
			GenerateIsotopicPeakList(runResult);
			GenerateTheoreticalIsotopicPeakList(runResult);
			runResult.ActualPeaksInCalculation = 5;
			outputEvent.Subscribe(OnPublishWithManualWithoutRelativeMZ);

			labelAmountCalculator.PeaksInCalcMode = Hydra.Core.PeaksInLabelCalculationMode.Manual;
			labelAmountCalculator.Mode = Mode.ExperimentalMassAndIntensity;
			labelAmountCalculator.Execute(runResult);

			Assert.AreEqual(500.45342, Math.Round(runResult.AverageMass, 5));
			Assert.AreEqual(500.42480, Math.Round(runResult.TheoreticalAverageMass, 5));
			Assert.AreEqual(499.44548, Math.Round(runResult.CentroidMR, 5));
			Assert.AreEqual(499.41686, Math.Round(runResult.TheoreticalCentroidMR, 5));
			Assert.AreEqual(5, runResult.AmideHydrogenTotal);
			Assert.AreEqual(0.02861, Math.Round(runResult.AmountDeut, 5));
			Assert.AreEqual(true, runResult.IsUsedInCalculations);
			Assert.AreEqual(true, publishCalled);
			Assert.AreEqual(8, messageCount);
		}

		[TestMethod]
		public void ExecuteWithNoIsotopicPeaks()
		{
			Peptide peptide = CreatePeptide();
			RunResult runResult = new RunResult(peptide, null);
			GenerateTheoreticalIsotopicPeakList(runResult);
			outputEvent.Subscribe(OnPublishWithNoIsotopicPeakList);

			labelAmountCalculator.PeaksInCalcMode = Hydra.Core.PeaksInLabelCalculationMode.Automatic;
			labelAmountCalculator.Mode = Mode.CalculatedMassAndExperimentalIntensity;
			labelAmountCalculator.Execute(runResult);

			Assert.AreEqual(500, Math.Round(runResult.AverageMass, 5));
			Assert.AreEqual(0, Math.Round(runResult.TheoreticalAverageMass, 5));
			Assert.AreEqual(498.99206, Math.Round(runResult.CentroidMR, 5));
			Assert.AreEqual(-1.00794, Math.Round(runResult.TheoreticalCentroidMR, 5));
			Assert.AreEqual(5, runResult.AmideHydrogenTotal);
			Assert.AreEqual(500, Math.Round(runResult.AmountDeut, 5));
			Assert.AreEqual(true, runResult.IsUsedInCalculations);
			Assert.AreEqual(true, publishCalled);
			Assert.AreEqual(8, messageCount);
		}

		[TestMethod]
		public void ExecuteWithNoTheoreticalIsotopicPeaks()
		{
			Peptide peptide = CreatePeptide();
			RunResult runResult = new RunResult(peptide, null);
			GenerateIsotopicPeakList(runResult);
			outputEvent.Subscribe(OnPublishWithNoTheoreticalPeakList);

			labelAmountCalculator.PeaksInCalcMode = Hydra.Core.PeaksInLabelCalculationMode.Automatic;
			labelAmountCalculator.Mode = Mode.CalculatedMassAndExperimentalIntensity;
			labelAmountCalculator.Execute(runResult);

			Assert.AreEqual(500.4375, Math.Round(runResult.AverageMass, 5));
			Assert.AreEqual(0, Math.Round(runResult.TheoreticalAverageMass, 5));
			Assert.AreEqual(499.42956, Math.Round(runResult.CentroidMR, 5));
			Assert.AreEqual(-1.00794, Math.Round(runResult.TheoreticalCentroidMR, 5));
			Assert.AreEqual(5, runResult.AmideHydrogenTotal);
			Assert.AreEqual(500.4375, Math.Round(runResult.AmountDeut, 5));
			Assert.AreEqual(true, runResult.IsUsedInCalculations);
			Assert.AreEqual(true, publishCalled);
			Assert.AreEqual(8, messageCount);
		}

		[TestMethod]
		public void ExecuteWithZeroAverageMass()
		{
			Peptide peptide = CreatePeptide();
			RunResult runResult = new RunResult(peptide, null);
			outputEvent.Subscribe(OnPublishWithNoPeaks);

			labelAmountCalculator.PeaksInCalcMode = Hydra.Core.PeaksInLabelCalculationMode.Automatic;
			labelAmountCalculator.Mode = Mode.CalculatedMassAndExperimentalIntensity;
			labelAmountCalculator.Execute(runResult);

			Assert.AreEqual(500, Math.Round(runResult.AverageMass, 5));
			Assert.AreEqual(0, Math.Round(runResult.TheoreticalAverageMass, 5));
			Assert.AreEqual(498.99206, Math.Round(runResult.CentroidMR, 5));
			Assert.AreEqual(-1.00794, Math.Round(runResult.TheoreticalCentroidMR, 5));
			Assert.AreEqual(5, runResult.AmideHydrogenTotal);
			Assert.AreEqual(500, Math.Round(runResult.AmountDeut, 5));
			Assert.AreEqual(true, runResult.IsUsedInCalculations);
			Assert.AreEqual(true, publishCalled);
			Assert.AreEqual(8, messageCount);
		}

		private void OnPublishWithAuto(string actualValue)
		{
			publishCalled = true;
			messageCount++;
			switch (messageCount - 1)
			{
				case 0:
					Assert.AreEqual("     Label Amount Calculator Started (Mode=CalculatedMassAndExperimentalIntensity, PeaksInCalcMode=Automatic)", actualValue);
					break;
				case 1:
					Assert.AreEqual("     Number of Peaks In Label Calculation: 3 (Automatic)", actualValue);
					break;
				case 2:
					Assert.AreEqual("     Calculated Average Mass=500.4375", actualValue);
					break;
				case 3:
					Assert.AreEqual("     Calculated Theoretical Average Mass=500.388745959496", actualValue);
					break;
				case 4:
					Assert.AreEqual("     Calculated Centroid MR=499.42956", actualValue);
					break;
				case 5:
					Assert.AreEqual("     Calculated Theoretical Centroid MR=499.380805959496", actualValue);
					break;
				case 6:
					Assert.AreEqual("     Calculated Amide Hydrogen Total=5", actualValue);
					break;
				case 7:
					Assert.AreEqual("     Calculated Amount Deuteration=0.0487540405039795 [AvgMass(500.4375) - TheoAvgMass(500.388745959496] * ChargeState(1)", actualValue);
					break;
			}
		}

		private void OnPublishWithAutoWithoutRelativeMZ(string actualValue)
		{
			publishCalled = true;
			messageCount++;
			switch (messageCount - 1)
			{
				case 0:
					Assert.AreEqual("     Label Amount Calculator Started (Mode=ExperimentalMassAndIntensity, PeaksInCalcMode=Automatic)", actualValue);
					break;
				case 1:
					Assert.AreEqual("     Number of Peaks In Label Calculation: 3 (Automatic)", actualValue);
					break;
				case 2:
					Assert.AreEqual("     Calculated Average Mass=500.4375", actualValue);
					break;
				case 3:
					Assert.AreEqual("     Calculated Theoretical Average Mass=500.388745959496", actualValue);
					break;
				case 4:
					Assert.AreEqual("     Calculated Centroid MR=499.42956", actualValue);
					break;
				case 5:
					Assert.AreEqual("     Calculated Theoretical Centroid MR=499.380805959496", actualValue);
					break;
				case 6:
					Assert.AreEqual("     Calculated Amide Hydrogen Total=5", actualValue);
					break;
				case 7:
					Assert.AreEqual("     Calculated Amount Deuteration=0.0487540405039795 [AvgMass(500.4375) - TheoAvgMass(500.388745959496] * ChargeState(1)", actualValue);
					break;
			}
		}

		private void OnPublishWithManual(string actualValue)
		{
			publishCalled = true;
			messageCount++;
			switch (messageCount - 1)
			{
				case 0:
					Assert.AreEqual("     Label Amount Calculator Started (Mode=CalculatedMassAndExperimentalIntensity, PeaksInCalcMode=Manual)", actualValue);
					break;
				case 1:
					Assert.AreEqual("     Number of Peaks In Label Calculation: 5 (Manual)", actualValue);
					break;
				case 2:
					Assert.AreEqual("     Calculated Average Mass=500.453416149068", actualValue);
					break;
				case 3:
					Assert.AreEqual("     Calculated Theoretical Average Mass=500.42480125114", actualValue);
					break;
				case 4:
					Assert.AreEqual("     Calculated Centroid MR=499.445476149068", actualValue);
					break;
				case 5:
					Assert.AreEqual("     Calculated Theoretical Centroid MR=499.41686125114", actualValue);
					break;
				case 6:
					Assert.AreEqual("     Calculated Amide Hydrogen Total=5", actualValue);
					break;
				case 7:
					Assert.AreEqual("     Calculated Amount Deuteration=0.028614897928037 [AvgMass(500.453416149068) - TheoAvgMass(500.42480125114] * ChargeState(1)", actualValue);
					break;
			}
		}

		private void OnPublishWithManualWithoutRelativeMZ(string actualValue)
		{
			publishCalled = true;
			messageCount++;
			switch (messageCount - 1)
			{
				case 0:
					Assert.AreEqual("     Label Amount Calculator Started (Mode=ExperimentalMassAndIntensity, PeaksInCalcMode=Manual)", actualValue);
					break;
				case 1:
					Assert.AreEqual("     Number of Peaks In Label Calculation: 5 (Manual)", actualValue);
					break;
				case 2:
					Assert.AreEqual("     Calculated Average Mass=500.453416149068", actualValue);
					break;
				case 3:
					Assert.AreEqual("     Calculated Theoretical Average Mass=500.42480125114", actualValue);
					break;
				case 4:
					Assert.AreEqual("     Calculated Centroid MR=499.445476149068", actualValue);
					break;
				case 5:
					Assert.AreEqual("     Calculated Theoretical Centroid MR=499.41686125114", actualValue);
					break;
				case 6:
					Assert.AreEqual("     Calculated Amide Hydrogen Total=5", actualValue);
					break;
				case 7:
					Assert.AreEqual("     Calculated Amount Deuteration=0.028614897928037 [AvgMass(500.453416149068) - TheoAvgMass(500.42480125114] * ChargeState(1)", actualValue);
					break;
			}
		}

		private void OnPublishWithNoIsotopicPeakList(string actualValue)
		{
			publishCalled = true;
			messageCount++;
			switch (messageCount - 1)
			{
				case 0:
					Assert.AreEqual("     Label Amount Calculator Started (Mode=CalculatedMassAndExperimentalIntensity, PeaksInCalcMode=Automatic)", actualValue);
					break;
				case 1:
					Assert.AreEqual("     Number of Peaks In Label Calculation: 0 (Automatic)", actualValue);
					break;
				case 2:
					Assert.AreEqual("     Calculated Average Mass=500", actualValue);
					break;
				case 3:
					Assert.AreEqual("     Calculated Theoretical Average Mass=0", actualValue);
					break;
				case 4:
					Assert.AreEqual("     Calculated Centroid MR=498.99206", actualValue);
					break;
				case 5:
					Assert.AreEqual("     Calculated Theoretical Centroid MR=-1.00794", actualValue);
					break;
				case 6:
					Assert.AreEqual("     Calculated Amide Hydrogen Total=5", actualValue);
					break;
				case 7:
					Assert.AreEqual("     Calculated Amount Deuteration=500 [AvgMass(500) - TheoAvgMass(0] * ChargeState(1)", actualValue);
					break;
			}
		}

		private void OnPublishWithNoTheoreticalPeakList(string actualValue)
		{
			publishCalled = true;
			messageCount++;
			switch (messageCount - 1)
			{
				case 0:
					Assert.AreEqual("     Label Amount Calculator Started (Mode=CalculatedMassAndExperimentalIntensity, PeaksInCalcMode=Automatic)", actualValue);
					break;
				case 1:
					Assert.AreEqual("     Number of Peaks In Label Calculation: 3 (Automatic)", actualValue);
					break;
				case 2:
					Assert.AreEqual("     Calculated Average Mass=500.4375", actualValue);
					break;
				case 3:
					Assert.AreEqual("     Calculated Theoretical Average Mass=0", actualValue);
					break;
				case 4:
					Assert.AreEqual("     Calculated Centroid MR=499.42956", actualValue);
					break;
				case 5:
					Assert.AreEqual("     Calculated Theoretical Centroid MR=-1.00794", actualValue);
					break;
				case 6:
					Assert.AreEqual("     Calculated Amide Hydrogen Total=5", actualValue);
					break;
				case 7:
					Assert.AreEqual("     Calculated Amount Deuteration=500.4375 [AvgMass(500.4375) - TheoAvgMass(0] * ChargeState(1)", actualValue);
					break;
			}
		}

		private void OnPublishWithNoPeaks(string actualValue)
		{
			publishCalled = true;
			messageCount++;
			switch (messageCount - 1)
			{
				case 0:
					Assert.AreEqual("     Label Amount Calculator Started (Mode=CalculatedMassAndExperimentalIntensity, PeaksInCalcMode=Automatic)", actualValue);
					break;
				case 1:
					Assert.AreEqual("     Number of Peaks In Label Calculation: 0 (Automatic)", actualValue);
					break;
				case 2:
					Assert.AreEqual("     Calculated Average Mass=500", actualValue);
					break;
				case 3:
					Assert.AreEqual("     Calculated Theoretical Average Mass=0", actualValue);
					break;
				case 4:
					Assert.AreEqual("     Calculated Centroid MR=498.99206", actualValue);
					break;
				case 5:
					Assert.AreEqual("     Calculated Theoretical Centroid MR=-1.00794", actualValue);
					break;
				case 6:
					Assert.AreEqual("     Calculated Amide Hydrogen Total=5", actualValue);
					break;
				case 7:
					Assert.AreEqual("     Calculated Amount Deuteration=500 [AvgMass(500) - TheoAvgMass(0] * ChargeState(1)", actualValue);
					break;
			}
		}

		private void GenerateIsotopicPeakList(RunResult runResult)
		{
			MSPeak mspeak1 = new MSPeak(500, 100, 0.5);
			MSPeak mspeak2 = new MSPeak(501, 50, 0.5);
			MSPeak mspeak3 = new MSPeak(502, 10, 0.5);
			MSPeak mspeak4 = new MSPeak(503, 1, 0.5);

			runResult.IsotopicPeakList.Add(mspeak1);
			runResult.IsotopicPeakList.Add(mspeak2);
			runResult.IsotopicPeakList.Add(mspeak3);
			runResult.IsotopicPeakList.Add(mspeak4);
		}

		private void GenerateTheoreticalIsotopicPeakList(RunResult runResult)
		{
			MSPeak mspeak1 = new MSPeak(500, 100, 0.5);
			MSPeak mspeak2 = new MSPeak(501, 44.25, 0.5);
			MSPeak mspeak3 = new MSPeak(502, 7.34, 0.5);
			MSPeak mspeak4 = new MSPeak(503, 1.22, 0.5);
			MSPeak mspeak5 = new MSPeak(504, 0.65, 0.5);

			runResult.TheoreticalIsotopicPeakList.Add(mspeak1);
			runResult.TheoreticalIsotopicPeakList.Add(mspeak2);
			runResult.TheoreticalIsotopicPeakList.Add(mspeak3);
			runResult.TheoreticalIsotopicPeakList.Add(mspeak4);
			runResult.TheoreticalIsotopicPeakList.Add(mspeak5);
		}

		private Peptide CreatePeptide()
		{
			Peptide peptide = new Peptide("SAMPLER");
			peptide.MonoIsotopicMass = 500;
			peptide.ChargeState = 1;
			return peptide;
		}
	}
}
