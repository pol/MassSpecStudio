using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MassSpecStudio.Core;
using MassSpecStudio.Core.Domain;
using MassSpecStudio.Core.Events;
using MassSpecStudio.Modules.Project.Views;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MassSpecStudio.Test.Modules.Project.Views
{
	[TestClass]
	public class NewProjectViewModelTest
	{
		private IEventAggregator eventAggregator;
		private Mock<IServiceLocator> mockServiceLocator;
		private Mock<IExperimentType> mockExperimentType;
		private Mock<IDocumentManager> mockDocumentManager;
		private NewProjectViewModel viewModel;
		private bool createProjectFired;

		[TestInitialize]
		public void TestInitialize()
		{
			try
			{
				if (Directory.Exists(@"c:\temp\testProject"))
				{
					Directory.Delete(@"c:\temp\testProject", true);
				}
			}
			catch (Exception)
			{
				// Do Nothing
			}

			mockServiceLocator = new Mock<IServiceLocator>();
			mockExperimentType = new Mock<IExperimentType>();
			mockDocumentManager = new Mock<IDocumentManager>();
			mockExperimentType.Setup(mock => mock.Name).Returns("test");
			mockServiceLocator.Setup(mock => mock.GetAllInstances<IExperimentType>()).Returns(new List<IExperimentType>() { mockExperimentType.Object });
			mockServiceLocator.Setup(mock => mock.GetInstance<IDocumentManager>()).Returns(mockDocumentManager.Object);
			eventAggregator = new EventAggregator();
			viewModel = new NewProjectViewModel(eventAggregator, mockServiceLocator.Object);
		}

		[TestMethod]
		public void Constructor()
		{
			Assert.AreEqual(1, viewModel.ExperimentTypes.Count());
			Assert.AreEqual(viewModel.ExperimentTypes.First(), viewModel.SelectedExperimentType);
			Assert.AreEqual(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Mass Spec Studio Projects"), viewModel.Location);
			Assert.AreEqual(true, viewModel.ProjectName.StartsWith("testProject"));
			Assert.AreEqual("testExperiment", viewModel.ExperimentName);
		}

		[TestMethod]
		public void ExperimentName()
		{
			SetGoodValues();
			viewModel.ExperimentName = "test";
			Assert.AreEqual(0, Validation.Validate(viewModel).Count());
		}

		[TestMethod]
		public void ExperimentNameTooShort()
		{
			SetGoodValues();
			viewModel.ExperimentName = string.Empty;
			AssertValidation(2, "Experiment name must be less than 100 characters.");
		}

		[TestMethod]
		public void ExperimentNameTooLong()
		{
			SetGoodValues();
			viewModel.ExperimentName = "12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901";
			AssertValidation(1, "Experiment name must be less than 100 characters.");
		}

		[TestMethod]
		public void ExperimentNameInvalidCharacters()
		{
			SetGoodValues();
			viewModel.ExperimentName = @"asdf\";
			AssertValidation(1, "Directory name contains invalid characters.");
		}

		[TestMethod]
		public void ExperimentNameAlreadyExists()
		{
			Directory.CreateDirectory(@"C:\temp\testProject\TestExperiment");
			SetGoodValues();
			AssertValidation(1, "A project with this name already exists.");
		}

		[TestMethod]
		public void ProjectName()
		{
			SetGoodValues();
			viewModel.ProjectName = "test";
			Assert.AreEqual(0, Validation.Validate(viewModel).Count());
		}

		[TestMethod]
		public void ProjectNameTooShort()
		{
			SetGoodValues();
			viewModel.ProjectName = string.Empty;
			AssertValidation(3, "Project name must be less than 100 characters.");
		}

		[TestMethod]
		public void ProjectNameTooLong()
		{
			SetGoodValues();
			viewModel.ProjectName = "12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901";
			AssertValidation(1, "Project name must be less than 100 characters.");
		}

		[TestMethod]
		public void ProjectNameInvalidCharacters()
		{
			SetGoodValues();
			viewModel.ProjectName = @"asdf\";
			AssertValidation(1, "Directory name contains invalid characters.");
		}

		[TestMethod]
		public void ProjectNameAlreadyExists()
		{
			Directory.CreateDirectory(@"C:\temp\testProject");
			SetGoodValues();
			AssertValidation(1, "A project with this name already exists.");
		}

		[TestMethod]
		public void Location()
		{
			SetGoodValues();
			viewModel.Location = @"c:\temp\";
			Assert.AreEqual(0, Validation.Validate(viewModel).Count());
		}

		[TestMethod]
		public void LocationRequired()
		{
			SetGoodValues();
			viewModel.Location = string.Empty;
			AssertValidation(1, "This location does not exist.");
		}

		[TestMethod]
		public void LocationInvalidDirectory()
		{
			SetGoodValues();
			viewModel.Location = @"c:\temp3";
			AssertValidation(1, "This location does not exist.");
		}

		[TestMethod]
		public void LocationInvalidDirectoryCharacters()
		{
			SetGoodValues();
			viewModel.Location = @"c:\temp3:\";
			AssertValidation(1, "This location does not exist.");
		}

		[TestMethod]
		public void CreateProject()
		{
			eventAggregator.GetEvent<CreateProjectEvent>().Subscribe(OnCreateProject);
			SetGoodValues();
			viewModel.ProjectName = "createTestProject";
			viewModel.CreateProject();

			Assert.AreEqual(true, createProjectFired);
		}

		[TestMethod]
		public void CreateProjectInvalidInput()
		{
			viewModel.ProjectName = string.Empty;
			viewModel.CreateProject();

			Assert.AreEqual(false, createProjectFired);
		}

		private void OnCreateProject(ProjectBase project)
		{
			createProjectFired = true;
			Assert.AreEqual("createTestProject", project.Name);
			Assert.AreEqual(@"C:\temp\createTestProject\createTestProject.mssproj", project.Location);
			Assert.AreEqual(1, project.ExperimentReferences.Count);
			Assert.AreEqual("testExperiment", project.ExperimentReferences[0].Name);
			Assert.AreEqual(@"testExperiment\testExperiment.mssexp", project.ExperimentReferences[0].Location);
		}

		private void SetGoodValues()
		{
			viewModel.ExperimentName = "testExperiment";
			viewModel.Location = @"C:\temp";
			viewModel.ProjectName = "testProject";
		}

		private void AssertValidation(int expectedValidationCount, string expectedMessage)
		{
			Assert.AreEqual(expectedValidationCount, Validation.Validate(viewModel).Count());
			bool messageFound = false;
			IList<ValidationResult> errors = Validation.Validate(viewModel).ToList();
			for (int i = 0; i < expectedValidationCount; i++)
			{
				if (errors[i].Message == expectedMessage)
				{
					messageFound = true;
				}
			}
			Assert.IsTrue(messageFound);
		}
	}
}
