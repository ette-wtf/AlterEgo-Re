using System.IO;
using System.Reflection;

namespace NUnit.Framework.Internal
{
	public class TestAssembly : TestSuite
	{
		public Assembly Assembly { get; private set; }

		public override string TestType
		{
			get
			{
				return "Assembly";
			}
		}

		public TestAssembly(Assembly assembly, string path)
			: base(path)
		{
			Assembly = assembly;
			base.Name = Path.GetFileName(path);
		}

		public TestAssembly(string path)
			: base(path)
		{
			base.Name = Path.GetFileName(path);
		}
	}
}
