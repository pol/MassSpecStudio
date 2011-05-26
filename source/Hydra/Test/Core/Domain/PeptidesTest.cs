using System.Collections.Generic;
using Hydra.Core.Domain;
using Hydra.Core.Provider;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Hydra.Test.Core.Domain
{
	[TestClass]
	public class PeptidesTest
	{
		private Mock<IServiceLocator> mockServiceLocator;
		private Mock<IPeptideDataProvider> mockPeptideDataProvider;
		private Peptides peptides;

		[TestInitialize]
		public void TestInitialize()
		{
			mockServiceLocator = new Mock<IServiceLocator>();
			mockPeptideDataProvider = new Mock<IPeptideDataProvider>();

			peptides = new Peptides(@"C:\temp\Data\peptides.xml", mockServiceLocator.Object);
		}

		[TestMethod]
		public void GetProperties()
		{
			Assert.AreEqual(@"C:\temp\Data\peptides.xml", peptides.Location);
			Assert.AreEqual(0, peptides.PeptideCollection.Count);
		}

		[TestMethod]
		public void Open()
		{
			peptides = Peptides.Open(Properties.Settings.Default.XmlPeptideTestFile);

			Assert.AreEqual(Properties.Settings.Default.XmlPeptideTestFile, peptides.Location);
			Assert.AreEqual(5, peptides.PeptideCollection.Count);
			Assert.AreEqual("DGIPSKVQRCAVG", peptides.PeptideCollection[0].Sequence);
			Assert.AreEqual("NENERYDAVQHCRYVDE", peptides.PeptideCollection[1].Sequence);
			Assert.AreEqual("VAHDDIPYSSAGSDDVYKHIKEAGM", peptides.PeptideCollection[2].Sequence);
			Assert.AreEqual("RIVRDYDVYA", peptides.PeptideCollection[3].Sequence);
			Assert.AreEqual("LNVSF", peptides.PeptideCollection[4].Sequence);
		}

		[TestMethod]
		public void Save()
		{
			peptides = Peptides.Open(Properties.Settings.Default.XmlPeptideTestFile);
			peptides.PeptideCollection[0].Sequence = "SAMPLE";
			peptides.Location = @"C:\temp\peptides.xml";
			peptides.Save();

			peptides = Peptides.Open(@"C:\temp\peptides.xml");
			Assert.AreEqual("SAMPLE", peptides.PeptideCollection[0].Sequence);
		}

		[TestMethod]
		public void Add()
		{
			mockPeptideDataProvider.Setup(mock => mock.Read(Properties.Settings.Default.PeptideTestFile1)).Returns(new List<Peptide>() { new Peptide(), new Peptide() });
			mockPeptideDataProvider.Setup(mock => mock.Read(Properties.Settings.Default.PeptideTestFile2)).Returns(new List<Peptide>() { new Peptide(), new Peptide(), new Peptide() });
			mockServiceLocator.Setup(mock => mock.GetInstance<IPeptideDataProvider>()).Returns(mockPeptideDataProvider.Object);

			peptides.Add(Properties.Settings.Default.PeptideTestFile1);
			Assert.AreEqual(2, peptides.PeptideCollection.Count);

			peptides.Add(Properties.Settings.Default.PeptideTestFile2);
			Assert.AreEqual(5, peptides.PeptideCollection.Count);
		}
	}
}
