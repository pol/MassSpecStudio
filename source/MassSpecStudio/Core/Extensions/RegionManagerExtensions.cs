using System;
using System.Collections.Generic;
using System.Linq;
using AvalonDock;

namespace Microsoft.Practices.Prism.Regions
{
	public static class RegionManagerExtensions
	{
		public static ManagedContent FindExistingView(this IRegionManager regionManager, string regionName, Type viewType, string title)
		{
			IList<ManagedContent> currentViews = regionManager.Regions[regionName].Views.Where(item => item.GetType() == viewType).Cast<ManagedContent>().ToList();
			return (from view in currentViews
					where view.Title == title
					select view).FirstOrDefault();
		}

		public static ManagedContent FindExistingView(this IRegionManager regionManager, string regionName, Type viewType)
		{
			return regionManager.Regions[regionName].Views.Where(item => item.GetType() == viewType).Cast<ManagedContent>().FirstOrDefault();
		}
	}
}
