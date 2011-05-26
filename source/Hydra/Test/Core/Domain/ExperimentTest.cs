using System;
using System.Collections.Generic;
using System.IO;
using Hydra.Core.Domain;
using Hydra.Modules.Project;
using MassSpecStudio.Core;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ProteoWizard.MassSpecStudio.DataProvider;

namespace Hydra.Test.Core.Domain
{
	[TestClass]
	public class ExperimentTest
	{
		private Mock<IEventAggregator> mockEventAggregator;
		private Mock<IServiceLocator> mockServiceLocator;
		private Experiment experiment;
		private DateTime dateTime;

		private string TestExperimentDirectory
		{
			get
			{
				if (dateTime.Ticks == 0)
				{
					dateTime = DateTime.Now;
				}
				return Modules.TestHelper.TestExperimentDirectory + dateTime.Ticks;
			}
		}

		private string TestExperimentName
		{
			get
			{
				if (dateTime.Ticks == 0)
				{
					dateTime = DateTime.Now;
				}
				return "testExperiment" + dateTime.Ticks;
			}
		}

		[TestInitialize]
		public void TestInitialize()
		{
			mockEventAggregator = new Mock<IEventAggregator>();
			mockServiceLocator = new Mock<IServiceLocator>();
			mockServiceLocator.Setup(mock => mock.GetAllInstances<IDataProvider>()).Returns(new List<IDataProvider>() { new ProteoWizardDataProvider(mockEventAggregator.Object) });
			experiment = TestHelper.GetTestExperiment(mockServiceLocator);

			try
			{
				if (Directory.Exists(TestExperimentDirectory))
				{
					Directory.Delete(TestExperimentDirectory, true);
				}
			}
			catch
			{
				// Do Nothing
			}
		}

		[TestMethod]
		public void GetProperties()
		{
			experiment.PeptideFilePendingImport = "test1";
			Assert.AreEqual("test1", experiment.PeptideFilePendingImport);
			experiment.PeptidesFileLocation = "test2";
			Assert.AreEqual("test2", experiment.PeptidesFileLocation);
			experiment.IsProcessed = true;
			Assert.AreEqual(true, experiment.IsProcessed);
		}

		[TestMethod]
		public void Open()
		{
			OpenExperiment();

			Assert.AreEqual("testProject1", experiment.Project.Name);
			Assert.AreEqual(5, experiment.Peptides.PeptideCollection.Count);
			Assert.AreEqual(1, experiment.Results.Count);
			Assert.AreEqual(new ProteoWizardDataProvider(mockEventAggregator.Object).TypeId, experiment.DataProviderType);
			Assert.IsInstanceOfType(experiment.DataProvider, typeof(ProteoWizardDataProvider));
		}

		[TestMethod]
		public void GetRunsByProteinState()
		{
			OpenExperiment();

			IList<Run> runs = experiment.GetRunsByProteinState(experiment.ProteinStates[1]);
			Assert.AreEqual(2, runs.Count);
			Assert.AreEqual(@"Data\20100623 Run 5-s1.mzXML", runs[0].FileName);
			Assert.AreEqual(@"Data\20100623 Run 7-s1.mzXML", runs[1].FileName);
		}

		[TestMethod]
		public void GetRunResult()
		{
			OpenExperiment();

			RunResult runResult = experiment.GetRunResult(experiment.Results[0], experiment.Runs[0], experiment.Peptides.PeptideCollection[0]);

			Assert.AreEqual(@"Data\20100623 Run 1-s1.mzXML", runResult.Run.FileName);
			Assert.AreEqual("DGIPSKVQRCAVG", runResult.Peptide.Sequence);
			Assert.AreEqual(4.57321, Math.Round(runResult.AmountDeut, 5));
		}

		[TestMethod]
		public void Save()
		{
			OpenExperiment();
			Assert.AreEqual(2, experiment.ProteinStates.Count);

			ProteinState proteinState = new ProteinState(experiment);
			experiment.Save();

			OpenExperiment();
			Assert.AreEqual(3, experiment.ProteinStates.Count);
			experiment.ProteinStates.Remove(experiment.ProteinStates[2]);
			experiment.Save();
		}

		[TestMethod]
		public void CreateDirectoryStructure()
		{
			Assert.IsFalse(Directory.Exists(TestExperimentDirectory));

			Experiment experiment = new Experiment(TestExperimentName, new MassSpecStudio.Core.Domain.ProjectBase("temp", @"C:\"), new HydraExperimentType(mockServiceLocator.Object));
			experiment.CreateDirectoryStructure();

			Assert.IsTrue(Directory.Exists(TestExperimentDirectory));
			Assert.IsTrue(Directory.Exists(Path.Combine(TestExperimentDirectory, "Data")));
			Assert.IsTrue(Directory.Exists(Path.Combine(TestExperimentDirectory, "Results")));
		}

		private void OpenExperiment()
		{
			experiment = Experiment.Open(Properties.Settings.Default.TestExperiment1, new MassSpecStudio.Core.Domain.ProjectBase("testProject1", Properties.Settings.Default.ProjectRootDirectory), mockServiceLocator.Object);
		}
	}
}
