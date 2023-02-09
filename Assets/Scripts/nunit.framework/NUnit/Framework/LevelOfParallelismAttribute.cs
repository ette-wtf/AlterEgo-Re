using System;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
	public sealed class LevelOfParallelismAttribute : PropertyAttribute
	{
		public LevelOfParallelismAttribute(int level)
			: base(level)
		{
		}
	}
}
