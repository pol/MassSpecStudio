using System;
using System.Collections.Generic;
using MassSpecStudio.Core.Domain;

namespace MassSpecStudio.Core
{
	public interface IDataProvider
	{
		/// <summary>
		/// Gets the unique GUID for this data provider.  This property should be given a hard coded GUID that will uniquely identify it.
		/// </summary>
		Guid TypeId { get; }

		/// <summary>
		/// Gets the friendly name of this data provider.  This name can be used within the UI.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets the description of this data provider.  This description can be made visible within the UI.
		/// </summary>
		string Description { get; }

		/// <summary>
		/// Gets the supported file types this data provider supports.  This property can be used within the UI for filtering the types of files that are displayed when selecting data files for inclusion in an experiment.
		/// </summary>
		string[] FileTypes { get; }

		/// <summary>
		/// Opens a single sample specified by the sample index from the data file specified in the file name.
		/// </summary>
		/// <param name="fileName">The full path file name of the data file.</param>
		/// <param name="sampleIndex">The index of the sample inside the data file to open.</param>
		void Open(string fileName, int sampleIndex);

		/// <summary>
		/// Closes the currently opened data file.
		/// </summary>
		void Close();

		/// <summary>
		/// Gets a list of sample names that are available within the currently opened data file.
		/// </summary>
		/// <returns>Returns a list of sample names available in the currently opened data file.</returns>
		IList<string> GetSampleNames();

		/// <summary>
		/// Gets a mass spectrum from the currently opened sample within the specified parameters.  This method will return either a single spectrum or an averaged spectrum depending on the start RT and end RT.
		/// </summary>
		/// <param name="startRt">The start retention time.</param>
		/// <param name="stopRt">The end retention time.</param>
		/// <param name="mzLower">The lower M/Z used to filter the spectral data.</param>
		/// <param name="mzUpper">The upper M/z used to filter the spectral data.</param>
		/// <param name="threshold">The intensity threshold.</param>
		/// <returns>Returns a spectrum from the currently opened data file within the parameters specified.</returns>
		IXYData GetSpectrum(double startRt, double stopRt, double mzLower, double mzUpper, double threshold);

		/// <summary>
		/// Gets an MSMS spectrum from the currently opened sample within the specficied parameters.
		/// </summary>
		/// <param name="targetRetentionTime">The target retention time.</param>
		/// <param name="retentionTimeWidth">The width window around the target retention time.</param>
		/// <param name="mz">The MZ (mass over charge)</param>
		/// <param name="mzTolerance">The MZ tolernance.</param>
		/// <returns>Returns the MSMS spectrum that is within the specified parameters.</returns>
		IXYData GetMsMsSpectrum(double targetRetentionTime, double retentionTimeWidth, double mz, double mzTolerance);

		/// <summary>
		/// Gets the XIC from the currently opened sample within the specified parameters.
		/// </summary>
		/// <param name="mass1">The first mass.</param>
		/// <param name="mass2">The second mass.</param>
		/// <param name="mzTolerance">The MZ tolerance.</param>
		/// <param name="timeUnits">The time units for the XIC.</param>
		/// <returns>Returns the XIC for the specified parameters.</returns>
		IXYData GetExtractedIonChromatogram(double mass1, double mass2, double mzTolerance, MassSpecStudio.Core.TimeUnit timeUnits);

		/// <summary>
		/// Gets the TIC from the currently opened sample with the specified time units.
		/// </summary>
		/// <param name="timeUnits">The time units for the TIC.</param>
		/// <returns>Returns the TIC for the specified parameter.</returns>
		IXYData GetTotalIonChromatogram(TimeUnit timeUnits);

		/// <summary>
		/// Checks whether the specified file matches the expected file type.  This method is used to filter the list of available data files when the user is adding data to an experiment.
		/// </summary>
		/// <param name="filePath">The full file path to the data file.</param>
		/// <param name="expectedFileType">The expected file type.  This must be one of the strings from the FileTypes property.</param>
		/// <returns>Returns whether the specified file is of the expected file type.</returns>
		bool IsCorrectFileType(string filePath, string expectedFileType);
	}
}
