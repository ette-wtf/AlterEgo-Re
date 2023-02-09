using System;
using System.Globalization;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public class SetCultureAttribute : PropertyAttribute, IApplyToContext
	{
		private string _culture;

		public SetCultureAttribute(string culture)
			: base("SetCulture", culture)
		{
			_culture = culture;
		}

		void IApplyToContext.ApplyToContext(ITestExecutionContext context)
		{
			context.CurrentCulture = new CultureInfo(_culture);
		}
	}
}
