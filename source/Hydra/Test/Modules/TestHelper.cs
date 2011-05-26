using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Hydra.Core;
using Hydra.Core.Domain;
using Hydra.Modules.Project;
using MassSpecStudio.Core;
using MassSpecStudio.Core.Domain;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Moq;
using ProteoWizard.MassSpecStudio.DataProvider;

namespace Hydra.Test.Modules
{
	public static class TestHelper
	{
		public const string TestExperimentDirectory = @"c:\temp\testExperiment";

		public static ProjectBase CreateTestProject()
		{
			DeleteProjectDirectory();

			Mock<IEventAggregator> mockEventAggregator = new Mock<IEventAggregator>();
			Mock<IServiceLocator> mockServiceLocator = new Mock<IServiceLocator>();
			mockServiceLocator.Setup(mock => mock.GetAllInstances<IDataProvider>()).Returns(new List<IDataProvider>() { new ProteoWizardDataProvider(mockEventAggregator.Object) });
			ProjectBase project = new ProjectBase("temp", @"c:\");
			HydraExperimentType hydraExperimentType = new HydraExperimentType(mockServiceLocator.Object);
			Experiment experiment = hydraExperimentType.CreateExperiment(project, "testExperiment") as Experiment;
			ProteinState proteinState1 = new ProteinState(experiment);
			ProteinState proteinState2 = new ProteinState(experiment);
			Labeling labeling1 = new Labeling(experiment);
			Labeling labeling2 = new Labeling(experiment);

			CreateRun(Properties.Settings.Default.mzXMLTestFile1, proteinState1, labeling1, experiment, mockEventAggregator);
			CreateRun(Properties.Settings.Default.mzXMLTestFile2, proteinState1, labeling1, experiment, mockEventAggregator);
			CreateRun(Properties.Settings.Default.mzXMLTestFile2, proteinState1, labeling2, experiment, mockEventAggregator);
			CreateRun(Properties.Settings.Default.mzXMLTestFile2, proteinState2, labeling1, experiment, mockEventAggregator);

			experiment.Peptides.PeptideCollection.Add(CreatePeptide("SAMPLE", 1));
			experiment.Peptides.PeptideCollection.Add(CreatePeptide("SAMPLES", 20));

			experiment.Save();
			project.Experiments.Add(experiment);
			return project;
		}

		public static Peptide CreatePeptide(string sequence, int seed)
		{
			Peptide peptide = new Peptide(sequence);
			peptide.AminoAcidStart = seed++;
			peptide.AminoAcidStop = seed++;
			peptide.MonoIsotopicMass = seed++;
			peptide.ChargeState = seed++;
			peptide.Notes = (seed++).ToString();
			peptide.Period = seed++;
			peptide.XicPeakPickerOption = XicPeakPickerOption.MostIntenseWithinEntireXic;
			peptide.RT = seed++;
			peptide.RtVariance = seed++;
			peptide.XicAdjustment = seed++;
			peptide.XicSelectionWidth = seed++;
			peptide.PeaksInCalculation = seed++;
			peptide.DeuteriumDistributionThreshold = seed++;
			peptide.DeuteriumDistributionRightPadding = seed++;
			return peptide;
		}

		public static void MockRegionManagerDisplay(string regionName, Type viewType, Mock<IServiceLocator> mockServiceLocator, Mock<IRegion> mockRegion, Mock<IRegionManager> mockRegionManager)
		{
			mockServiceLocator.Setup(mock => mock.GetInstance(viewType)).Returns(null);
			mockRegion.Setup(mock => mock.Views).Returns(new ViewsCollection(new ObservableCollection<ItemMetadata>(), null));
			mockRegion.Setup(mock => mock.Activate(null));
			mockRegionManager.Setup(mock => mock.Regions.ContainsRegionWithName(regionName)).Returns(true);
			mockRegionManager.Setup(mock => mock.Regions[regionName]).Returns(mockRegion.Object);
		}

		private static Hydra.Core.Domain.Run CreateRun(string filename, ProteinState proteinState, Labeling labeling, Experiment experiment, Mock<IEventAggregator> mockEventAggregator)
		{
			string runFileName = Path.GetFileName(filename);
			File.Copy(filename, Path.Combine(TestExperimentDirectory, "Data", runFileName), true);
			Hydra.Core.Domain.Run run = new Hydra.Core.Domain.Run(
				@"Data\" + runFileName,
				Path.Combine(TestExperimentDirectory, "Data", runFileName),
				proteinState,
				labeling,
				experiment,
				new ProteoWizardDataProvider(mockEventAggregator.Object));
			return run;
		}

		private static void DeleteProjectDirectory()
		{
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
	}
}
