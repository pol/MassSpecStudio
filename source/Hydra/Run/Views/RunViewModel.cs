using System.IO;
using MassSpecStudio.Core;
using MassSpecStudio.Core.Domain;

namespace Hydra.Modules.Run.Views
{
	public class RunViewModel : ViewModelBase
	{
		private Core.Domain.Run _run;
		private IXYData _xyData;
		private double _xicMass;
		private double _mzTolerance;
		private double _startTime;
		private double _stopTime;
		private double _monoIsotopicMass;
		private double _mzLowerOffset;
		private double _mzUpperOffset;

		public RunViewModel(Core.Domain.Run run)
		{
			_run = run;

			LoadTic();
		}

		public RunViewModel(Core.Domain.Run run, double xicMass, double mzTolerance)
		{
			_run = run;
			_mzTolerance = mzTolerance;
			_xicMass = xicMass;

			LoadXic();
		}

		public RunViewModel(Core.Domain.Run run, double startTime, double stopTime, double monoisotopicMass, double mzLowerOffset, double mzUpperOffset)
		{
			_run = run;
			_startTime = startTime;
			_stopTime = stopTime;
			_monoIsotopicMass = monoisotopicMass;
			_mzLowerOffset = mzLowerOffset;
			_mzUpperOffset = mzUpperOffset;

			LoadSpectrum();
		}

		public Core.Domain.Run Run
		{
			get { return _run; }
		}

		public IXYData XYData
		{
			get { return _xyData; }
		}

		private void LoadTic()
		{
			_xyData = _run.GetTotalIonChromatogram();
			_xyData.Title = "TIC - " + Path.GetFileName(_run.FileName);
		}

		private void LoadXic()
		{
			_xyData = _run.GetExtractedIonChromatogram(_xicMass, _xicMass, _mzTolerance, TimeUnit.Seconds);
			_xyData.Title = "XIC (" + _xicMass.ToString("N4") + " mzTol=" + _mzTolerance + ") - " + Path.GetFileName(_run.FileName);
		}

		private void LoadSpectrum()
		{
			_xyData = _run.GetSpectrum(_startTime, _stopTime, _monoIsotopicMass - _mzLowerOffset, _monoIsotopicMass + _mzUpperOffset, 0);
			_xyData.Title = "MS (RT=" + _startTime.ToString("N2") + "-" + _stopTime.ToString("N2") + " Mass=" + _monoIsotopicMass.ToString("N4") + ") - " + Path.GetFileName(_run.FileName);
		}
	}
}
