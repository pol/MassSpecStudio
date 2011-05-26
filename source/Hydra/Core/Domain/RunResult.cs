using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using MassSpecStudio.Core.Domain;

namespace Hydra.Core.Domain
{
	[DataContract]
	public class RunResult
	{
		private Run run;
		private Peptide peptide;
		private FragmentIon fragmentIon;
		private bool isResultBasedOnFragment;
		private IList<MSPeak> isotopticPeakList;
		private IList<MSPeak> theoreticalIsotopticPeakList;
		private int actualPeaksInCalculation;
		private double averageMass;
		private double theoreticalAverageMass;
		private double centroidMR;
		private double theoreticalCentroidMR;
		private int amideHydrogenTotal;
		private double amountDeut;
		private double actualXicAdjustment;
		private double actualXicSelectionWidth;
		private IXYData cachedXic;
		private IXYData cachedSpectrum;
		private ChromatographicPeak cachedXicPeak;
		private IList<ChromatographicPeak> cachedXicPeakList;
		private IList<MSPeak> cachedMSPeakList;
		private int actualDeutDistRightPadding;

		public RunResult()
		{
			InitializePeakLists();
		}

		public RunResult(Peptide peptide, Run run)
			: this()
		{
			this.peptide = peptide;
			this.run = run;

			this.ActualPeaksInCalculation = peptide.PeaksInCalculation;
			this.ActualDeutDistThreshold = peptide.DeuteriumDistributionThreshold;
			this.ActualDeutDistRightPadding = peptide.DeuteriumDistributionRightPadding;

			this.ActualXicAdjustment = peptide.XicAdjustment;
			this.ActualXicSelectionWidth = peptide.XicSelectionWidth;

			ActualPeaksInCalculation = peptide.PeaksInCalculation;
		}

		public RunResult(FragmentIon fragmentIon, Run run)
			: this()
		{
			this.fragmentIon = fragmentIon;
			this.peptide = fragmentIon.Peptide;
			this.run = run;

			this.ActualPeaksInCalculation = fragmentIon.PeaksInCalculation;
			this.ActualDeutDistThreshold = fragmentIon.DeutDistThreshold;
			this.ActualDeutDistRightPadding = fragmentIon.DeutDistRightPadding;

			this.IsResultBasedOnFragment = true;
		}

		[DataMember]
		public double AmountDeutFromDeutDist { get; set; }

		[DataMember]
		public Run Run
		{
			get { return run; }
			internal set { run = value; }
		}

		[DataMember]
		public Peptide Peptide
		{
			get { return peptide; }
			internal set { peptide = value; }
		}

		[DataMember]
		public double ActualXicAdjustment
		{
			get { return actualXicAdjustment; }
			set { actualXicAdjustment = value; }
		}

		[DataMember]
		public double ActualXicSelectionWidth
		{
			get { return actualXicSelectionWidth; }
			set { actualXicSelectionWidth = value; }
		}

		public IXYData CachedXic
		{
			get { return cachedXic; }
			set { cachedXic = value; }
		}

		public IXYData CachedSpectrum
		{
			get { return cachedSpectrum; }
			set { cachedSpectrum = value; }
		}

		[DataMember]
		public XYData PersistedCachedXic
		{
			get
			{
				if (cachedXic != null)
				{
					XYData xyData = new XYData(cachedXic.XValues, cachedXic.YValues);
					xyData.Title = cachedXic.Title;
					return xyData;
				}
				return null;
			}

			set
			{
				cachedXic = value;
			}
		}

		[DataMember]
		public XYData PersistedCachedSpectrum
		{
			get
			{
				if (cachedSpectrum != null)
				{
					XYData xyData = new XYData(cachedSpectrum.XValues, cachedSpectrum.YValues);
					xyData.Title = cachedXic.Title;
					return xyData;
				}
				return null;
			}

			set
			{
				cachedSpectrum = value;
			}
		}

		[DataMember]
		public ChromatographicPeak CachedXicPeak
		{
			get
			{
				if (cachedXicPeak != null && CachedXicPeakList != null)
				{
					return CachedXicPeakList.Where(item => item.Rt == cachedXicPeak.Rt && item.PeakHeight == cachedXicPeak.PeakHeight).FirstOrDefault();
				}
				return cachedXicPeak;
			}

			set
			{
				cachedXicPeak = value;
			}
		}

		[DataMember]
		public IList<ChromatographicPeak> CachedXicPeakList
		{
			get { return cachedXicPeakList; }
			set { cachedXicPeakList = value; }
		}

		[DataMember]
		public IList<MSPeak> CachedMSPeakList
		{
			get { return cachedMSPeakList; }
			set { cachedMSPeakList = value; }
		}

		[DataMember]
		public FragmentIon FragmentIon
		{
			get { return fragmentIon; }
			set { fragmentIon = value; }
		}

		[DataMember]
		public double ActualDeutDistThreshold { get; set; }

		[DataMember]
		public bool IsResultBasedOnFragment
		{
			get { return isResultBasedOnFragment; }
			set { isResultBasedOnFragment = value; }
		}

		[DataMember]
		public IList<MSPeak> IsotopicPeakList
		{
			get { return isotopticPeakList; }
			set { isotopticPeakList = value; }
		}

		[DataMember]
		public IList<MSPeak> TheoreticalIsotopicPeakList
		{
			get { return theoreticalIsotopticPeakList; }
			set { theoreticalIsotopticPeakList = value; }
		}

		[DataMember]
		public int ActualPeaksInCalculation
		{
			get { return actualPeaksInCalculation; }
			set { actualPeaksInCalculation = value; }
		}

		[DataMember]
		public double AverageMass
		{
			get { return averageMass; }
			set { averageMass = value; }
		}

		[DataMember]
		public double TheoreticalAverageMass
		{
			get { return theoreticalAverageMass; }
			set { theoreticalAverageMass = value; }
		}

		[DataMember]
		public double CentroidMR
		{
			get { return centroidMR; }
			set { centroidMR = value; }
		}

		[DataMember]
		public double TheoreticalCentroidMR
		{
			get { return theoreticalCentroidMR; }
			set { theoreticalCentroidMR = value; }
		}

		[DataMember]
		public int AmideHydrogenTotal
		{
			get { return amideHydrogenTotal; }
			set { amideHydrogenTotal = value; }
		}

		[DataMember]
		public double AmountDeut
		{
			get { return amountDeut; }
			set { amountDeut = value; }
		}

		[DataMember]
		public int ActualDeutDistRightPadding
		{
			get { return actualDeutDistRightPadding; }
			set { actualDeutDistRightPadding = value; }
		}

		[DataMember]
		public bool IsUsedInCalculations { get; set; }

		public void InitializePeakLists()
		{
			isotopticPeakList = new List<MSPeak>();
			theoreticalIsotopticPeakList = new List<MSPeak>();
		}

		public string ToString(string delimiter)
		{
			string result = string.Empty;

			result += Peptide.Sequence + delimiter;
			result += Peptide.AminoAcidStart + delimiter;
			result += Peptide.AminoAcidStop + delimiter;
			result += Run.ProteinState.Name + delimiter;
			result += Run.Labeling.Name + delimiter;
			result += AmountDeut + delimiter;
			result += AverageMass + delimiter;
			result += TheoreticalAverageMass + delimiter;
			result += CentroidMR + delimiter;
			result += TheoreticalCentroidMR + delimiter;

			result += Environment.NewLine;
			return result;
		}

		public RunResult Clone()
		{
			RunResult clonedRunResult = new RunResult();
			clonedRunResult.AmountDeutFromDeutDist = AmountDeutFromDeutDist;
			clonedRunResult.Run = Run;
			clonedRunResult.Peptide = Peptide;
			clonedRunResult.ActualXicAdjustment = ActualXicAdjustment;
			clonedRunResult.ActualXicSelectionWidth = ActualXicSelectionWidth;
			clonedRunResult.CachedXic = CachedXic;
			clonedRunResult.CachedMSPeakList = CachedMSPeakList;
			clonedRunResult.CachedXicPeakList = CachedXicPeakList;
			clonedRunResult.CachedSpectrum = CachedSpectrum;
			clonedRunResult.CachedXicPeak = CachedXicPeak;
			clonedRunResult.FragmentIon = FragmentIon;
			clonedRunResult.ActualDeutDistThreshold = ActualDeutDistThreshold;
			clonedRunResult.IsResultBasedOnFragment = IsResultBasedOnFragment;
			clonedRunResult.IsotopicPeakList = IsotopicPeakList;
			clonedRunResult.TheoreticalIsotopicPeakList = TheoreticalIsotopicPeakList;
			clonedRunResult.ActualPeaksInCalculation = ActualPeaksInCalculation;
			clonedRunResult.AverageMass = AverageMass;
			clonedRunResult.TheoreticalAverageMass = TheoreticalAverageMass;
			clonedRunResult.CentroidMR = CentroidMR;
			clonedRunResult.TheoreticalCentroidMR = TheoreticalCentroidMR;
			clonedRunResult.AmideHydrogenTotal = AmideHydrogenTotal;
			clonedRunResult.AmountDeut = AmountDeut;
			clonedRunResult.IsUsedInCalculations = IsUsedInCalculations;
			return clonedRunResult;
		}
	}
}
