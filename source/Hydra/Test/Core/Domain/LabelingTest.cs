using Hydra.Core.Domain;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Hydra.Test.Core.Domain
{
	[TestClass]
	public class LabelingTest
	{
		private Mock<IServiceLocator> mockServiceLocator;
		private Labeling labeling;
		private Experiment experiment;

		[TestInitialize]
		public void TestInitialize()
		{
			mockServiceLocator = new Mock<IServiceLocator>();
			experiment = TestHelper.GetTestExperiment(mockServiceLocator);
			labeling = new Labeling(experiment);
		}

		[TestMethod]
		public void Constructor()
		{
			Assert.AreEqual(1, labeling.LabelingTime);
			Assert.AreEqual(20, labeling.LabelingPercent);
			Assert.AreEqual(experiment, labeling.Experiment);
			Assert.AreEqual(1, experiment.Labeling.Count);
		}

		[TestMethod]
		public void SetNextPercentAndTime()
		{
			labeling = new Labeling(experiment);

			Assert.AreEqual(2, labeling.LabelingTime);
			Assert.AreEqual(30, labeling.LabelingPercent);
		}

		[TestMethod]
		public void Properties()
		{
			labeling.LabelingTime = 3;
			labeling.LabelingPercent = 4;

			Assert.AreEqual(3, labeling.LabelingTime);
			Assert.AreEqual(4, labeling.LabelingPercent);
		}

		[TestMethod]
		public void EqualOperator()
		{
			Labeling labeling2 = new Labeling(experiment);

			Assert.IsFalse(labeling == labeling2);

			labeling2.LabelingTime = 1;
			Assert.IsFalse(labeling == labeling2);

			labeling2.LabelingPercent = 20;
			Assert.IsTrue(labeling == labeling2);
		}

		[TestMethod]
		public void EqualOperatorWithNulls()
		{
			Assert.IsFalse(labeling == (Labeling)null);
			Assert.IsTrue((Labeling)null == (Labeling)null);
		}

		[TestMethod]
		public void NotEqualOperator()
		{
			Labeling labeling2 = new Labeling(experiment);

			Assert.IsTrue(labeling != labeling2);

			labeling2.LabelingTime = 1;
			Assert.IsTrue(labeling != labeling2);

			labeling2.LabelingPercent = 20;
			Assert.IsFalse(labeling != labeling2);
		}

		[TestMethod]
		public void NotEqualOperatorWithNulls()
		{
			Assert.IsTrue(labeling != (Labeling)null);
			Assert.IsFalse((Labeling)null != (Labeling)null);
		}

		[TestMethod]
		public void GetHashCodeTest()
		{
			Assert.IsTrue(labeling.GetHashCode() > 0);
		}

		[TestMethod]
		public void Equals()
		{
			Labeling labeling2 = new Labeling(experiment);

			Assert.IsFalse(labeling.Equals(null));
			Assert.IsFalse(labeling.Equals(labeling2));

			labeling2.LabelingTime = 1;
			Assert.IsFalse(labeling.Equals(labeling2));

			labeling2.LabelingPercent = 20;
			Assert.IsTrue(labeling.Equals(labeling2));
		}
	}
}
