using System.IO;

namespace MassSpecStudio.Core
{
	public static class RecentListHelper
	{
		private const int NumberOfRecentItems = 5;
		
		public static string AddToRecentList(string currentList, string newEntry)
		{
			if (!currentList.Contains(newEntry + ";"))
			{
				currentList = newEntry + ";" + currentList;
			}

			string[] existingEntry = currentList.Split(';');
			currentList = string.Empty;
			for (int i = 0; i < existingEntry.Length && i < NumberOfRecentItems; i++)
			{
				if (File.Exists(existingEntry[i]) || Directory.Exists(existingEntry[i]))
				{
					currentList += existingEntry[i] + ";";
				}
			}

			return currentList;
		}
	}
}
