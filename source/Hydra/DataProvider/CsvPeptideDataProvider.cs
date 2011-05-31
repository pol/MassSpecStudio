using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Hydra.Core.Domain;
using Hydra.Core.Provider;

namespace Hydra.DataProvider
{
	[Export(typeof(IPeptideDataProvider))]
	public class CsvPeptideDataProvider : IPeptideDataProvider
	{
		public IList<Peptide> Read(string filename)
		{
			IList<Peptide> peptides = new List<Peptide>();

			IList<string> tableHeaders = new List<string>();
			StreamReader reader = new StreamReader(filename);

			string searchString0 = @"^[iI][dD]";
			string headerline = reader.ReadLine();

			Regex regex = new Regex(searchString0);
			Match match = regex.Match(headerline);

			if (match.Success)
			{
				tableHeaders = ProcessLine(headerline);
			}

			int lineIndex = 1;
			string line;
			do
			{
				++lineIndex;
				line = reader.ReadLine();
				IList<string> processedData = ProcessLine(line);
				if (processedData.Count > tableHeaders.Count)
				{
					throw new Exception("TODO");
				}

				Peptide peptide = new Peptide();

				try
				{
					peptide.Id = ConvertStringToInteger(processedData[GetIndex(tableHeaders, "ID")], -1);
				}
				catch (Exception)
				{
					peptide.Id = -1;
				}

				try
				{
					peptide.ProteinSource = processedData[GetIndex(tableHeaders, "ProteinSource")];
				}
				catch (Exception)
				{
					peptide.ProteinSource = string.Empty;
				}

				try
				{
					peptide.AminoAcidStart = ConvertStringToInteger(processedData[GetIndex(tableHeaders, "AminoAcidStart")], -1);
				}
				catch (Exception)
				{
					peptide.AminoAcidStart = -1;
				}

				try
				{
					peptide.AminoAcidStop = ConvertStringToInteger(processedData[GetIndex(tableHeaders, "AminoAcidStop")], -1);
				}
				catch (Exception)
				{
					peptide.AminoAcidStop = -1;
				}

				try
				{
					peptide.Sequence = processedData[GetIndex(tableHeaders, "Sequence")];
				}
				catch (Exception)
				{
					peptide.Sequence = string.Empty;
				}

				try
				{
					peptide.MonoIsotopicMass = ConvertStringToDouble(processedData[GetIndex(tableHeaders, "MonoIsotopicMass")], -1);
				}
				catch (Exception)
				{
					peptide.MonoIsotopicMass = -1;
				}

				try
				{
					peptide.ChargeState = ConvertStringToInteger(processedData[GetIndex(tableHeaders, "Z")], 0);
				}
				catch (Exception)
				{
					peptide.ChargeState = 0;
				}

				try
				{
					peptide.Period = ConvertStringToInteger(processedData[GetIndex(tableHeaders, "Period")], 0);
				}
				catch (Exception)
				{
					peptide.Period = 0;
				}

				try
				{
					peptide.RT = ConvertStringToDouble(processedData[GetIndex(tableHeaders, "RT")], 0);
				}
				catch (Exception)
				{
					peptide.RT = 0;
				}

				// TODO: reference default rtVar value
				try
				{
					peptide.RtVariance = ConvertStringToDouble(processedData[GetIndex(tableHeaders, "RTVariance")], 0.75);
				}
				catch (Exception)
				{
					peptide.RtVariance = 0.75;
				}

				try
				{
					peptide.XicMass1 = ConvertStringToDouble(processedData[GetIndex(tableHeaders, "XicMass1")], peptide.MonoIsotopicMass);
				}
				catch (Exception)
				{
					peptide.XicMass1 = peptide.MonoIsotopicMass;
				}

				try
				{
					peptide.XicMass2 = ConvertStringToDouble(processedData[GetIndex(tableHeaders, "XicMass2")], 0);
				}
				catch (Exception)
				{
					peptide.XicMass2 = 0;
				}

				try
				{
					peptide.XicAdjustment = ConvertStringToDouble(processedData[GetIndex(tableHeaders, "XicAdjustment")], 0);
				}
				catch (Exception)
				{
					peptide.XicAdjustment = 0;
				}

				// TODO: reference Properties
				try
				{
					peptide.XicSelectionWidth = ConvertStringToDouble(processedData[GetIndex(tableHeaders, "XicSelectionWidth")], 0.2);
				}
				catch (Exception)
				{
					peptide.XicSelectionWidth = 0.2;
				}

				try
				{
					peptide.MsThreshold = ConvertStringToDouble(processedData[GetIndex(tableHeaders, "MsThreshold")], 0);
				}
				catch (Exception)
				{
					peptide.MsThreshold = 0;
				}

				try
				{
					peptide.PeaksInCalculation = ConvertStringToInteger(processedData[GetIndex(tableHeaders, "PeaksInCalculation")], 2);
				}
				catch (Exception)
				{
					peptide.PeaksInCalculation = 2;
				}

				try
				{
					peptide.DeuteriumDistributionThreshold = ConvertStringToInteger(processedData[GetIndex(tableHeaders, "DeutDistThreshold")], 1);
				}
				catch (Exception)
				{
					peptide.DeuteriumDistributionThreshold = 1;
				}

				try
				{
					peptide.DeuteriumDistributionRightPadding = ConvertStringToInteger(processedData[GetIndex(tableHeaders, "DeutDistRightPadding")], 2);
				}
				catch (Exception)
				{
					peptide.DeuteriumDistributionRightPadding = 2;
				}

				try
				{
					peptide.Notes = processedData[GetIndex(tableHeaders, "Notes")];
				}
				catch (Exception)
				{
					peptide.Notes = string.Empty;
				}

				peptides.Add(peptide);
			}
			while (reader.Peek() > -1);

			return peptides;
		}

		public void Write(IList<Peptide> peptides, string filename)
		{
			StreamWriter sw;
			try
			{
				sw = new StreamWriter(filename);
			}
			catch (System.IO.IOException)
			{
				throw new System.IO.IOException("There was a problem writing to the file - check filename");
			}
			catch (Exception)
			{
				throw new Exception("There was a problem writing to the file");
			}

			sw.WriteLine("ID,ProteinSource,AminoAcidStart,AminoAcidStop,Sequence,MonoIsotopicMass,Z,Period,RT,RTvariance,XicMass1,XicMass2,XicAdjustment,XicSelectionWidth,XicPeakPickerOption,MsThreshold,PeaksInCalculation,DeutDistThreshold,DeutDistRightPadding,Notes");

			for (int i = 0; i < peptides.Count; i++)
			{
				StringBuilder sb = new StringBuilder();
				sb.Append(peptides[i].Id);
				sb.Append(",");
				sb.Append(peptides[i].ProteinSource);
				sb.Append(",");
				sb.Append(peptides[i].AminoAcidStart);
				sb.Append(",");
				sb.Append(peptides[i].AminoAcidStop);
				sb.Append(",");
				sb.Append(peptides[i].Sequence);
				sb.Append(",");
				sb.Append(peptides[i].MonoIsotopicMass);
				sb.Append(",");
				sb.Append(peptides[i].ChargeState);
				sb.Append(",");
				sb.Append(peptides[i].Period);
				sb.Append(",");
				sb.Append(peptides[i].RT);
				sb.Append(",");
				sb.Append(peptides[i].RtVariance);
				sb.Append(",");
				sb.Append(peptides[i].XicMass1);
				sb.Append(",");
				sb.Append(peptides[i].XicMass2);
				sb.Append(",");
				sb.Append(peptides[i].XicAdjustment);
				sb.Append(",");
				sb.Append(peptides[i].XicSelectionWidth);
				sb.Append(",");
				sb.Append(Convert.ToInt32(peptides[i].XicPeakPickerOption));
				sb.Append(",");
				sb.Append(peptides[i].MsThreshold);
				sb.Append(",");
				sb.Append(peptides[i].PeaksInCalculation);
				sb.Append(",");
				sb.Append(peptides[i].DeuteriumDistributionThreshold);
				sb.Append(",");
				sb.Append(peptides[i].DeuteriumDistributionRightPadding);
				sb.Append(",");
				sb.Append(peptides[i].Notes);
				sw.WriteLine(sb.ToString());
			}
			sw.Close();
		}

		private double ConvertStringToDouble(string inputString, double defaultValue)
		{
			if (inputString == string.Empty || inputString == null)
			{
				return defaultValue;
			}
			else
			{
				try
				{
					return Convert.ToDouble(inputString);
				}
				catch (Exception)
				{
					return double.NaN;
				}
			}
		}

		private int ConvertStringToInteger(string inputString, int defaultValue)
		{
			if (inputString == string.Empty || inputString == null)
			{
				return defaultValue;
			}
			else
			{
				try
				{
					return Convert.ToInt32(inputString);
				}
				catch (Exception)
				{
					return -1;
				}
			}
		}

		private int GetIndex(IList<string> tableHeaders, string target)
		{
			for (int i = 0; i < tableHeaders.Count; i++)
			{
				if (tableHeaders[i].ToLower() == target.ToLower())
				{
					return i;
				}
			}

			// didn't find header!
			return -1;
		}

		private IList<string> ProcessLine(string line)
		{
			bool isWithinQuote = false;
			StringBuilder sb = new StringBuilder();

			for (int i = 0; i < line.Length; i++)
			{
				// this is the " character
				if (line[i] == '\x0022')
				{
					if (isWithinQuote)
					{
						isWithinQuote = false;
					}
					else
					{
						isWithinQuote = true;
					}
				}

				if (isWithinQuote)
				{
					// checks for ','
					if (line[i] == '\x002C')
					{
						// this is the ~ symbol
						sb.Append('\x007E');
					}
					else
					{
						sb.Append(line[i]);
					}
				}
				else
				{
					sb.Append(line[i]);
				}
			}

			line = sb.ToString();

			char[] splitter = { ',' };
			List<string> returnedList = new List<string>();
			string[] arr = line.Split(splitter);
			foreach (string str in arr)
			{
				StringBuilder sb2 = new StringBuilder();

				// I'm sure there is a cleaner way to do this!!
				for (int i = 0; i < str.Length; i++)
				{
					// this replaces '~' with ','
					if (str[i] == '\x007E')
					{
						sb2.Append('\x002C');
					}
					else
					{
						sb2.Append(str[i]);
					}
				}
				returnedList.Add(sb2.ToString());
			}
			return returnedList;
		}
	}
}
