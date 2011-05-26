using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hydra.Test.Core.Domain
{
	[TestClass]
	public class DeuterationResultTest
	{
		////private Mock<IServiceLocator> mockServiceLocator;
		////private DeuterationResult2 deuterationResult;

		////[TestInitialize]
		////public void TestInitialize()
		////{
		////    mockServiceLocator = new Mock<IServiceLocator>();
		////    deuterationResult = new DeuterationResult2(new Peptide("SAMPLE"), new Labeling(TestHelper.GetTestExperiment(mockServiceLocator)));
		////}

		////[TestMethod]
		////public void Constructor()
		////{
		////    Assert.AreEqual("SAMPLE", deuterationResult.Peptide.Sequence);
		////    Assert.IsNotNull(deuterationResult.Labeling);
		////}

		////[TestMethod]
		////public void ProteinStateDeuterationResults()
		////{
		////    IList<ProteinStateDeuterationResult> results = new List<ProteinStateDeuterationResult>();
		////    results.Add(new ProteinStateDeuterationResult());
		////    results.Add(new ProteinStateDeuterationResult());

		////    deuterationResult.ProteinStateDeuterationResults = results;

		////    Assert.AreEqual(2, deuterationResult.ProteinStateDeuterationResults.Count);
		////}

		[TestMethod]
		[Ignore]
		public void ToStringTest()
		{
			//// TODO: Reenable the code commented out above.
			throw new NotImplementedException();
		}
	}
}
