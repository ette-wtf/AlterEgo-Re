using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Builders
{
	public class SequentialStrategy : ICombiningStrategy
	{
		public IEnumerable<ITestCaseData> GetTestCases(IEnumerable[] sources)
		{
			List<ITestCaseData> list = new List<ITestCaseData>();
			IEnumerator[] array = new IEnumerator[sources.Length];
			for (int i = 0; i < sources.Length; i++)
			{
				array[i] = sources[i].GetEnumerator();
			}
			while (true)
			{
				bool flag = true;
				bool flag2 = false;
				object[] array2 = new object[sources.Length];
				for (int i = 0; i < sources.Length; i++)
				{
					if (array[i].MoveNext())
					{
						array2[i] = array[i].Current;
						flag2 = true;
					}
					else
					{
						array2[i] = null;
					}
				}
				if (!flag2)
				{
					break;
				}
				TestCaseParameters item = new TestCaseParameters(array2);
				list.Add(item);
			}
			return list;
		}
	}
}
