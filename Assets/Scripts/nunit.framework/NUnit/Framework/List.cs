using System.Collections;

namespace NUnit.Framework
{
	public class List
	{
		public static ListMapper Map(ICollection actual)
		{
			return new ListMapper(actual);
		}
	}
}
