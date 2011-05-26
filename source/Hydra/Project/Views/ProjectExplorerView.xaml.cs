using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using AvalonDock;
using Hydra.Core.Events;
using MassSpecStudio.Core;
using Microsoft.Practices.Prism.Events;

namespace Hydra.Modules.Project.Views
{
	/// <summary>
	/// Interaction logic for ProjectView.xaml
	/// </summary>
	[Export]
	public partial class ProjectExplorerView : DockableContent
	{
		private readonly IEventAggregator _eventAggregator;

		[ImportingConstructor]
		public ProjectExplorerView(ProjectExplorerViewModel viewModel, IEventAggregator eventAggregator)
		{
			DataContext = viewModel;
			InitializeComponent();
			_eventAggregator = eventAggregator;
		}

		public ProjectExplorerViewModel ViewModel
		{
			get
			{
				return (ProjectExplorerViewModel)DataContext;
			}
		}

		private void OnSelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			if (e.NewValue != e.OldValue)
			{
				ViewModel.SelectedItem = e.NewValue as ITreeViewItem;
				ViewModel.OnSelectionChanged();
			}
		}

		private void OnDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (ViewModel.SelectedItem != null)
			{
				_eventAggregator.GetEvent<FileClickedEvent>().Publish(ViewModel.SelectedItem.Data);
			}
		}

		private void OnAddRun(object sender, RoutedEventArgs e)
		{
			LabelingViewModel selectedLabel = ((MenuItem)sender).DataContext as LabelingViewModel;

			System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();

			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				selectedLabel.AddRunDataAndCopyFile.Execute(dialog.FileName);
			}
		}

		private void OnKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.Delete && ViewModel.SelectedItem is PeptideViewModel)
			{
				PeptideViewModel selectedPeptide = (PeptideViewModel)ViewModel.SelectedItem;
				PeptidesViewModel peptides = selectedPeptide.Parent as PeptidesViewModel;
				int index = peptides.Children.IndexOf(selectedPeptide);
				peptides.Remove.Execute(selectedPeptide);

				if (index < peptides.Children.Count)
				{
					peptides.Children[index].IsSelected = true;
				}
				else
					if ((index - 1) < peptides.Children.Count && (index - 1) >= 0 && peptides.Children.Count > 0)
					{
						peptides.Children[index - 1].IsSelected = true;
					}
			}
		}
	}
}
