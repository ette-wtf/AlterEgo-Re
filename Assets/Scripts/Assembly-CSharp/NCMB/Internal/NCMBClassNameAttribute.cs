using System;

namespace NCMB.Internal
{
	[AttributeUsage(AttributeTargets.All)]
	internal class NCMBClassNameAttribute : Attribute
	{
		public string ClassName { get; private set; }

		internal NCMBClassNameAttribute(string className)
		{
			ClassName = className;
		}
	}
}
