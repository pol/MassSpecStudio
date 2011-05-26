using System;
using System.IO;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace MassSpecStudio.Core.Validators
{
	public class DirectoryDoesNotExistValidator : ValueValidator<string>
	{
		private ValueAccess valueAccess;

		/// <summary>
		/// Initializes a new instance of the DirectoryDoesNotExistValidator class.
		/// </summary>
		/// <param name="valueAccess">The root path value.</param>
		public DirectoryDoesNotExistValidator(ValueAccess valueAccess)
			: this(valueAccess, false)
		{
		}

		/// <summary>
		/// Initializes a new instance of the DirectoryDoesNotExistValidator class.
		/// </summary>
		/// <param name="valueAccess">The root path value.</param>
		/// <param name="negated">True if the validator must negate the result of the validation.</param>
		public DirectoryDoesNotExistValidator(ValueAccess valueAccess, bool negated)
			: this(valueAccess, negated, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the DirectoryDoesNotExistValidator class.
		/// </summary>
		/// <param name="valueAccess">The root path value.</param>
		/// <param name="messageTemplate">The message template to log failures.</param>
		public DirectoryDoesNotExistValidator(ValueAccess valueAccess, string messageTemplate)
			: this(valueAccess, false, messageTemplate)
		{
		}

		/// <summary>
		/// Initializes a new instance of the DirectoryDoesNotExistValidator class.
		/// </summary>
		/// <param name="valueAccess">The root path value.</param>
		/// <param name="negated">True if the validator must negate the result of the validation.</param>
		/// <param name="messageTemplate">The message template to log failures.</param>
		public DirectoryDoesNotExistValidator(ValueAccess valueAccess, bool negated, string messageTemplate)
			: base(messageTemplate, null, negated)
		{
			if (valueAccess == null)
			{
				throw new ArgumentNullException("valueAccess");
			}

			this.valueAccess = valueAccess;
		}

		/// <summary>
		/// Gets the Default Message Template when the validator is non negated.
		/// </summary>
		protected override string DefaultNonNegatedMessageTemplate
		{
			get { return "This directory already exists.  Choose a different name."; }
		}

		/// <summary>
		/// Gets the Default Message Template when the validator is negated.
		/// </summary>
		protected override string DefaultNegatedMessageTemplate
		{
			get { return "This directory does not exist.  Choose a different name."; }
		}

		/// <summary>
		/// Validates by checking if <paramref name="objectToValidate"/> is <see langword="null"/>.
		/// </summary>
		/// <param name="objectToValidate">The object to validate.</param>
		/// <param name="currentTarget">The object on the behalf of which the validation is performed.</param>
		/// <param name="key">The key that identifies the source of <paramref name="objectToValidate"/>.</param>
		/// <param name="validationResults">The validation results to which the outcome of the validation should be stored.</param>
		protected override void DoValidate(string objectToValidate, object currentTarget, string key, ValidationResults validationResults)
		{
			object rootPath;
			string valueAccessFailureMessage;
			if (valueAccess.GetValue(currentTarget, out rootPath, out valueAccessFailureMessage))
			{
				string root = rootPath.ToString();
				string subdirectory = objectToValidate.ToString();
				string directory = Path.Combine(root, subdirectory);
				if (Directory.Exists(directory) == !Negated)
				{
					LogValidationResult(validationResults, GetMessage(objectToValidate, key), currentTarget, key);
				}
			}
		}
	}
}
