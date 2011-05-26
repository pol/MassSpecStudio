using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ServiceLocation;

namespace Microsoft.Practices.Prism.Regions
{
	public static class RegionManagerExtensions
	{
		public static void Display(this IRegionManager regionManager, string regionName, Type viewType, IServiceLocator serviceLocator)
		{
			object existingView = regionManager.Regions[regionName].Views.Where(view => view.GetType() == viewType).FirstOrDefault();

			if (existingView == null)
			{
				existingView = serviceLocator.GetInstance(viewType);
				regionManager.AddToRegion(regionName, existingView);
			}
			regionManager.Regions[regionName].Activate(existingView);
		}

		public static void Remove(this IRegionManager regionManager, string regionName, Type viewType)
		{
			IList<object> existingViews = regionManager.Regions[regionName].Views.Where(view => view.GetType() == viewType).ToList();

			foreach (object existingView in existingViews)
			{
				regionManager.Regions[regionName].Remove(existingView);
			}
		}
	}
}
