using System;

namespace NUnit.Framework
{
	[Flags]
	public enum ParallelScope
	{
		None = 0,
		Self = 1,
		Children = 2,
		Fixtures = 4
	}
}
