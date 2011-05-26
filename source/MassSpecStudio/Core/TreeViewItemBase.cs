using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MassSpecStudio.Core
{
	public abstract class TreeViewItemBase<TParent, TChild>
		: ViewModelBase, ITreeViewItem
		where TParent : ITreeViewItem
		where TChild : ITreeViewItem
	{
		private ObservableCollection<TChild> _children;
		private TParent _parent;
		private bool _isExpanded;
		private bool _isSelected;
		private object _data;

		protected TreeViewItemBase()
		{
			_children = new ObservableCollection<TChild>();
		}

		[Browsable(false)]
		public ObservableCollection<TChild> Children
		{
			get { return _children; }
		}

		[Browsable(false)]
		public object Data
		{
			get { return _data; }
			set { _data = value; }
		}

		[Browsable(false)]
		public TParent Parent
		{
			get { return _parent; }
			set { _parent = value; }
		}

		[Browsable(false)]
		public virtual bool IsSelected
		{
			get
			{
				return _isSelected;
			}

			set
			{
				if (value != _isSelected)
				{
					_isSelected = value;
					NotifyPropertyChanged(() => IsSelected);
				}
			}
		}

		[Browsable(false)]
		public virtual bool IsExpanded
		{
			get
			{
				return _isExpanded;
			}

			set
			{
				if (value != _isExpanded)
				{
					_isExpanded = value;
					NotifyPropertyChanged(() => IsExpanded);
				}

				// Expand all the way up to the root.
				if (_isExpanded && _parent != null)
				{
					_parent.IsExpanded = true;
				}
			}
		}

		[Browsable(false)]
		public abstract string Name { get; set; }
	}
}
