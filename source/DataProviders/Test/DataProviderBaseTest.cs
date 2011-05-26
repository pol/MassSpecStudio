using System;
using System.Collections.Generic;
using MassSpecStudio.Core;
using MassSpecStudio.Core.Domain;
using MassSpecStudio.Core.Events;
using Microsoft.Practices.Prism.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MassSpecStudio.DataProvider.Test
{
	public abstract class DataProviderBaseTest
	{
		private IDataProvider _provider;
		private Mock<IEventAggregator> _mockEventAggregator;
		private string msDataFilePath;
		private string msmsDataFilePath;

		public DataProviderBaseTest(string msDataFilePath, string msmsDataFilePath)
		{
			this.msDataFilePath = msDataFilePath;
			this.msmsDataFilePath = msmsDataFilePath;
		}

		public Mock<IEventAggregator> MockEventAggregator
		{
			get { return _mockEventAggregator; }
		}

		public IDataProvider Provider
		{
			get { return _provider; }
		}

		public abstract IDataProvider GetNewDataProvider();

		public virtual void TestInitialize()
		{
			_mockEventAggregator = new Mock<IEventAggregator>();
			_mockEventAggregator.Setup(mock => mock.GetEvent<OutputEvent>()).Returns(new OutputEvent());
			_provider = GetNewDataProvider();
		}

		public virtual void TestCleanup()
		{
			_provider.Close();
		}

		public virtual void GetSampleList()
		{
			_provider.Open(msDataFilePath, 0);
			IList<string> sampleNames = _provider.GetSampleNames();

			Assert.AreEqual(0, sampleNames.Count);
		}

		public virtual void GetTotalIonChromatogram()
		{
			_provider.Open(msDataFilePath, 0);
			IXYData xyData = _provider.GetTotalIonChromatogram(TimeUnit.Seconds);

			Assert.AreEqual(1500, xyData.Count);
			Assert.AreEqual(7.5832000000000006d, xyData.XValues[454]);
			Assert.AreEqual(69884, xyData.YValues[454]);
		}

		public virtual void GetExtractedIonChromatogram()
		{
			_provider.Open(msDataFilePath, 0);

			IXYData xyData = _provider.GetExtractedIonChromatogram(592.3, 592.3, 0.01, MassSpecStudio.Core.TimeUnit.Seconds);

			Assert.AreEqual(1500, xyData.Count);
			Assert.AreEqual(142, xyData.YValues[454]);
			Assert.AreEqual(7.5832000000000006d, xyData.XValues[454]);
		}

		public virtual void GetSpectrum()
		{
			_provider.Open(msDataFilePath, 0);
			IXYData xyData = _provider.GetSpectrum(7.4, 7.4, 590, 600, 0);

			Assert.AreEqual(339, xyData.Count);
			Assert.AreEqual(5, xyData.YValues[250]);
			Assert.AreEqual(595.4766845703, Math.Round(xyData.XValues[250], 10));
		}

		public virtual void GetAveragedSpectrum()
		{
			_provider.Open(msDataFilePath, 0);
			IXYData xyData = _provider.GetSpectrum(7.4, 7.6, 590, 600, 0);

			Assert.AreEqual(339, xyData.Count);
			Assert.AreEqual(6, xyData.YValues[250]);
			Assert.AreEqual(595.4766845703, Math.Round(xyData.XValues[250], 10));
		}

		public virtual void GetMsMsSpectrum()
		{
			_provider.Open(msmsDataFilePath, 0);
			IXYData xyData = _provider.GetMsMsSpectrum(7.8, 1, 592.3, 0.5);

			Assert.AreEqual(14264, xyData.Count);
			Assert.AreEqual(38.05, xyData.GetXYPair(422).YValue);
			Assert.AreEqual(173.0875549316, Math.Round(xyData.GetXYPair(422).XValue, 10));
		}
	}
}
