using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSStudio.Composition.Service
{

	/// <summary>
	/// Enumeration of the possible module statuses
	/// </summary>
	public enum ModuleStatus
	{
		/// <summary>
		/// The module has been loaded and is activated
		/// </summary>
		Loaded,

		/// <summary>
		/// The module was returned by the module enumerator, but is currently not loaded
		/// </summary>
		Unloaded
	}

	/// <summary>
	/// Contains status information of a specific module
	/// </summary>
	public struct ModuleStatusInfo
	{
		private string _moduleName;
		private ModuleStatus _status;

		/// <summary>
		/// Initializes a new instance of the ModuleStatusInfo structure.
		/// </summary>
		public ModuleStatusInfo(string moduleName, ModuleStatus status)
		{
			_moduleName = moduleName;
			_status = status;
		}

		/// <summary>
		/// Gets the name of the module
		/// </summary>
		public string ModuleName
		{
			get { return _moduleName; }
		}

		/// <summary>
		/// Gets the status of the module
		/// </summary>
		public ModuleStatus Status
		{
			get { return _status; }
		}

		/// <summary>
		/// Checks if two instances of <see cref="ModuleStatusInfo"/> are not the same
		/// </summary>
		/// <param name="first"></param>
		/// <param name="second"></param>
		/// <returns></returns>
		public static bool operator ==(ModuleStatusInfo first, ModuleStatusInfo second)
		{
			return first.Equals(second);
		}

		/// <summary>
		/// Checks if two instances of <see cref="ModuleStatusInfo"/> are not the same
		/// </summary>
		/// <param name="first"></param>
		/// <param name="second"></param>
		/// <returns></returns>
		public static bool operator !=(ModuleStatusInfo first, ModuleStatusInfo second)
		{
			return !first.Equals(second);
		}

		/// <summary>
		/// Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <param name="obj">Another object to compare to.</param>
		/// <returns>
		/// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
		/// </returns>
		public override bool Equals(object obj)
		{
			if (obj is ModuleStatusInfo)
			{
				return ((ModuleStatusInfo)obj).ModuleName.Equals(_moduleName);
			}

			return false;
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>
		/// A 32-bit signed integer that is the hash code for this instance.
		/// </returns>
		public override int GetHashCode()
		{
			return _moduleName.GetHashCode();
		}
	}
}
