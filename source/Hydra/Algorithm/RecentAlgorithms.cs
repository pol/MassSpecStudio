using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace Hydra.Processing.Algorithm
{
	public static class RecentAlgorithms
	{
		private const int NumberOfAlgorithmsInRecentHistory = 5;

		public static IList<MassSpecStudio.Core.Domain.Algorithm> Read()
		{
			IList<MassSpecStudio.Core.Domain.Algorithm> recentAlgorithms = new List<MassSpecStudio.Core.Domain.Algorithm>();
			string recentAlgortihmsUsed = Properties.Settings.Default.RecentAlgorithms;

			string[] algorithms = recentAlgortihmsUsed.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string algorithm in algorithms)
			{
				try
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(MassSpecStudio.Core.Domain.Algorithm));
					using (XmlReader stream = XmlReader.Create(new StringReader(algorithm)))
					{
						MassSpecStudio.Core.Domain.Algorithm recentAlgorithm = serializer.ReadObject(stream, true) as MassSpecStudio.Core.Domain.Algorithm;
						recentAlgorithms.Add(recentAlgorithm);
					}
				}
				catch
				{
					// Do Nothing
				}
			}
			return recentAlgorithms;
		}

		public static void Write(MassSpecStudio.Core.Domain.Algorithm algorithm)
		{
			IList<MassSpecStudio.Core.Domain.Algorithm> allAlgorithms = Read();

			IList<MassSpecStudio.Core.Domain.Algorithm> algorithms = allAlgorithms.Where(item => item.Name == algorithm.Name).ToList();

			while (algorithms.Count >= NumberOfAlgorithmsInRecentHistory)
			{
				MassSpecStudio.Core.Domain.Algorithm oldestAlgorithm = algorithms.First();
				algorithms.Remove(oldestAlgorithm);
				allAlgorithms.Remove(oldestAlgorithm);
			}

			allAlgorithms.Add(algorithm);
			Properties.Settings.Default.RecentAlgorithms = ConvertListToString(allAlgorithms);
			Properties.Settings.Default.Save();
		}

		private static string ConvertListToString(IList<MassSpecStudio.Core.Domain.Algorithm> algorithms)
		{
			string stringValue = string.Empty;
			foreach (MassSpecStudio.Core.Domain.Algorithm algorithm in algorithms)
			{
				DataContractSerializer serializer = new DataContractSerializer(typeof(MassSpecStudio.Core.Domain.Algorithm));
				StringBuilder builder = new StringBuilder();
				using (XmlWriter stream = XmlWriter.Create(new StringWriter(builder)))
				{
					serializer.WriteObject(stream, algorithm);
				}
				stringValue += builder.ToString() + "|";
			}
			return stringValue;
		}
	}
}
