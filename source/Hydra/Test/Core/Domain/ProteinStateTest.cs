using Hydra.Core.Domain;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Hydra.Test.Core.Domain
{
	[TestClass]
	public class ProteinStateTest
	{
		private Mock<IServiceLocator> mockServiceLocator;
		private ProteinState proteinState;
		private Experiment experiment;

		[TestInitialize]
		public void TestInitialize()
		{
			mockServiceLocator = new Mock<IServiceLocator>();
			experiment = TestHelper.GetTestExperiment(mockServiceLocator);
			proteinState = new ProteinState(experiment);
		}

		[TestMethod]
		public void Properties()
		{
			proteinState.Name = "test";
			Assert.AreEqual("test", proteinState.Name);
		}

		[TestMethod]
		public void GetNextName()
		{
			Assert.AreEqual("Protein state 1", proteinState.Name);
			ProteinState proteinState2 = new ProteinState(experiment);
			Assert.AreEqual("Protein state 2", proteinState2.Name);
		}

		[TestMethod]
		public void EqualOperator()
		{
			ProteinState proteinState2 = new ProteinState(experiment);

			Assert.IsFalse(proteinState == proteinState2);
			proteinState2.Name = "Protein state 1";
			Assert.IsTrue(proteinState == proteinState2);
		}

		[TestMethod]
		public void NotEqualOperator()
		{
			ProteinState proteinState2 = new ProteinState(experiment);

			Assert.IsTrue(proteinState != proteinState2);
			proteinState2.Name = "Protein state 1";
			Assert.IsFalse(proteinState != proteinState2);
		}

		[TestMethod]
		public void GetHashCodeTest()
		{
			Assert.IsTrue(proteinState.GetHashCode() > 0);
		}

		[TestMethod]
		public void Equals()
		{
			ProteinState proteinState2 = new ProteinState(experiment);

			Assert.IsFalse(proteinState.Equals(proteinState2));
			proteinState2.Name = "Protein state 1";
			Assert.IsTrue(proteinState.Equals(proteinState2));
		}

		[TestMethod]
		public void EqualsWithNull()
		{
			Assert.IsFalse(proteinState.Equals(null));
		}
	}
}
