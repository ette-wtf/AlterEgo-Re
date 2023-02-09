using System;

namespace NCMB.Internal
{
	internal static class ExtensionsClass
	{
		public static Type GetTypeInfo(this Type t)
		{
			return t;
		}

		internal static bool IsPrimitive(this Type type)
		{
			return type.GetTypeInfo().IsPrimitive;
		}
	}
}
