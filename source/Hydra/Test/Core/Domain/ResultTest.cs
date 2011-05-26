using System;
using System.Collections.Generic;
using System.IO;
using Hydra.Core.Domain;
using Hydra.Modules.Project;
using MassSpecStudio.Core.Domain;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Hydra.Test.Core.Domain
{
	[TestClass]
	public class ResultTest
	{
		private Mock<IServiceLocator> mockServiceLocator;
		private Experiment experiment;

		[TestInitialize]
		public void TestInitialize()
		{
			mockServiceLocator = new Mock<IServiceLocator>();
			experiment = new Experiment("testExperiment1", new ProjectBase("testProject1", @"..\..\..\..\testData\testProjects\"), new HydraExperimentType(mockServiceLocator.Object));
			DeleteTestResultFile();
		}

		[TestCleanup]
		public void TestCleanup()
		{
			DeleteTestResultFile();
		}

		[TestMethod]
		public void BlankConstructor()
		{
			Result result = new Result();
			Assert.AreEqual(0, result.DeuterationResults.Count);
			Assert.AreEqual(0, result.RunResults.Count);
		}

		[TestMethod]
		public void Constructor()
		{
			Result result = new Result("result", experiment);

			Assert.AreEqual("result", result.Name);
			Assert.IsTrue(result.Location.StartsWith("Results - "));
		}

		[TestMethod]
		public void GetProperties()
		{
			Result result = new Result();

			result.Name = "test1";
			Assert.AreEqual("test1", result.Name);
			result.Location = "test2";
			Assert.AreEqual("test2", result.Location);
		}

		[TestMethod]
		public void Open()
		{
			Result result = Result.Open(experiment, Properties.Settings.Default.ResultFile1);

			Assert.AreEqual("Results - 2011-02-10 05.46.05.xml", result.Location);
			Assert.AreEqual("Result (2/10/2011 05:46:05 PM)", result.Name);
			Assert.AreEqual(10, result.DeuterationResults.Count);
			AssertResult(result.DeuterationResults[0], 20, 1, "DGIPSKVQRCAVG", 3.98177);
			AssertResult(result.DeuterationResults[1], 20, 1, "DGIPSKVQRCAVG", 2.66857);
			AssertResult(result.DeuterationResults[2], 20, 1, "NENERYDAVQHCRYVDE", 0.46591);
			AssertResult(result.DeuterationResults[3], 20, 1, "NENERYDAVQHCRYVDE", 0.49935);
			AssertResult(result.DeuterationResults[4], 20, 1, "VAHDDIPYSSAGSDDVYKHIKEAGM", 3.68599);
			AssertResult(result.DeuterationResults[5], 20, 1, "VAHDDIPYSSAGSDDVYKHIKEAGM", 3.71192);
			AssertResult(result.DeuterationResults[6], 20, 1, "RIVRDYDVYA", 1.2074);
			AssertResult(result.DeuterationResults[7], 20, 1, "RIVRDYDVYA", 0.77797);
			AssertResult(result.DeuterationResults[8], 20, 1, "LNVSF", 1.09356);
			AssertResult(result.DeuterationResults[9], 20, 1, "LNVSF", double.NaN);
		}

		[TestMethod]
		public void OpenDirectory()
		{
			IList<Result> results = Result.OpenDirectory(experiment);

			Assert.AreEqual(1, results.Count);
			Assert.AreEqual("Result (2/10/2011 05:46:05 PM)", results[0].Name);
		}

		[TestMethod]
		public void Save()
		{
			Result result = Result.Open(experiment, Properties.Settings.Default.ResultFile1);

			result.Name = "test";
			result.Location = "testResult.xml";
			result.Save();

			Result newResult = Result.Open(experiment, "testResult.xml");
			Assert.AreEqual("test", newResult.Name);
		}

		private void AssertResult(DeuterationResult result, double labelingPercent, double labelingTime, string sequence, double amountDeut)
		{
			Assert.AreEqual(labelingPercent, result.Labeling.LabelingPercent);
			Assert.AreEqual(labelingTime, result.Labeling.LabelingTime);
			Assert.AreEqual(sequence, result.Peptide.Sequence);
			Assert.AreEqual(0, result.ReplicateResults.Count);
			Assert.AreEqual(amountDeut, Math.Round(result.AmountDeut, 5));
		}

		private void DeleteTestResultFile()
		{
			string fullTestFilePath = Path.Combine(@"..\..\..\..\testData\testProjects\testProject1\testExperiment1\Results", "testResult.xml");
			if (File.Exists(fullTestFilePath))
			{
				File.Delete(fullTestFilePath);
			}
		}
	}
}
