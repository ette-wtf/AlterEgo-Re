using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
	public class TestCaseData : TestCaseParameters
	{
		public TestCaseData(params object[] args)
			: base((args == null) ? new object[1] : args)
		{
		}

		public TestCaseData(object arg)
			: base(new object[1] { arg })
		{
		}

		public TestCaseData(object arg1, object arg2)
			: base(new object[2] { arg1, arg2 })
		{
		}

		public TestCaseData(object arg1, object arg2, object arg3)
			: base(new object[3] { arg1, arg2, arg3 })
		{
		}

		public TestCaseData Returns(object result)
		{
			base.ExpectedResult = result;
			return this;
		}

		public TestCaseData SetName(string name)
		{
			base.TestName = name;
			return this;
		}

		public TestCaseData SetDescription(string description)
		{
			base.Properties.Set("Description", description);
			return this;
		}

		public TestCaseData SetCategory(string category)
		{
			base.Properties.Add("Category", category);
			return this;
		}

		public TestCaseData SetProperty(string propName, string propValue)
		{
			base.Properties.Add(propName, propValue);
			return this;
		}

		public TestCaseData SetProperty(string propName, int propValue)
		{
			base.Properties.Add(propName, propValue);
			return this;
		}

		public TestCaseData SetProperty(string propName, double propValue)
		{
			base.Properties.Add(propName, propValue);
			return this;
		}

		public TestCaseData Explicit()
		{
			base.RunState = RunState.Explicit;
			return this;
		}

		public TestCaseData Explicit(string reason)
		{
			base.RunState = RunState.Explicit;
			base.Properties.Set("_SKIPREASON", reason);
			return this;
		}

		public TestCaseData Ignore(string reason)
		{
			base.RunState = RunState.Ignored;
			base.Properties.Set("_SKIPREASON", reason);
			return this;
		}
	}
}
