using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace MassSpecStudio.Core
{
	public static class PropertySupport
	{
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "There is no other way of having strongly typed property names for notifyproperties.")]
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "There is no other way of having strongly typed property names for notifyproperties.")]
		public static string ExtractPropertyName<T>(Expression<Func<T>> propertyExpression)
		{
			if (propertyExpression == null)
			{
				throw new ArgumentNullException("propertyExpression");
			}

			var memberExpression = propertyExpression.Body as MemberExpression;
			if (memberExpression == null)
			{
				throw new ArgumentException("The expression is not a member access expression.", "propertyExpression");
			}

			var property = memberExpression.Member as PropertyInfo;
			if (property == null)
			{
				throw new ArgumentException("The member access expression does not access a property.", "propertyExpression");
			}

			var getMethod = property.GetGetMethod(true);
			if (getMethod.IsStatic)
			{
				throw new ArgumentException("The referenced property is a static property.", "propertyExpression");
			}

			return memberExpression.Member.Name;
		}
	}
}
