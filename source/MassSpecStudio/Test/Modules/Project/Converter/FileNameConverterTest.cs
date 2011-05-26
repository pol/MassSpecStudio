using System.Globalization;
using System.IO;
using MassSpecStudio.Modules.Project.Converter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MassSpecStudio.Test.Modules.Project.Converter
{
	[TestClass]
	public class FileNameConverterTest
	{
		private FileNameConverter converter;

		[TestInitialize]
		public void TestInitialize()
		{
			if (!File.Exists(@"c:\temp\test.txt"))
			{
				File.WriteAllText(@"C:\temp\test.txt", "test");
			}
			converter = new FileNameConverter();
		}

		[TestMethod]
		public void ConvertWhenExists()
		{
			Assert.AreEqual("test", converter.Convert(@"C:\temp\test.txt", typeof(string), null, CultureInfo.InvariantCulture));
		}

		[TestMethod]
		public void ConvertWithFileThatDoesNotExist()
		{
			Assert.AreEqual(@"C:\temp\test2.txt", converter.Convert(@"C:\temp\test2.txt", typeof(string), null, CultureInfo.InvariantCulture));
		}

		[TestMethod]
		public void ConvertBack()
		{
			Assert.AreEqual(null, converter.ConvertBack(null, typeof(string), null, CultureInfo.InvariantCulture));
		}
	}
}
