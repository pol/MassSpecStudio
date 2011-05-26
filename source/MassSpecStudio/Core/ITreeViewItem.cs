
namespace MassSpecStudio.Core
{
	public interface ITreeViewItem
	{
		bool IsSelected { get; set; }

		bool IsExpanded { get; set; }
	
		string Name { get; set; }

		object Data { get; set; }
	}
}
