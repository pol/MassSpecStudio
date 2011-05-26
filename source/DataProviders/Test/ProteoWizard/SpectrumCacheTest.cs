using System;
using System.Collections.Generic;
using MassSpecStudio.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProteoWizard.MassSpecStudio.DataProvider;
using pwiz.CLI.msdata;
using Domain = MassSpecStudio.Core.Domain;

namespace MassSpecStudio.DataProvider.Test.ProteoWizard
{
	[TestClass()]
	public class SpectrumCacheTest
	{
		private SpectrumCache cache;

		[TestInitialize()]
		public void MyTestInitialize()
		{
			cache = new SpectrumCache();
		}

		[TestMethod]
		public void IsInCache()
		{
			AddToCache(1);

			Assert.AreEqual(true, cache.IsInCache(1));
			Assert.AreEqual(false, cache.IsInCache(2));
		}

		[TestMethod]
		public void ReadCache()
		{
			AddToCache(1);

			Domain.ISpectrum xyData = cache.Read(1);

			XYDataHelper.AssertXYData(xyData, XYDataHelper.XValueTestSet1, XYDataHelper.YValueTestSet1);
		}

		[TestMethod]
		[ExpectedException(typeof(KeyNotFoundException))]
		public void ReadCacheThatDoesNotExist()
		{
			cache.Read(3);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void AddCacheValueTwiceAtTheSameRT()
		{
			ReadCache();
			AddToCache(1);
		}

		private void AddToCache(int timePoint)
		{
			BinaryData xValues = new BinaryData();
			BinaryData yValues = new BinaryData();
			for (int i = 0; i < XYDataHelper.XValueTestSet1.Length; i++)
			{
				xValues.Add(XYDataHelper.XValueTestSet1[i]);
				yValues.Add(XYDataHelper.YValueTestSet1[i]);
			}
			cache.Add(timePoint, new BinarySpectrum(1.5, xValues, yValues));
		}
	}
}
