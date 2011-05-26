using System.Collections.Specialized;
using System.ComponentModel.Composition;
using AvalonDock;
using Microsoft.Practices.Prism.Regions;

namespace MassSpecStudio.Composition.Regions
{
	/// <summary>
	/// created 18.02.2009 by Markus Raufer
	/// </summary>
	[Export]
	public class DocumentPaneRegionAdapter : RegionAdapterBase<DocumentPane>
	{
		[ImportingConstructor]
		public DocumentPaneRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
			: base(regionBehaviorFactory)
		{
		}

		protected override IRegion CreateRegion()
		{
			return new Region();
		}

		protected override void Adapt(IRegion region, DocumentPane regionTarget)
		{
			region.Views.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs e)
			{
				OnViewsCollectionChanged(e, regionTarget);
			};
		}

		private static void OnViewsCollectionChanged(NotifyCollectionChangedEventArgs e, DocumentPane regionTarget)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					foreach (object item in e.NewItems)
					{
						var newContent = item as DocumentContent;

						if (newContent != null)
						{
							regionTarget.Items.Add(newContent);
							newContent.InvalidateParents();
						}
					}
					break;
			}
		}
	}
}
