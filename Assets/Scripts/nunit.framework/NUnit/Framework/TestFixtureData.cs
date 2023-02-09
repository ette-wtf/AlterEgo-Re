using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
	public class TestFixtureData : TestFixtureParameters
	{
		public TestFixtureData(params object[] args)
			: base((args == null) ? new object[1] : args)
		{
		}

		public TestFixtureData(object arg)
			: base(arg)
		{
		}

		public TestFixtureData(object arg1, object arg2)
			: base(arg1, arg2)
		{
		}

		public TestFixtureData(object arg1, object arg2, object arg3)
			: base(arg1, arg2, arg3)
		{
		}

		public TestFixtureData Explicit()
		{
			base.RunState = RunState.Explicit;
			return this;
		}

		public TestFixtureData Explicit(string reason)
		{
			base.RunState = RunState.Explicit;
			base.Properties.Set("_SKIPREASON", reason);
			return this;
		}

		public TestFixtureData Ignore(string reason)
		{
			base.RunState = RunState.Ignored;
			base.Properties.Set("_SKIPREASON", reason);
			return this;
		}
	}
}
