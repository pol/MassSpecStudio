using System;
using Hydra.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hydra.Test.Core.Domain
{
	[TestClass]
	public class FragmentIonTest
	{
		private FragmentIon fragmentIon;

		[TestInitialize]
		public void TestInitialize()
		{
			fragmentIon = new FragmentIon("SAMPLE", new Peptide());
		}

		[TestMethod]
		public void Properties()
		{
			Guid guid = Guid.NewGuid();
			fragmentIon.Id = guid;
			Assert.AreEqual(guid, fragmentIon.Id);

			fragmentIon.IonSeriesNumber = 1;
			Assert.AreEqual(1, fragmentIon.IonSeriesNumber);

			fragmentIon.Sequence = "A";
			Assert.AreEqual("A", fragmentIon.Sequence);

			fragmentIon.FragmentIonType = Hydra.Core.FragmentIonType.BFragment;
			Assert.AreEqual(Hydra.Core.FragmentIonType.BFragment, fragmentIon.FragmentIonType);

			fragmentIon.Peptide = new Peptide("SAMPLE");
			Assert.AreEqual("SAMPLE", fragmentIon.Peptide.Sequence);

			fragmentIon.MsThreshold = 2.5;
			Assert.AreEqual(2.5, fragmentIon.MsThreshold);

			fragmentIon.Notes = "test1";
			Assert.AreEqual("test1", fragmentIon.Notes);

			fragmentIon.MZ = 3.5;
			Assert.AreEqual(3.5, fragmentIon.MZ);

			fragmentIon.ChargeState = 2;
			Assert.AreEqual(2, fragmentIon.ChargeState);

			fragmentIon.PeaksInCalculation = 3;
			Assert.AreEqual(3, fragmentIon.PeaksInCalculation);

			fragmentIon.DeutDistThreshold = 4;
			Assert.AreEqual(4, fragmentIon.DeutDistThreshold);

			fragmentIon.DeutDistRightPadding = 5;
			Assert.AreEqual(5, fragmentIon.DeutDistRightPadding);
		}

		[TestMethod]
		public void ToStringTest()
		{
			Properties();

			fragmentIon.ChargeState = 1;
			Assert.AreEqual("BFragment1+", fragmentIon.ToString());
			fragmentIon.ChargeState = 2;
			fragmentIon.FragmentIonType = Hydra.Core.FragmentIonType.CFragment;
			Assert.AreEqual("CFragment1++", fragmentIon.ToString());
			fragmentIon.ChargeState = 3;
			Assert.AreEqual("CFragment1+++", fragmentIon.ToString());
			fragmentIon.ChargeState = -1;
			Assert.AreEqual("CFragment1-", fragmentIon.ToString());
			fragmentIon.ChargeState = -2;
			Assert.AreEqual("CFragment1--", fragmentIon.ToString());
			fragmentIon.ChargeState = -3;
			Assert.AreEqual("CFragment1---", fragmentIon.ToString());
			fragmentIon.ChargeState = 5;
			Assert.AreEqual("CFragment1", fragmentIon.ToString());
		}
	}
}
