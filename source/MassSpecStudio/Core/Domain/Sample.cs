using System;
using System.Runtime.Serialization;

namespace MassSpecStudio.Core.Domain
{
	[DataContract]
	public class Sample
	{
		private Guid dataProviderTypeId;
		private string _fileName;
		private IDataProvider _dataProvider;

		public Sample(string fileName, string fullPath, IDataProvider dataProvider)
		{
			_fileName = fileName;
			FullPath = fullPath;
			_dataProvider = dataProvider;
			dataProviderTypeId = dataProvider.TypeId;
		}

		~Sample()
		{
			if (_dataProvider != null)
			{
				_dataProvider.Close();
			}
		}

		[DataMember]
		public string FileName
		{
			get { return _fileName; }
			set { _fileName = value; }
		}

		[DataMember]
		public Guid DataProviderType
		{
			get { return dataProviderTypeId; }
			set { dataProviderTypeId = value; }
		}

		public string FullPath { get; set; }

		public IXYData GetSpectrum(double startTime, double stopTime, double mzLower, double mzUpper, double threshold)
		{
			return _dataProvider.GetSpectrum(startTime, stopTime, mzLower, mzUpper, threshold);
		}

		public IXYData GetMsMsSpectrum(double targetRetentionTime, double retentionTimeWidth, double mz, double mzWindow)
		{
			return _dataProvider.GetMsMsSpectrum(targetRetentionTime, retentionTimeWidth, mz, mzWindow);
		}

		public IXYData GetExtractedIonChromatogram(double mass1, double mass2, double mzWindow, MassSpecStudio.Core.TimeUnit timeUnits)
		{
			return _dataProvider.GetExtractedIonChromatogram(mass1, mass2, mzWindow, timeUnits);
		}

		public IXYData GetTotalIonChromatogram()
		{
			return _dataProvider.GetTotalIonChromatogram(TimeUnit.Seconds);
		}

		public void SetDataProvider(IDataProvider dataProvider)
		{
			if (_dataProvider != null)
			{
				_dataProvider.Close();
			}
			_dataProvider = dataProvider;
			_dataProvider.Open(FullPath, 0);
		}
	}
}
