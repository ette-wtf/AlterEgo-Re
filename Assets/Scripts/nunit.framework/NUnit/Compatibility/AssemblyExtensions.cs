using System;
using System.Reflection;

namespace NUnit.Compatibility
{
	public static class AssemblyExtensions
	{
		public static T GetCustomAttribute<T>(this Assembly assembly) where T : Attribute
		{
			T[] array = (T[])assembly.GetCustomAttributes(typeof(T), false);
			return (array.Length > 0) ? array[0] : null;
		}
	}
}
