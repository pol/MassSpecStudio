using System;
using System.Collections.Generic;
using System.ComponentModel;
using Hydra.Core.Domain;
using Hydra.Core.Provider;
using MassSpecStudio.Core;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;

namespace Hydra.Modules.Project.Views
{
	public class PeptidesViewModel : TreeViewItemBase<ExperimentViewModel, PeptideViewModel>
	{
		private IServiceLocator serviceLocator;
		private Experiment experiment;

		public PeptidesViewModel(Experiment experiment)
		{
			try
			{
				serviceLocator = ServiceLocator.Current;
			}
			catch (NullReferenceException)
			{
			}

			this.experiment = experiment;
			Data = experiment.Peptides;

			foreach (Peptide peptide in experiment.Peptides.PeptideCollection)
			{
				PeptideViewModel peptideViewModel = new PeptideViewModel(peptide);
				peptideViewModel.Parent = this;
				Children.Add(peptideViewModel);
			}

			AddFromFile = new DelegateCommand<PeptidesViewModel>(OnAddFromFile);
			AddPeptide = new DelegateCommand<PeptidesViewModel>(OnAddPeptide);
			Remove = new DelegateCommand<PeptideViewModel>(OnRemove);
		}

		public PeptidesViewModel(Experiment experiment, IServiceLocator serviceLocator)
			: this(experiment)
		{
			this.serviceLocator = serviceLocator;
		}

		[Category("Common Information")]
		[ReadOnly(true)]
		public string Location
		{
			get { return experiment.Peptides.Location; }
		}

		[Browsable(false)]
		public DelegateCommand<PeptidesViewModel> AddFromFile { get; set; }

		[Browsable(false)]
		public DelegateCommand<PeptidesViewModel> AddPeptide { get; set; }

		[Browsable(false)]
		public DelegateCommand<PeptideViewModel> Remove { get; set; }

		public override string Name
		{
			get
			{
				return "peptides";
			}

			set
			{
			}
		}

		public void AddPeptides(string path)
		{
			IPeptideDataProvider dataProvider = serviceLocator.GetInstance<IPeptideDataProvider>();
			IList<Peptide> peptides = dataProvider.Read(path);

			foreach (Peptide peptide in peptides)
			{
				experiment.Peptides.PeptideCollection.Add(peptide);
				PeptideViewModel peptideViewModel = new PeptideViewModel(peptide);
				peptideViewModel.Parent = this;
				Children.Add(peptideViewModel);
			}
			IsExpanded = true;
		}

		private void OnAddFromFile(PeptidesViewModel viewModel)
		{
			System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();

			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				AddPeptides(dialog.FileName);
			}
		}

		private void OnAddPeptide(PeptidesViewModel viewModel)
		{
			Peptide peptide = new Peptide("A");
			experiment.Peptides.PeptideCollection.Add(peptide);
			PeptideViewModel peptideViewModel = new PeptideViewModel(peptide);
			peptideViewModel.Parent = this;
			Children.Add(peptideViewModel);
		}

		private void OnRemove(PeptideViewModel value)
		{
			experiment.Peptides.PeptideCollection.Remove(value.Data as Peptide);
			Children.Remove(value);
		}
	}
}
