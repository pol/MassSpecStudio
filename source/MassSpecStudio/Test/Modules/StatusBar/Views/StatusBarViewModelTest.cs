using MassSpecStudio.Core.Events;
using MassSpecStudio.Modules.StatusBar.Views;
using Microsoft.Practices.Prism.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MassSpecStudio.Test.Modules.StatusBar.Views
{
	[TestClass]
	public class StatusBarViewModelTest
	{
		private IEventAggregator eventAggregator;
		private StatusBarViewModel viewModel;

		[TestInitialize]
		public void TestInitialize()
		{
			eventAggregator = new EventAggregator();
			viewModel = new StatusBarViewModel(eventAggregator);
		}

		[TestMethod]
		public void Constructor()
		{
			Assert.AreEqual("Ready", viewModel.StatusMessage);
		}

		[TestMethod]
		public void UpdateStatusMessage()
		{
			eventAggregator.GetEvent<StatusUpdateEvent>().Publish("test");

			Assert.AreEqual("test", viewModel.StatusMessage);
		}
	}
}
