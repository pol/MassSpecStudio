using Hydra.Core.Domain;
using Hydra.Modules.Project.Views;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hydra.Test.Modules.Project.Views
{
	[TestClass]
	public class ResultViewModelTest
	{
		[TestMethod]
		public void Name()
		{
			ResultViewModel viewModel = new ResultViewModel(new Result("name", null));
			Assert.AreEqual("name", viewModel.Name);
		}
	}
}
