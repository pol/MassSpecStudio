using System;
using System.Collections.Generic;
using MassSpecStudio.Core.Domain;

namespace MassSpecStudio.Core
{
	public interface IDataProvider
	{
		Guid TypeId { get; }

		string Name { get; }

		string Description { get; }

		string[] FileTypes { get; }

		void Open(string fileName, int sampleIndex);

		void Close();

		IList<string> GetSampleNames();

		IXYData GetSpectrum(double startRt, double stopRt, double mzLower, double mzUpper, double threshold);

		IXYData GetMsMsSpectrum(double targetRetentionTime, double retentionTimeWidth, double mz, double mzTolerance);

		IXYData GetExtractedIonChromatogram(double mass1, double mass2, double mzTolerance, MassSpecStudio.Core.TimeUnit timeUnits);

		IXYData GetTotalIonChromatogram(TimeUnit timeUnits);

		bool IsCorrectFileType(string filePath, string expectedFileType);
	}
}
