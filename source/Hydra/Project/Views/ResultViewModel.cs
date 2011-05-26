using System.ComponentModel;
using Hydra.Core.Domain;
using MassSpecStudio.Core;

namespace Hydra.Modules.Project.Views
{
	[DefaultProperty("Name")]
	public class ResultViewModel : TreeViewItemBase<ResultsViewModel, ResultViewModel>
	{
		private Result _result;

		public ResultViewModel(Result result)
		{
			_result = result;
			Data = _result;
		}

		[Browsable(true)]
		[Category("Basic Information")]
		public override string Name
		{
			get
			{
				return _result.Name;
			}

			set
			{
				_result.Name = value;
				NotifyPropertyChanged("Name");
			}
		}

		[ReadOnly(true)]
		[Category("Result Details")]
		[DisplayName("Algotihm Used")]
		public string AlgorithmUsed
		{
			get { return _result.AlgorithmUsed != null ? _result.AlgorithmUsed.Name : string.Empty; }
		}

		[ReadOnly(true)]
		[Category("Result Details")]
		[DisplayName("Is Manually Validated")]
		public bool IsManuallyValidated
		{
			get { return _result.IsManuallyValidated; }
		}

		[ReadOnly(true)]
		[Category("Result Details")]
		[DisplayName("# of Results")]
		public string NumberOfResults
		{
			get { return _result.RunResults.Count.ToString(); }
		}
	}
}
