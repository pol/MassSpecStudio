using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Regions;

namespace MassSpecStudio.Composition.Adapter
{
	[Export]
	public class MenuRegionAdapter : RegionAdapterBase<Menu>
	{
		[ImportingConstructor]
		public MenuRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
			: base(regionBehaviorFactory)
		{
		}

		protected override IRegion CreateRegion()
		{
			return new Region();
		}

		protected override void Adapt(IRegion region, Menu regionTarget)
		{
			region.Views.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs e)
			{
				OnViewsCollectionChanged(e, regionTarget);
			};
		}

		private static void OnViewsCollectionChanged(NotifyCollectionChangedEventArgs e, Menu regionTarget)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					
					foreach (object item in e.NewItems)
					{
						var newMenu = item as MenuItem;
						
						if (newMenu != null)
						{
							regionTarget.Items.Insert(regionTarget.Items.Count - 2, newMenu);
						}
					}
					break;
			}
		}
	}
}
