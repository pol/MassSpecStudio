
namespace Hydra.Core
{
	public enum PeaksInLabelCalculationMode
	{
		/// <summary>
		/// Manual peak selection.
		/// </summary>
		Manual,

		/// <summary>
		/// Automatic peak selection.
		/// </summary>
		Automatic
	}

	public enum MSPeakSelectionOption
	{
		/// <summary>
		/// Most intense within mz variation.
		/// </summary>
		MostIntenseWithinMzVariation,

		/// <summary>
		/// Closet to mz within mz variation.
		/// </summary>
		ClosestToMzWithinMzVariation
	}

	public enum FragmentIonType
	{
		/// <summary>
		/// Undefined fragmention type.
		/// </summary>
		Undefined,

		/// <summary>
		/// B fragment.
		/// </summary>
		BFragment,

		/// <summary>
		/// Y fragment.
		/// </summary>
		YFragment,

		/// <summary>
		/// A fragment.
		/// </summary>
		AFragment,

		/// <summary>
		/// B minus H2O fragment.
		/// </summary>
		BMinusH2O,

		/// <summary>
		/// Y minus H2O fragment.
		/// </summary>
		YMinusH2O,

		/// <summary>
		/// B minus NH3 fragment.
		/// </summary>
		BMinusNH3,

		/// <summary>
		/// Y minus NH3 fragment.
		/// </summary>
		YMinusNH3,

		/// <summary>
		/// C fragment.
		/// </summary>
		CFragment,

		/// <summary>
		/// Z fragment.
		/// </summary>
		ZFragment,

		/// <summary>
		/// Parent fragment.
		/// </summary>
		Parent
	}

	public enum XicPeakPickerOption
	{
		/// <summary>
		/// Closest to RT within RT variation.
		/// </summary>
		ClosestToRTWithinRTVariation,

		/// <summary>
		/// Most intense within RT variation.
		/// </summary>
		MostIntenseWithinRtVariation,

		/// <summary>
		/// Most intense within entire XIC.
		/// </summary>
		MostIntenseWithinEntireXic
	}

	public static class Constants
	{
		public const int SignificantDigits = 5;
	}
}
