using System;
using System.Collections;
using System.Globalization;
using System.Text;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
	internal static class MsgUtils
	{
		private const string ELLIPSIS = "...";

		private static readonly string Fmt_Null;

		private static readonly string Fmt_EmptyString;

		private static readonly string Fmt_EmptyCollection;

		private static readonly string Fmt_String;

		private static readonly string Fmt_Char;

		private static readonly string Fmt_DateTime;

		private static readonly string Fmt_DateTimeOffset;

		private static readonly string Fmt_ValueType;

		private static readonly string Fmt_Default;

		public static ValueFormatter DefaultValueFormatter { get; set; }

		static MsgUtils()
		{
			Fmt_Null = "null";
			Fmt_EmptyString = "<string.Empty>";
			Fmt_EmptyCollection = "<empty>";
			Fmt_String = "\"{0}\"";
			Fmt_Char = "'{0}'";
			Fmt_DateTime = "yyyy-MM-dd HH:mm:ss.fff";
			Fmt_DateTimeOffset = "yyyy-MM-dd HH:mm:ss.fffzzz";
			Fmt_ValueType = "{0}";
			Fmt_Default = "<{0}>";
			DefaultValueFormatter = (object val) => string.Format(Fmt_Default, val);
			AddFormatter((ValueFormatter next) => (object val) => (val is ValueType) ? string.Format(Fmt_ValueType, val) : next(val));
			AddFormatter((ValueFormatter next) => (object val) => (val is DateTime) ? FormatDateTime((DateTime)val) : next(val));
			AddFormatter((ValueFormatter next) => (object val) => (val is DateTimeOffset) ? FormatDateTimeOffset((DateTimeOffset)val) : next(val));
			AddFormatter((ValueFormatter next) => (object val) => (val is decimal) ? FormatDecimal((decimal)val) : next(val));
			AddFormatter((ValueFormatter next) => (object val) => (val is float) ? FormatFloat((float)val) : next(val));
			AddFormatter((ValueFormatter next) => (object val) => (val is double) ? FormatDouble((double)val) : next(val));
			AddFormatter((ValueFormatter next) => (object val) => (val is char) ? string.Format(Fmt_Char, val) : next(val));
			AddFormatter((ValueFormatter next) => (object val) => (val is IEnumerable) ? FormatCollection((IEnumerable)val, 0L, 10) : next(val));
			AddFormatter((ValueFormatter next) => (object val) => (val is string) ? FormatString((string)val) : next(val));
			AddFormatter((ValueFormatter next) => (object val) => val.GetType().IsArray ? FormatArray((Array)val) : next(val));
		}

		public static void AddFormatter(ValueFormatterFactory formatterFactory)
		{
			DefaultValueFormatter = formatterFactory(DefaultValueFormatter);
		}

		public static string FormatValue(object val)
		{
			if (val == null)
			{
				return Fmt_Null;
			}
			ITestExecutionContext currentContext = TestExecutionContext.CurrentContext;
			if (currentContext != null)
			{
				return currentContext.CurrentValueFormatter(val);
			}
			return DefaultValueFormatter(val);
		}

		public static string FormatCollection(IEnumerable collection, long start, int max)
		{
			int num = 0;
			int num2 = 0;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (object item in collection)
			{
				if (num2++ >= start)
				{
					if (++num > max)
					{
						break;
					}
					stringBuilder.Append((num == 1) ? "< " : ", ");
					stringBuilder.Append(FormatValue(item));
				}
			}
			if (num == 0)
			{
				return Fmt_EmptyCollection;
			}
			if (num > max)
			{
				stringBuilder.Append("...");
			}
			stringBuilder.Append(" >");
			return stringBuilder.ToString();
		}

		private static string FormatArray(Array array)
		{
			if (array.Length == 0)
			{
				return Fmt_EmptyCollection;
			}
			int rank = array.Rank;
			int[] array2 = new int[rank];
			int num = 1;
			int num2 = rank;
			while (--num2 >= 0)
			{
				num = (array2[num2] = num * array.GetLength(num2));
			}
			int num3 = 0;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (object item in array)
			{
				if (num3 > 0)
				{
					stringBuilder.Append(", ");
				}
				bool flag = false;
				for (num2 = 0; num2 < rank; num2++)
				{
					flag = flag || num3 % array2[num2] == 0;
					if (flag)
					{
						stringBuilder.Append("< ");
					}
				}
				stringBuilder.Append(FormatValue(item));
				num3++;
				bool flag2 = false;
				for (num2 = 0; num2 < rank; num2++)
				{
					flag2 = flag2 || num3 % array2[num2] == 0;
					if (flag2)
					{
						stringBuilder.Append(" >");
					}
				}
			}
			return stringBuilder.ToString();
		}

		private static string FormatString(string s)
		{
			return (s == string.Empty) ? Fmt_EmptyString : string.Format(Fmt_String, s);
		}

		private static string FormatDouble(double d)
		{
			if (double.IsNaN(d) || double.IsInfinity(d))
			{
				return d.ToString();
			}
			string text = d.ToString("G17", CultureInfo.InvariantCulture);
			if (text.IndexOf('.') > 0)
			{
				return text + "d";
			}
			return text + ".0d";
		}

		private static string FormatFloat(float f)
		{
			if (float.IsNaN(f) || float.IsInfinity(f))
			{
				return f.ToString();
			}
			string text = f.ToString("G9", CultureInfo.InvariantCulture);
			if (text.IndexOf('.') > 0)
			{
				return text + "f";
			}
			return text + ".0f";
		}

		private static string FormatDecimal(decimal d)
		{
			return d.ToString("G29", CultureInfo.InvariantCulture) + "m";
		}

		private static string FormatDateTime(DateTime dt)
		{
			return dt.ToString(Fmt_DateTime, CultureInfo.InvariantCulture);
		}

		private static string FormatDateTimeOffset(DateTimeOffset dto)
		{
			return dto.ToString(Fmt_DateTimeOffset, CultureInfo.InvariantCulture);
		}

		public static string GetTypeRepresentation(object obj)
		{
			Array array = obj as Array;
			if (array == null)
			{
				return string.Format("<{0}>", obj.GetType());
			}
			StringBuilder stringBuilder = new StringBuilder();
			Type type = array.GetType();
			int num = 0;
			while (type.IsArray)
			{
				type = type.GetElementType();
				num++;
			}
			stringBuilder.Append(type.ToString());
			stringBuilder.Append('[');
			for (int i = 0; i < array.Rank; i++)
			{
				if (i > 0)
				{
					stringBuilder.Append(',');
				}
				stringBuilder.Append(array.GetLength(i));
			}
			stringBuilder.Append(']');
			while (--num > 0)
			{
				stringBuilder.Append("[]");
			}
			return string.Format("<{0}>", stringBuilder.ToString());
		}

		public static string EscapeControlChars(string s)
		{
			if (s != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				string text = s;
				foreach (char c in text)
				{
					switch (c)
					{
					case '\\':
						stringBuilder.Append("\\\\");
						break;
					case '\0':
						stringBuilder.Append("\\0");
						break;
					case '\a':
						stringBuilder.Append("\\a");
						break;
					case '\b':
						stringBuilder.Append("\\b");
						break;
					case '\f':
						stringBuilder.Append("\\f");
						break;
					case '\n':
						stringBuilder.Append("\\n");
						break;
					case '\r':
						stringBuilder.Append("\\r");
						break;
					case '\t':
						stringBuilder.Append("\\t");
						break;
					case '\v':
						stringBuilder.Append("\\v");
						break;
					case '\u0085':
					case '\u2028':
					case '\u2029':
						stringBuilder.Append(string.Format("\\x{0:X4}", (int)c));
						break;
					default:
						stringBuilder.Append(c);
						break;
					}
				}
				s = stringBuilder.ToString();
			}
			return s;
		}

		public static string EscapeNullCharacters(string s)
		{
			if (s != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				string text = s;
				foreach (char c in text)
				{
					if (c == '\0')
					{
						stringBuilder.Append("\\0");
					}
					else
					{
						stringBuilder.Append(c);
					}
				}
				s = stringBuilder.ToString();
			}
			return s;
		}

		public static string GetArrayIndicesAsString(int[] indices)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append('[');
			for (int i = 0; i < indices.Length; i++)
			{
				if (i > 0)
				{
					stringBuilder.Append(',');
				}
				stringBuilder.Append(indices[i].ToString());
			}
			stringBuilder.Append(']');
			return stringBuilder.ToString();
		}

		public static int[] GetArrayIndicesFromCollectionIndex(IEnumerable collection, long index)
		{
			Array array = collection as Array;
			int num = ((array == null) ? 1 : array.Rank);
			int[] array2 = new int[num];
			int num2 = num;
			while (--num2 > 0)
			{
				int length = array.GetLength(num2);
				array2[num2] = (int)index % length;
				index /= length;
			}
			array2[0] = (int)index;
			return array2;
		}

		public static string ClipString(string s, int maxStringLength, int clipStart)
		{
			int num = maxStringLength;
			StringBuilder stringBuilder = new StringBuilder();
			if (clipStart > 0)
			{
				num -= "...".Length;
				stringBuilder.Append("...");
			}
			if (s.Length - clipStart > num)
			{
				num -= "...".Length;
				stringBuilder.Append(s.Substring(clipStart, num));
				stringBuilder.Append("...");
			}
			else if (clipStart > 0)
			{
				stringBuilder.Append(s.Substring(clipStart));
			}
			else
			{
				stringBuilder.Append(s);
			}
			return stringBuilder.ToString();
		}

		public static void ClipExpectedAndActual(ref string expected, ref string actual, int maxDisplayLength, int mismatch)
		{
			int num = Math.Max(expected.Length, actual.Length);
			if (num > maxDisplayLength)
			{
				int num2 = maxDisplayLength - "...".Length;
				int num3 = num - num2;
				if (num3 > mismatch)
				{
					num3 = Math.Max(0, mismatch - num2 / 2);
				}
				expected = ClipString(expected, maxDisplayLength, num3);
				actual = ClipString(actual, maxDisplayLength, num3);
			}
		}

		public static int FindMismatchPosition(string expected, string actual, int istart, bool ignoreCase)
		{
			int num = Math.Min(expected.Length, actual.Length);
			string text = (ignoreCase ? expected.ToLower() : expected);
			string text2 = (ignoreCase ? actual.ToLower() : actual);
			for (int i = istart; i < num; i++)
			{
				if (text[i] != text2[i])
				{
					return i;
				}
			}
			if (expected.Length != actual.Length)
			{
				return num;
			}
			return -1;
		}
	}
}
