using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using MassSpecStudio.Core;
using MassSpecStudio.Core.Processing;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;

namespace Hydra.Processing.Algorithm.Views
{
	[Export(typeof(ProcessingDialogViewModel))]
	public class ProcessingDialogViewModel : ViewModelBase
	{
		private IList<IAlgorithm> _algorithms;
		private IList<MassSpecStudio.Core.Domain.Algorithm> _recentAlgorithmsUsed;
		private IAlgorithm _selectedAlgorithm;

		[ImportingConstructor]
		public ProcessingDialogViewModel()
		{
			LoadParameters = new DelegateCommand<MassSpecStudio.Core.Domain.Algorithm>(OnLoadParameters);
			_algorithms = ServiceLocator.Current.GetAllInstances<IAlgorithm>().ToList();
			_selectedAlgorithm = _algorithms.FirstOrDefault();
			_recentAlgorithmsUsed = RecentAlgorithms.Read();
		}

		public DelegateCommand<MassSpecStudio.Core.Domain.Algorithm> LoadParameters { get; set; }

		public IList<IAlgorithm> Algorithms
		{
			get { return _algorithms; }
		}

		public IAlgorithm SelectedAlgorithm
		{
			get
			{
				return _selectedAlgorithm;
			}

			set
			{
				_selectedAlgorithm = value;
				NotifyPropertyChanged(() => SelectedAlgorithm);
				NotifyPropertyChanged(() => RecentAlgorithmsUsed);
			}
		}

		public IList<MassSpecStudio.Core.Domain.Algorithm> RecentAlgorithmsUsed
		{
			get
			{
				return _recentAlgorithmsUsed.Where(item => item.Name == SelectedAlgorithm.Name).ToList();
			}
		}

		private void OnLoadParameters(MassSpecStudio.Core.Domain.Algorithm algorithm)
		{
			SelectedAlgorithm.SetParameters(algorithm);
		}
	}
}
