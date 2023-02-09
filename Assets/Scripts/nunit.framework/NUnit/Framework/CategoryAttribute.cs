using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
	public class CategoryAttribute : NUnitAttribute, IApplyToTest
	{
		protected string categoryName;

		public string Name
		{
			get
			{
				return categoryName;
			}
		}

		public CategoryAttribute(string name)
		{
			categoryName = name.Trim();
		}

		protected CategoryAttribute()
		{
			categoryName = GetType().Name;
			if (categoryName.EndsWith("Attribute"))
			{
				categoryName = categoryName.Substring(0, categoryName.Length - 9);
			}
		}

		public void ApplyToTest(Test test)
		{
			test.Properties.Add("Category", Name);
			if (Name.IndexOfAny(new char[4] { ',', '!', '+', '-' }) >= 0)
			{
				test.RunState = RunState.NotRunnable;
				test.Properties.Set("_SKIPREASON", "Category name must not contain ',', '!', '+' or '-'");
			}
		}
	}
}
