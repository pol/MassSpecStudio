using System;
using System.IO;
using System.Text;

namespace MassSpecStudio.Core
{
	public static class DirectoryHelper
	{
		public static string GetRelativePath(string fullDestinationPath, string startPath)
		{
			string[] startPathParts = Path.GetFullPath(startPath).Trim(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar);
			string[] destinationPathParts = fullDestinationPath.Split(Path.DirectorySeparatorChar);

			int sameCounter = 0;
			while ((sameCounter < startPathParts.Length) && (sameCounter < destinationPathParts.Length) && startPathParts[sameCounter].Equals(destinationPathParts[sameCounter], StringComparison.InvariantCultureIgnoreCase))
			{
				sameCounter++;
			}

			if (sameCounter == 0)
			{
				return fullDestinationPath; // There is no relative link.
			}

			StringBuilder builder = new StringBuilder();
			for (int i = sameCounter; i < startPathParts.Length; i++)
			{
				builder.Append(".." + Path.DirectorySeparatorChar);
			}

			for (int i = sameCounter; i < destinationPathParts.Length; i++)
			{
				builder.Append(destinationPathParts[i] + Path.DirectorySeparatorChar);
			}

			builder.Length--;

			return builder.ToString();
		}
	}
}
