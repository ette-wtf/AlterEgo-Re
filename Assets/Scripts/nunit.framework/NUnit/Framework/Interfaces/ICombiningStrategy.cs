using System.Collections;
using System.Collections.Generic;

namespace NUnit.Framework.Interfaces
{
	public interface ICombiningStrategy
	{
		IEnumerable<ITestCaseData> GetTestCases(IEnumerable[] sources);
	}
}
