using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Builders
{
	public class CombinatorialStrategy : ICombiningStrategy
	{
		public IEnumerable<ITestCaseData> GetTestCases(IEnumerable[] sources)
		{
			List<ITestCaseData> list = new List<ITestCaseData>();
			IEnumerator[] array = new IEnumerator[sources.Length];
			int num = -1;
			do
			{
				bool flag = true;
				while (++num < sources.Length)
				{
					array[num] = sources[num].GetEnumerator();
					if (!array[num].MoveNext())
					{
						return list;
					}
				}
				object[] array2 = new object[sources.Length];
				for (int i = 0; i < sources.Length; i++)
				{
					array2[i] = array[i].Current;
				}
				TestCaseParameters item = new TestCaseParameters(array2);
				list.Add(item);
				num = sources.Length;
				while (--num >= 0 && !array[num].MoveNext())
				{
				}
			}
			while (num >= 0);
			return list;
		}
	}
}
