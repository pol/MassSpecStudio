using System.Collections.Specialized;
using System.ComponentModel.Composition;
using AvalonDock;
using Microsoft.Practices.Prism.Regions;

namespace MassSpecStudio.Composition.Regions
{
	/// <summary>
	/// RegionAdapter that creates a new <see cref="Region"/> and binds all
	/// the views to the adapted <see cref="DockingManager"/> or <see cref="ResizingPanel"/>.
	/// Just insert a XAML-Tag xmlns:DockablePane into your XAML-file (e.g. Shell.xaml).
	/// The xml namespace must be previously defined (e.g. xmlns:AvalonDock=...)
	/// Created 18.02.2009 by Markus Raufer
	/// </summary>
	[Export]
	public class DockablePaneRegionAdapter : RegionAdapterBase<DockablePane>
	{
		/// <summary>
		/// Initializes a new instance of the DockablePaneRegionAdapter class.
		/// This is the default constructor.
		/// Let the container do the creation of the RegionBehaviorFactory.
		/// Just get it by injection.
		/// </summary>
		/// <param name="regionBehaviorFactory">The region behaviour factory.</param>
		[ImportingConstructor]
		public DockablePaneRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
			: base(regionBehaviorFactory)
		{
		}

		/// <summary>
		/// The new region which will be placed at the palceholder in the XAML-file.
		/// </summary>
		/// <returns>Returns the created region.</returns>
		protected override IRegion CreateRegion()
		{
			return new Region();
		}

		/// <summary>
		/// This method will be called, each time the parser encounters a XAML-Tag xmlns:DockablePane.
		/// </summary>
		/// <param name="region">The region with the encountered DockablePane.</param>
		/// <param name="regionTarget">The encountered DockablePane.</param>
		protected override void Adapt(IRegion region, DockablePane regionTarget)
		{
			region.Views.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs e)
			{
				OnViewsCollectionChanged(e, regionTarget);
			};
		}

		/// <summary>
		/// Adds the DockableContent to the DockablePane.
		/// This method will be called on each change of the regions view collection.
		/// </summary>
		/// <param name="e">The arguments of the notification event.</param>
		/// <param name="regionTarget">The DockablePane where the Content should be added.</param>
		private static void OnViewsCollectionChanged(NotifyCollectionChangedEventArgs e, DockablePane regionTarget)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					foreach (object item in e.NewItems)
					{
						var newContent = item as DockableContent;

						if (newContent != null)
						{
							regionTarget.Items.Add(newContent);
							newContent.InvalidateParents();
						}
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					foreach (object item in e.OldItems)
					{
						var oldContent = item as DockableContent;

						if (oldContent != null)
						{
							oldContent.Show();
							if (regionTarget.Items.Contains(oldContent))
							{
								regionTarget.Items.Remove(oldContent);
							}
						}
					}
					break;
			}
		}
	}
}
