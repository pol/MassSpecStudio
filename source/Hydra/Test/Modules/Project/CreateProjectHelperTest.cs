using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Hydra.Core.Domain;
using Hydra.Core.Provider;
using Hydra.DataProvider;
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
	public class CreateProjectHelperTest
	{
		private Mock<IServiceLocator> mockServiceLocator;
		private CreateProjectHelper helper;
		private ProjectBase project;
		private BackgroundWorker worker;
		private IEventAggregator eventAggregator;
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
		public void TestInitialize()
		{
			eventAggregator = new EventAggregator();
			mockServiceLocator = new Mock<IServiceLocator>();
			mockServiceLocator.Setup(mock => mock.GetAllInstances<IExperimentType>()).Returns(new List<IExperimentType>() { new HydraExperimentType(mockServiceLocator.Object) });
			worker = new BackgroundWorker();
			worker.WorkerReportsProgress = true;
			project = new ProjectBase("temp", @"c:\");
			project.ExperimentReferences.Add(new MassSpecStudio.Core.Domain.ProjectBase.ExperimentReference(TestExperimentName, @"c:\temp\" + TestExperimentName + @"\" + TestExperimentName + ".mssexp", new HydraExperimentType(mockServiceLocator.Object).ExperimentType));

			helper = new CreateProjectHelper(worker, project);

			if (Directory.Exists(TestExperimentDirectory))
			{
				Directory.Delete(TestExperimentDirectory, true);
			}
		}

		[TestMethod]
		public void CreateProjectDirectoryStructure()
		{
			Assert.IsFalse(Directory.Exists(TestExperimentDirectory));

			CreateProjectHelper.ConvertExperimentReferencesToExperiments(project, mockServiceLocator.Object);
			helper.CreateProjectDirectoryStructure();

			Assert.IsTrue(Directory.Exists(TestExperimentDirectory));
			Assert.IsTrue(Directory.Exists(Path.Combine(TestHelper.TestExperimentDirectory, "Data")));
			Assert.IsTrue(Directory.Exists(Path.Combine(TestHelper.TestExperimentDirectory, "Results")));
		}

		[TestMethod]
		public void CopyDataFiles()
		{
			CreateProjectHelper.ConvertExperimentReferencesToExperiments(project, mockServiceLocator.Object);
			helper.CreateProjectDirectoryStructure();
			Experiment experiment = project.Experiments.First() as Experiment;

			experiment.Runs.Add(new Run(Properties.Settings.Default.mzXMLTestFile1, Properties.Settings.Default.mzXMLTestFile1, new ProteinState(experiment), new Labeling(experiment), experiment, new ProteoWizardDataProvider(eventAggregator)));
			experiment.Runs.Add(new Run(Properties.Settings.Default.mzXMLTestFile2, Properties.Settings.Default.mzXMLTestFile1, new ProteinState(experiment), new Labeling(experiment), experiment, new ProteoWizardDataProvider(eventAggregator)));

			Assert.AreEqual(0, Directory.GetFiles(Path.Combine(TestExperimentDirectory, "Data")).Length);

			helper.CopyDataFiles();

			Assert.IsTrue(Directory.Exists(Path.Combine(TestExperimentDirectory, "Data")));
			Assert.AreEqual(2, Directory.GetFiles(Path.Combine(TestExperimentDirectory, "Data")).Length);
		}

		[TestMethod]
		public void ReadPeptides()
		{
			ServiceLocator.SetLocatorProvider(new ServiceLocatorProvider(() => mockServiceLocator.Object));
			mockServiceLocator.Setup(mock => mock.GetInstance<IPeptideDataProvider>()).Returns(new CsvPeptideDataProvider());
			CreateProjectHelper.ConvertExperimentReferencesToExperiments(project, mockServiceLocator.Object);
			helper.CreateProjectDirectoryStructure();
			Experiment experiment = project.Experiments.First() as Experiment;

			experiment.PeptideFilePendingImport = Properties.Settings.Default.PeptideTestFile1;

			Assert.AreEqual(0, experiment.Peptides.PeptideCollection.Count);
			helper.ReadPeptides();
			Assert.AreEqual(7, experiment.Peptides.PeptideCollection.Count);
		}

		[TestMethod]
		public void ConvertExperimentReferencesToExperiments()
		{
			Assert.AreEqual(0, project.Experiments.Count);

			CreateProjectHelper.ConvertExperimentReferencesToExperiments(project, mockServiceLocator.Object);

			Assert.AreEqual(1, project.Experiments.Count);
			Assert.IsInstanceOfType(project.Experiments[0].ExperimentTypeObject, typeof(HydraExperimentType));
			Assert.AreEqual(TestExperimentName, project.Experiments[0].Name);
			Assert.AreEqual(Path.Combine(TestExperimentDirectory, TestExperimentName + ".mssexp"), project.Experiments[0].Location);
			Assert.AreEqual(project, project.Experiments[0].Project);
		}
	}
}
