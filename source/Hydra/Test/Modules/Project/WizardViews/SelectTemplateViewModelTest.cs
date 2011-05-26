using Hydra.Modules.Project.WizardViews;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hydra.Test.Modules.Project.WizardViews
{
	[TestClass]
	public class SelectTemplateViewModelTest
	{
		[TestMethod]
		public void Properties()
		{
			SelectTemplateViewModel viewModel = new SelectTemplateViewModel(null, null);
			Assert.AreEqual(typeof(SelectTemplateView), viewModel.ViewType);
			Assert.AreEqual("Select a project as a template", viewModel.Title);
		}
	}
}
