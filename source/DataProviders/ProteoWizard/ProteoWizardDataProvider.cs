using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using MassSpecStudio.Core;
using MassSpecStudio.Core.Domain;
using MassSpecStudio.Core.Events;
using Microsoft.Practices.Prism.Events;
using pwiz.CLI.msdata;

namespace ProteoWizard.MassSpecStudio.DataProvider
{
	[Export(typeof(IDataProvider))]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	[ExportMetadata("Title", "ProteoWizard Data Provider")]
	[ExportMetadata("Type", "Data Provider")]
	[ExportMetadata("Author", "Mass Spec Studio")]
	[ExportMetadata("Description", "This module provides access to .mzxml and .mzml file formats via the ProteoWizard library.")]
	public sealed class ProteoWizardDataProvider : IDataProvider, IDisposable
	{
		private readonly IEventAggregator _eventAggregator;
		private SpectrumCache spectrumCache;
		private OutputEvent _output;
		private MSData _dataFile;
		private TicGenerator ticGenerator;
		private XicGenerator xicGenerator;
		private SpectrumExtractor spectrumExtractor;
		private AveragedSpectrumExtractor averagedSpectrumExtractor;

		private Run _run;
		private string _currentFileName;
		private int _currentSampleIndex;

		[ImportingConstructor]
		public ProteoWizardDataProvider(IEventAggregator eventAggregator)
		{
			_eventAggregator = eventAggregator;
			_output = _eventAggregator.GetEvent<OutputEvent>();
		}

		public Guid TypeId
		{
			get { return new Guid("ea35cd69-b064-46c3-afef-5e99c1433932"); }
		}

		public string Name
		{
			get { return "ProteoWizard Data Provider"; }
		}

		public string Description
		{
			get { return "Provides data access for .mzxml and .mzml files."; }
		}

		public string[] FileTypes
		{
			get
			{
				return new string[9] 
				{
					"Any spectra format",
					"mzML",
					"mzXML",
					"Thermo RAW",
					"Waters RAW",
					"Bruker Analysis",
					"Agilent MassHunter",
					"Mascot Generic",
					"Bruker Data Exchange",
				};
			}
		}

		public bool IsCorrectFileType(string filePath, string expectedFileType)
		{
			string fileType;
			try
			{
				fileType = ReaderList.FullReaderList.identify(filePath);
			}
			catch
			{
				fileType = null;
			}

			return (expectedFileType == FileTypes[0] && !string.IsNullOrEmpty(fileType)) || (expectedFileType == fileType);
		}

		public void Open(string fileName, int sampleIndex)
		{
			if (_currentSampleIndex != sampleIndex || _currentFileName != fileName)
			{
				MSDataList msdList = new MSDataList();
				ReaderList.FullReaderList.read(fileName, msdList);
				_dataFile = msdList[0];
				_run = _dataFile.run;

				_currentFileName = fileName;
				_currentSampleIndex = sampleIndex;

				spectrumCache = new SpectrumCache();
				ticGenerator = new TicGenerator(_run, spectrumCache);
				xicGenerator = new XicGenerator(_run, spectrumCache);
				spectrumExtractor = new SpectrumExtractor(_run, spectrumCache);
				averagedSpectrumExtractor = new AveragedSpectrumExtractor(_run, spectrumCache);
			}
		}

		public void Close()
		{
			if (spectrumCache != null)
			{
				spectrumCache.Clear();
			}
			_currentFileName = null;
			_currentSampleIndex = -1;
			Dispose();
		}

		public IList<string> GetSampleNames()
		{
			IList<string> sampleNames = new List<string>();
			foreach (pwiz.CLI.msdata.Sample sample in _dataFile.samples)
			{
				sampleNames.Add(sample.name);
			}
			return sampleNames;
		}

		public IXYData GetSpectrum(double startRt, double stopRt, double mzLower, double mzUpper, double threshold)
		{
			_output.Publish("     GetSpectrum(startTime=" + startRt + ", stopTime=" + stopRt + ", mzLower=" + mzLower + ", mzUpper=" + mzUpper + ", threshold=" + threshold + ")");

			if (startRt == stopRt)
			{
				return spectrumExtractor.GetSpectrum(startRt, GetTimeUnits(), mzLower, mzUpper);
			}
			return averagedSpectrumExtractor.GetAveragedSpectrum(startRt, stopRt, GetTimeUnits(), mzLower, mzUpper);
		}

		public IXYData GetMsMsSpectrum(double targetRetentionTime, double retentionTimeWidth, double mz, double mzTolerance)
		{
			return averagedSpectrumExtractor.GetMSMSSpectrum(targetRetentionTime, retentionTimeWidth, mz, mzTolerance);
		}

		public IXYData GetTotalIonChromatogram(TimeUnit timeUnits)
		{
			return ticGenerator.Generate(GetTimeUnits());
		}

		public IXYData GetExtractedIonChromatogram(double mass1, double mass2, double mzTolerance, TimeUnit timeUnits)
		{
			_output.Publish("     GetExtractedIonChromatogram (mass1=" + mass1 + ", mass2=" + mass2 + ", mzTolerance=" + mzTolerance + ", timeUnits=" + GetTimeUnits() + ")");
			return xicGenerator.Generate(mass1, mass2, mzTolerance, GetTimeUnits());
		}

		public void Dispose()
		{
			if (_dataFile != null)
			{
				_dataFile.Dispose();
				_dataFile = null;
			}
		}

		private string GetFileType(string filePath)
		{
			return ReaderList.FullReaderList.identify(filePath);
		}

		private TimeUnit GetTimeUnits()
		{
			string fileType = GetFileType(_currentFileName);
			if (fileType == "Thermo RAW")
			{
				return TimeUnit.Minutes;
			}
			return TimeUnit.Seconds;
		}
	}
}