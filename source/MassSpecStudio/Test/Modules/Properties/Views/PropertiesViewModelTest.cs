using MassSpecStudio.Core.Events;
using MassSpecStudio.Modules.Properties.Views;
using Microsoft.Practices.Prism.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MassSpecStudio.Test.Modules.Properties.Views
{
	[TestClass]
	public class PropertiesViewModelTest
	{
		private IEventAggregator eventAggregator;
		private PropertiesViewModel viewModel;

		[TestInitialize]
		public void TestInitialize()
		{
			eventAggregator = new EventAggregator();
			viewModel = new PropertiesViewModel(eventAggregator);
		}

		[TestMethod]
		public void Properties()
		{
			viewModel.ObjectInstance = "test";
			Assert.AreEqual("test", viewModel.ObjectInstance);
		}

		[TestMethod]
		public void OnSelectionChanged()
		{
			eventAggregator.GetEvent<ObjectSelectionEvent>().Publish(2);

			Assert.AreEqual(2, viewModel.ObjectInstance);
		}
	}
}
