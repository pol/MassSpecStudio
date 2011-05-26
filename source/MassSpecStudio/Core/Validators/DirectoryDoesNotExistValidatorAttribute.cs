using System;
using System.Globalization;
using System.Reflection;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace MassSpecStudio.Core.Validators
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1019", Justification = "Fields are used internally")]
	[AttributeUsage(AttributeTargets.Property
		| AttributeTargets.Field
		| AttributeTargets.Method
		| AttributeTargets.Parameter,
		AllowMultiple = true,
		Inherited = false)]
	public sealed class DirectoryDoesNotExistValidatorAttribute : ValueValidatorAttribute
	{
		private string propertyOfRootPath;

		/// <summary>
		/// Initializes a new instance of the DirectoryDoesNotExistValidatorAttribute class.
		/// </summary>
		/// <param name="propertyOfRootPath">The name of the property to use when comparing a value.</param>
		public DirectoryDoesNotExistValidatorAttribute(string propertyOfRootPath)
		{
			if (propertyOfRootPath == null)
			{
				throw new ArgumentNullException("propertyToCompare");
			}

			this.propertyOfRootPath = propertyOfRootPath;
		}
		
		protected override Validator DoCreateValidator(Type targetType, Type ownerType, MemberValueAccessBuilder memberValueAccessBuilder)
		{
			PropertyInfo propertyInfo = ownerType.GetProperty(propertyOfRootPath, BindingFlags.Public | BindingFlags.Instance);
			if (propertyInfo == null)
			{
				throw new InvalidOperationException(
					string.Format(
						CultureInfo.CurrentCulture,
						"Could not find the property specified for the rootPath",
						this.propertyOfRootPath,
						ownerType.FullName));
			}

			return new DirectoryDoesNotExistValidator(memberValueAccessBuilder.GetPropertyValueAccess(propertyInfo), Negated);
		}

		/// <summary>
		/// Creates the <see cref="Validator"/> described by the attribute object providing validator specific
		/// information.
		/// </summary>
		/// <param name="targetType">The type of object that will be validated by the validator.</param>
		/// <remarks>This method must not be called on this class. Call 
		/// <see cref="PropertyComparisonValidatorAttribute.DoCreateValidator(Type, Type, MemberValueAccessBuilder, ValidatorFactory)"/>.</remarks>
		/// <returns>The validator.</returns>
		protected override Validator DoCreateValidator(Type targetType)
		{
			throw new NotImplementedException("Should not be called");
		}
	}
}
