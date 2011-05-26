using System.Windows;
using AvalonDock;

namespace MassSpecStudio.Composition
{
	public static class AvalonDockExtension
	{
		/// <summary>
		/// Force parents to redraw.
		/// </summary>
		/// <param name="content">The dockable content</param>
		public static void InvalidateParents(this DockableContent content)
		{
			InvalidateParents(content as FrameworkElement);
		}

		/// <summary>
		/// Force parents to redraw.
		/// </summary>
		/// <param name="content">The document content.</param>
		public static void InvalidateParents(this DocumentContent content)
		{
			InvalidateParents(content as FrameworkElement);
		}

		/// <summary>
		/// Force parents to redraw.
		/// Compensates redraw problem.
		/// Created 18.02.2009 by Markus Raufer
		/// </summary>
		/// <param name="content">The framework element.</param>
		private static void InvalidateParents(FrameworkElement content)
		{
			var parentElement = content.Parent as FrameworkElement;

			while (parentElement != null)
			{
				parentElement.InvalidateMeasure();
				parentElement = parentElement.Parent as FrameworkElement;
			}
		}
	}
}
