using MassSpecStudio.Core;
using MassSpecStudio.Modules.Project.Views;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MassSpecStudio.Test.Modules.Project.Views
{
	[TestClass]
	public class StartPageViewModelTest
	{
		private Mock<IDocumentManager> mockDocumentManager;
		private StartPageViewModel viewModel;

		[TestInitialize]
		public void TestInitialize()
		{
			mockDocumentManager = new Mock<IDocumentManager>();
			MassSpecStudio.Core.Properties.Settings.Default.RecentProjects = "test1.mssproj;test2.mssproj";
			viewModel = new StartPageViewModel(mockDocumentManager.Object);
		}

		[TestMethod]
		public void Constructor()
		{
			Assert.AreEqual(2, viewModel.RecentProjects.Count);
			Assert.AreEqual("test1.mssproj", viewModel.RecentProjects[0]);
			Assert.AreEqual("test2.mssproj", viewModel.RecentProjects[1]);
			Assert.AreEqual(mockDocumentManager.Object, viewModel.DocumentManager);
		}

		[TestMethod]
		public void Open()
		{
			mockDocumentManager.Setup(mock => mock.Open("test.mssproj"));
			viewModel.Open("test.mssproj");

			mockDocumentManager.VerifyAll();
		}
	}
}
