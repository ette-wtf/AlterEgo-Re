using System;

namespace NUnit.Framework
{
	internal static class Guard
	{
		public static void ArgumentNotNull(object value, string name)
		{
			if (value == null)
			{
				throw new ArgumentNullException("Argument " + name + " must not be null", name);
			}
		}

		public static void ArgumentNotNullOrEmpty(string value, string name)
		{
			ArgumentNotNull(value, name);
			if (value == string.Empty)
			{
				throw new ArgumentException("Argument " + name + " must not be the empty string", name);
			}
		}

		public static void ArgumentInRange(bool condition, string message, string paramName)
		{
			if (!condition)
			{
				throw new ArgumentOutOfRangeException(paramName, message);
			}
		}

		public static void ArgumentValid(bool condition, string message, string paramName)
		{
			if (!condition)
			{
				throw new ArgumentException(message, paramName);
			}
		}

		public static void OperationValid(bool condition, string message)
		{
			if (!condition)
			{
				throw new InvalidOperationException(message);
			}
		}
	}
}
