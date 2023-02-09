using System;
using System.Globalization;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public class SetUICultureAttribute : PropertyAttribute, IApplyToContext
	{
		private string _culture;

		public SetUICultureAttribute(string culture)
			: base("SetUICulture", culture)
		{
			_culture = culture;
		}

		void IApplyToContext.ApplyToContext(ITestExecutionContext context)
		{
			context.CurrentUICulture = new CultureInfo(_culture);
		}
	}
}
