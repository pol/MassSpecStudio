using System;
using System.Collections.Generic;
using System.IO;
using Hydra.Core.Domain;
using Hydra.Modules.Project;
using MassSpecStudio.Core;
using MassSpecStudio.Core.Domain;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ProteoWizard.MassSpecStudio.DataProvider;

namespace Hydra.Test.Modules.Project
{
	[TestClass]
	public class HydraExperimentTypeTest
	{
		private HydraExperimentType hydraExperimentType;
		private Mock<IServiceLocator> mockServiceLocator;
		private Mock<IEventAggregator> mockEventAggregator;
		private ProjectBase project;
		private DateTime dateTime;

		private string TestExperimentDirectory
		{
			get
			{
				if (dateTime.Ticks == 0)
				{
					dateTime = DateTime.Now;
				}
				return TestHelper.TestExperimentDirectory + dateTime.Ticks;
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
		public void ClassInitialize()
		{
			mockServiceLocator = new Mock<IServiceLocator>();
			mockEventAggregator = new Mock<IEventAggregator>();
			hydraExperimentType = new HydraExperimentType(mockServiceLocator.Object);
			project = new ProjectBase("temp", @"C:\");

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
		public void Parameters()
		{
			Assert.AreEqual("/MassSpecStudio;component/Images/add-item.png", hydraExperimentType.Icon);
			Assert.AreEqual("Hydra", hydraExperimentType.Name);
			Assert.AreEqual("Used to investigate HDX data.", hydraExperimentType.Description);
			Assert.AreEqual(new Guid("DAEC1373-5354-4622-BDCD-8BAB3BD5788B"), hydraExperimentType.ExperimentType);
			Assert.AreEqual(hydraExperimentType, hydraExperimentType.ExperimentTypeObject);
		}

		[TestMethod]
		public void CreateExperiment()
		{
			Assert.IsFalse(Directory.Exists(TestExperimentDirectory));

			ExperimentBase experiment = hydraExperimentType.CreateExperiment(project, TestExperimentName);

			Assert.IsTrue(Directory.Exists(TestExperimentDirectory));
			Assert.IsTrue(Directory.Exists(Path.Combine(TestExperimentDirectory, "Data")));
			Assert.IsTrue(Directory.Exists(Path.Combine(TestExperimentDirectory, "Results")));

			Assert.IsTrue(File.Exists(Path.Combine(TestExperimentDirectory, TestExperimentName + ".mssexp")));

			Assert.AreEqual(Path.Combine(TestExperimentDirectory, TestExperimentName + ".mssexp").ToLower(), experiment.Location.ToLower());
			Assert.AreEqual(TestExperimentName, experiment.Name);
			Assert.AreEqual(project, experiment.Project);
			Assert.AreEqual(hydraExperimentType, experiment.ExperimentTypeObject);
			Assert.AreEqual(hydraExperimentType.ExperimentType, experiment.ExperimentType);
		}

		[TestMethod]
		public void SaveAndOpen()
		{
			List<IDataProvider> dataProviders = new List<IDataProvider>();
			dataProviders.Add(new ProteoWizardDataProvider(mockEventAggregator.Object));
			mockServiceLocator.Setup(mock => mock.GetAllInstances<IDataProvider>()).Returns(dataProviders);
			Experiment experiment = CreateTestExperiment();
			experiment.DataProvider = new ProteoWizardDataProvider(mockEventAggregator.Object);
			experiment.Save();

			ExperimentBase loadedExperiment = hydraExperimentType.Open(Path.Combine(TestExperimentDirectory, TestExperimentName + ".mssexp"), project);

			Assert.AreEqual(1, ((Experiment)loadedExperiment).ProteinStates.Count);
			mockServiceLocator.VerifyAll();
		}

		[TestMethod]
		[ExpectedException(typeof(Exception))]
		public void OpenWithMissingDataProvider()
		{
			List<IDataProvider> dataProviders = new List<IDataProvider>();
			mockServiceLocator.Setup(mock => mock.GetAllInstances<IDataProvider>()).Returns(dataProviders);
			Experiment experiment = CreateTestExperiment();

			hydraExperimentType.Open(Path.Combine(TestExperimentDirectory, TestExperimentName + ".mssexp"), project);
		}

		private Experiment CreateTestExperiment()
		{
			Experiment experiment = hydraExperimentType.CreateExperiment(project, TestExperimentName) as Experiment;
			ProteinState proteinState = new ProteinState(experiment);
			Labeling labeling = new Labeling(experiment);

			string runFileName = Path.GetFileName(Properties.Settings.Default.mzXMLTestFile1);
			File.Copy(Properties.Settings.Default.mzXMLTestFile1, Path.Combine(TestExperimentDirectory, "Data", runFileName), true);
			Run run = new Run(
				@"Data\" + runFileName,
				Path.Combine(TestExperimentDirectory, "Data", runFileName),
				proteinState,
				labeling,
				experiment,
				new ProteoWizardDataProvider(mockEventAggregator.Object));
			experiment.Save();
			return experiment as Experiment;
		}
	}
}
