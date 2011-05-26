using System.Collections.Generic;
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
	public class IsotopicProfileFinderTest
	{
		private IsotopicProfileFinder isotopicProfileFinder;
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
			isotopicProfileFinder = new IsotopicProfileFinder(mockEventAggregator.Object);
			isotopicProfileFinder.PeakWidthMaximum = 0.5;
			isotopicProfileFinder.MSPeakSelectionOption = Hydra.Core.MSPeakSelectionOption.MostIntenseWithinMzVariation;
		}

		[TestMethod]
		public void ProcessingParameterValues()
		{
			isotopicProfileFinder.MassVariability = 1.5;
			Assert.AreEqual(1.5, isotopicProfileFinder.MassVariability);
			isotopicProfileFinder.PeakWidthMinimum = 2.5;
			Assert.AreEqual(2.5, isotopicProfileFinder.PeakWidthMinimum);
			isotopicProfileFinder.PeakWidthMaximum = 3.5;
			Assert.AreEqual(3.5, isotopicProfileFinder.PeakWidthMaximum);
			isotopicProfileFinder.IntensityThreshold = 4.5;
			Assert.AreEqual(4.5, isotopicProfileFinder.IntensityThreshold);
			isotopicProfileFinder.PeakNumberMaximum = 5;
			Assert.AreEqual(5, isotopicProfileFinder.PeakNumberMaximum);
			isotopicProfileFinder.MSPeakSelectionOption = Hydra.Core.MSPeakSelectionOption.MostIntenseWithinMzVariation;
			Assert.AreEqual(Hydra.Core.MSPeakSelectionOption.MostIntenseWithinMzVariation, isotopicProfileFinder.MSPeakSelectionOption);
		}

		[TestMethod]
		public void ExecuteResultsBasedOnWholePeptide()
		{
			Peptide peptide = CreatePeptide();
			RunResult runResult = new RunResult(peptide, null);
			outputEvent.Subscribe(OnPublish);

			isotopicProfileFinder.PeakNumberMaximum = 5;
			isotopicProfileFinder.MassVariability = 1.5;
			isotopicProfileFinder.Execute(runResult, GenerateTestPeakList());

			Assert.AreEqual(5, runResult.IsotopicPeakList.Count);
			Assert.AreEqual(20, runResult.TheoreticalIsotopicPeakList.Count);
			Assert.AreEqual(true, publishCalled);
			Assert.AreEqual(31, messageCount);
		}

		[TestMethod]
		public void ExecuteResultsBasedOnFragment()
		{
			Peptide peptide = CreatePeptide();
			RunResult runResult = new RunResult(new FragmentIon("SAMPLER", peptide), null);
			runResult.FragmentIon.FragmentIonType = Hydra.Core.FragmentIonType.BFragment;
			outputEvent.Subscribe(OnPublishWithFragments);

			isotopicProfileFinder.PeakNumberMaximum = 5;
			isotopicProfileFinder.MassVariability = 1.5;
			isotopicProfileFinder.Execute(runResult, GenerateTestPeakList());

			Assert.AreEqual(5, runResult.IsotopicPeakList.Count);
			Assert.AreEqual(20, runResult.TheoreticalIsotopicPeakList.Count);
			Assert.AreEqual(true, publishCalled);
			Assert.AreEqual(31, messageCount);
		}

		[TestMethod]
		public void ExecuteWhenNoTargetMassesFound()
		{
			Peptide peptide = CreatePeptide();
			RunResult runResult = new RunResult(peptide, null);
			outputEvent.Subscribe(OnPublishWithNoTargetMasses);

			isotopicProfileFinder.PeakNumberMaximum = 0;
			isotopicProfileFinder.MassVariability = 1.5;
			isotopicProfileFinder.Execute(runResult, GenerateTestPeakList());

			Assert.AreEqual(0, runResult.IsotopicPeakList.Count);
			Assert.AreEqual(20, runResult.TheoreticalIsotopicPeakList.Count);
			Assert.AreEqual(true, publishCalled);
			Assert.AreEqual(26, messageCount);
		}

		[TestMethod]
		public void ExecuteWhenMonoIsopticPeakNotFound()
		{
			Peptide peptide = CreatePeptide();
			peptide.MonoIsotopicMass = 200;
			RunResult runResult = new RunResult(peptide, null);
			outputEvent.Subscribe(OnPublishWithNoMonoisotopicPeak);

			isotopicProfileFinder.PeakNumberMaximum = 5;
			isotopicProfileFinder.MassVariability = 1.5;
			isotopicProfileFinder.Execute(runResult, GenerateTestPeakList());

			Assert.AreEqual(0, runResult.IsotopicPeakList.Count);
			Assert.AreEqual(20, runResult.TheoreticalIsotopicPeakList.Count);
			Assert.AreEqual(true, publishCalled);
			Assert.AreEqual(26, messageCount);
		}

		[TestMethod]
		public void ExecuteWhenNoPeaks()
		{
			Peptide peptide = CreatePeptide();
			RunResult runResult = new RunResult(peptide, null);
			outputEvent.Subscribe(OnPublishWithNoPeaks);

			isotopicProfileFinder.PeakNumberMaximum = 5;
			isotopicProfileFinder.MassVariability = 1.5;
			isotopicProfileFinder.Execute(runResult, new List<MSPeak>());

			Assert.AreEqual(0, runResult.IsotopicPeakList.Count);
			Assert.AreEqual(20, runResult.TheoreticalIsotopicPeakList.Count);
			Assert.AreEqual(true, publishCalled);
			Assert.AreEqual(26, messageCount);
		}

		private void OnPublish(string actualValue)
		{
			publishCalled = true;
			messageCount++;
			switch (messageCount - 1)
			{
				case 0:
					Assert.AreEqual("     Isotopic Profile Finder Started (MassRange=1.5, PeakWidthMinimum=0, PeakWidthMaximum=0.5, IntensityThreshold=0, PeakNumberMaximum=5, MSPeakSelectionOption=MostIntenseWithinMzVariation)", actualValue);
					break;
				case 1:
					Assert.AreEqual("     Targeted isotopic peak masses: 500, 501, 502, 503, 504, ", actualValue);
					break;
				case 2:
					Assert.AreEqual("     Monoisotopic Peak Found (MZ=500, Intensity=100, 0.5)", actualValue);
					break;
				case 3:
					Assert.AreEqual("     Identifying Isotopic Peak List:", actualValue);
					break;
				case 4:
					Assert.AreEqual("     Peak Found: (MZ=501, Intensity=50, PeakWidth=0.5)", actualValue);
					break;
				case 5:
					Assert.AreEqual("     Peak Found: (MZ=502, Intensity=10, PeakWidth=0.5)", actualValue);
					break;
				case 6:
					Assert.AreEqual("     Peak Found: (MZ=503, Intensity=1, PeakWidth=0.5)", actualValue);
					break;
				case 7:
					Assert.AreEqual("     Peak Found: (MZ=503, Intensity=1, PeakWidth=0.5)", actualValue);
					break;
				case 8:
					Assert.AreEqual("     4 peaks found.", actualValue);
					break;
				case 9:
					Assert.AreEqual("     Calculating Theoretical Isotopic Peak List (based on peptide sequence)", actualValue);
					break;
				case 10:
					Assert.AreEqual("     Calculated Peak (MZ=803.40809, Intensity=100, PeakWidth=0)", actualValue);
					break;
				case 11:
					Assert.AreEqual("     Calculated Peak (MZ=804.40809, Intensity=41.4752360235359, PeakWidth=0)", actualValue);
					break;
				case 30:
					Assert.AreEqual("     20 peaks found.", actualValue);
					break;
			}
		}

		private void OnPublishWithFragments(string actualValue)
		{
			publishCalled = true;
			messageCount++;
			switch (messageCount - 1)
			{
				case 0:
					Assert.AreEqual("     Isotopic Profile Finder Started (MassRange=1.5, PeakWidthMinimum=0, PeakWidthMaximum=0.5, IntensityThreshold=0, PeakNumberMaximum=5, MSPeakSelectionOption=MostIntenseWithinMzVariation)", actualValue);
					break;
				case 1:
					Assert.AreEqual("     Targeted isotopic peak masses: 500, 501, 502, 503, 504, ", actualValue);
					break;
				case 2:
					Assert.AreEqual("     Monoisotopic Peak Found (MZ=500, Intensity=100, 0.5)", actualValue);
					break;
				case 3:
					Assert.AreEqual("     Identifying Isotopic Peak List:", actualValue);
					break;
				case 4:
					Assert.AreEqual("     Peak Found: (MZ=501, Intensity=50, PeakWidth=0.5)", actualValue);
					break;
				case 5:
					Assert.AreEqual("     Peak Found: (MZ=502, Intensity=10, PeakWidth=0.5)", actualValue);
					break;
				case 6:
					Assert.AreEqual("     Peak Found: (MZ=503, Intensity=1, PeakWidth=0.5)", actualValue);
					break;
				case 7:
					Assert.AreEqual("     Peak Found: (MZ=503, Intensity=1, PeakWidth=0.5)", actualValue);
					break;
				case 8:
					Assert.AreEqual("     4 peaks found.", actualValue);
					break;
				case 9:
					Assert.AreEqual("     Calculating Theoretical Isotopic Peak List (based on fragment sequences)", actualValue);
					break;
				case 10:
					Assert.AreEqual("     Calculated Peak (MZ=785.39796, Intensity=100, PeakWidth=0)", actualValue);
					break;
				case 11:
					Assert.AreEqual("     Calculated Peak (MZ=786.39796, Intensity=41.4221424648485, PeakWidth=0)", actualValue);
					break;
				case 30:
					Assert.AreEqual("     20 peaks found.", actualValue);
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
					Assert.AreEqual("     Isotopic Profile Finder Started (MassRange=1.5, PeakWidthMinimum=0, PeakWidthMaximum=0.5, IntensityThreshold=0, PeakNumberMaximum=5, MSPeakSelectionOption=MostIntenseWithinMzVariation)", actualValue);
					break;
				case 1:
					Assert.AreEqual("     Targeted isotopic peak masses: 500, 501, 502, 503, 504, ", actualValue);
					break;
				case 2:
					Assert.AreEqual("     Identifying Isotopic Peak List:", actualValue);
					break;
				case 3:
					Assert.AreEqual("     0 peaks found.", actualValue);
					break;
				case 4:
					Assert.AreEqual("     Calculating Theoretical Isotopic Peak List (based on peptide sequence)", actualValue);
					break;
				case 5:
					Assert.AreEqual("     Calculated Peak (MZ=803.40809, Intensity=100, PeakWidth=0)", actualValue);
					break;
				case 6:
					Assert.AreEqual("     Calculated Peak (MZ=804.40809, Intensity=41.4752360235359, PeakWidth=0)", actualValue);
					break;
				case 25:
					Assert.AreEqual("     20 peaks found.", actualValue);
					break;
			}
		}

		private void OnPublishWithNoMonoisotopicPeak(string actualValue)
		{
			publishCalled = true;
			messageCount++;
			switch (messageCount - 1)
			{
				case 0:
					Assert.AreEqual("     Isotopic Profile Finder Started (MassRange=1.5, PeakWidthMinimum=0, PeakWidthMaximum=0.5, IntensityThreshold=0, PeakNumberMaximum=5, MSPeakSelectionOption=MostIntenseWithinMzVariation)", actualValue);
					break;
				case 1:
					Assert.AreEqual("     Targeted isotopic peak masses: 200, 201, 202, 203, 204, ", actualValue);
					break;
				case 2:
					Assert.AreEqual("     Identifying Isotopic Peak List:", actualValue);
					break;
				case 3:
					Assert.AreEqual("     0 peaks found.", actualValue);
					break;
				case 4:
					Assert.AreEqual("     Calculating Theoretical Isotopic Peak List (based on peptide sequence)", actualValue);
					break;
				case 5:
					Assert.AreEqual("     Calculated Peak (MZ=803.40809, Intensity=100, PeakWidth=0)", actualValue);
					break;
				case 6:
					Assert.AreEqual("     Calculated Peak (MZ=804.40809, Intensity=41.4752360235359, PeakWidth=0)", actualValue);
					break;
				case 25:
					Assert.AreEqual("     20 peaks found.", actualValue);
					break;
			}
		}

		private void OnPublishWithNoTargetMasses(string actualValue)
		{
			publishCalled = true;
			messageCount++;
			switch (messageCount - 1)
			{
				case 0:
					Assert.AreEqual("     Isotopic Profile Finder Started (MassRange=1.5, PeakWidthMinimum=0, PeakWidthMaximum=0.5, IntensityThreshold=0, PeakNumberMaximum=0, MSPeakSelectionOption=MostIntenseWithinMzVariation)", actualValue);
					break;
				case 1:
					Assert.AreEqual("     Targeted isotopic peak masses: ", actualValue);
					break;
				case 2:
					Assert.AreEqual("     Identifying Isotopic Peak List:", actualValue);
					break;
				case 3:
					Assert.AreEqual("     0 peaks found.", actualValue);
					break;
				case 4:
					Assert.AreEqual("     Calculating Theoretical Isotopic Peak List (based on peptide sequence)", actualValue);
					break;
				case 5:
					Assert.AreEqual("     Calculated Peak (MZ=803.40809, Intensity=100, PeakWidth=0)", actualValue);
					break;
				case 6:
					Assert.AreEqual("     Calculated Peak (MZ=804.40809, Intensity=41.4752360235359, PeakWidth=0)", actualValue);
					break;
				case 25:
					Assert.AreEqual("     20 peaks found.", actualValue);
					break;
			}
		}

		private IList<MSPeak> GenerateTestPeakList()
		{
			IList<MSPeak> msPeaklist = new List<MSPeak>();
			MSPeak mspeak1 = new MSPeak(500, 100, 0.5);
			MSPeak mspeak2 = new MSPeak(501, 50, 0.5);
			MSPeak mspeak3 = new MSPeak(502, 10, 0.5);
			MSPeak mspeak4 = new MSPeak(503, 1, 0.5);

			msPeaklist.Add(mspeak1);
			msPeaklist.Add(mspeak2);
			msPeaklist.Add(mspeak3);
			msPeaklist.Add(mspeak4);

			return msPeaklist;
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
