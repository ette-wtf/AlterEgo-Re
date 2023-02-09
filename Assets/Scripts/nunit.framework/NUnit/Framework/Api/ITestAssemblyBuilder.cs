using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Api
{
	public interface ITestAssemblyBuilder
	{
		ITest Build(Assembly assembly, IDictionary<string, object> options);

		ITest Build(string assemblyName, IDictionary<string, object> options);
	}
}
