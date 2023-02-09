using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace NUnit.Framework.Internal
{
	public class TestNameGenerator
	{
		private abstract class NameFragment
		{
			private const string THREE_DOTS = "...";

			public virtual string GetText(TestMethod testMethod, object[] args)
			{
				return GetText(testMethod.Method.MethodInfo, args);
			}

			public abstract string GetText(MethodInfo method, object[] args);

			protected static void AppendGenericTypeNames(StringBuilder sb, MethodInfo method)
			{
				sb.Append("<");
				int num = 0;
				Type[] genericArguments = method.GetGenericArguments();
				foreach (Type type in genericArguments)
				{
					if (num++ > 0)
					{
						sb.Append(",");
					}
					sb.Append(type.Name);
				}
				sb.Append(">");
			}

			protected static string GetDisplayString(object arg, int stringMax)
			{
				string text = ((arg == null) ? "null" : Convert.ToString(arg, CultureInfo.InvariantCulture));
				if (arg is double)
				{
					double num = (double)arg;
					if (double.IsNaN(num))
					{
						text = "double.NaN";
					}
					else if (double.IsPositiveInfinity(num))
					{
						text = "double.PositiveInfinity";
					}
					else if (double.IsNegativeInfinity(num))
					{
						text = "double.NegativeInfinity";
					}
					else if (num == double.MaxValue)
					{
						text = "double.MaxValue";
					}
					else if (num == double.MinValue)
					{
						text = "double.MinValue";
					}
					else
					{
						if (text.IndexOf('.') == -1)
						{
							text += ".0";
						}
						text += "d";
					}
				}
				else if (arg is float)
				{
					float num2 = (float)arg;
					if (float.IsNaN(num2))
					{
						text = "float.NaN";
					}
					else if (float.IsPositiveInfinity(num2))
					{
						text = "float.PositiveInfinity";
					}
					else if (float.IsNegativeInfinity(num2))
					{
						text = "float.NegativeInfinity";
					}
					else if (num2 == float.MaxValue)
					{
						text = "float.MaxValue";
					}
					else if (num2 == float.MinValue)
					{
						text = "float.MinValue";
					}
					else
					{
						if (text.IndexOf('.') == -1)
						{
							text += ".0";
						}
						text += "f";
					}
				}
				else if (arg is decimal)
				{
					decimal num3 = (decimal)arg;
					text = ((num3 == decimal.MinValue) ? "decimal.MinValue" : ((!(num3 == decimal.MaxValue)) ? (text + "m") : "decimal.MaxValue"));
				}
				else if (arg is long)
				{
					text = (arg.Equals(long.MinValue) ? "long.MinValue" : ((!arg.Equals(long.MaxValue)) ? (text + "L") : "long.MaxValue"));
				}
				else if (arg is ulong)
				{
					switch ((ulong)arg)
					{
					case 0uL:
						text = "ulong.MinValue";
						break;
					case ulong.MaxValue:
						text = "ulong.MaxValue";
						break;
					default:
						text += "UL";
						break;
					}
				}
				else if (arg is string)
				{
					string text2 = (string)arg;
					bool flag = stringMax > 0 && text2.Length > stringMax;
					int num4 = (flag ? (stringMax - "...".Length) : 0);
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append("\"");
					string text3 = text2;
					foreach (char c in text3)
					{
						stringBuilder.Append(EscapeCharInString(c));
						if (flag && stringBuilder.Length > num4)
						{
							stringBuilder.Append("...");
							break;
						}
					}
					stringBuilder.Append("\"");
					text = stringBuilder.ToString();
				}
				else if (arg is char)
				{
					text = "'" + EscapeSingleChar((char)arg) + "'";
				}
				else if (arg is int)
				{
					if (arg.Equals(int.MaxValue))
					{
						text = "int.MaxValue";
					}
					else if (arg.Equals(int.MinValue))
					{
						text = "int.MinValue";
					}
				}
				else if (arg is uint)
				{
					if (arg.Equals(uint.MaxValue))
					{
						text = "uint.MaxValue";
					}
					else if (arg.Equals(0u))
					{
						text = "uint.MinValue";
					}
				}
				else if (arg is short)
				{
					if (arg.Equals(short.MaxValue))
					{
						text = "short.MaxValue";
					}
					else if (arg.Equals(short.MinValue))
					{
						text = "short.MinValue";
					}
				}
				else if (arg is ushort)
				{
					if (arg.Equals(ushort.MaxValue))
					{
						text = "ushort.MaxValue";
					}
					else if (arg.Equals((ushort)0))
					{
						text = "ushort.MinValue";
					}
				}
				else if (arg is byte)
				{
					if (arg.Equals(byte.MaxValue))
					{
						text = "byte.MaxValue";
					}
					else if (arg.Equals((byte)0))
					{
						text = "byte.MinValue";
					}
				}
				else if (arg is sbyte)
				{
					if (arg.Equals(sbyte.MaxValue))
					{
						text = "sbyte.MaxValue";
					}
					else if (arg.Equals(sbyte.MinValue))
					{
						text = "sbyte.MinValue";
					}
				}
				return text;
			}

			private static string EscapeSingleChar(char c)
			{
				if (c == '\'')
				{
					return "\\'";
				}
				return EscapeControlChar(c);
			}

			private static string EscapeCharInString(char c)
			{
				if (c == '"')
				{
					return "\\\"";
				}
				return EscapeControlChar(c);
			}

			private static string EscapeControlChar(char c)
			{
				switch (c)
				{
				case '\\':
					return "\\\\";
				case '\0':
					return "\\0";
				case '\a':
					return "\\a";
				case '\b':
					return "\\b";
				case '\f':
					return "\\f";
				case '\n':
					return "\\n";
				case '\r':
					return "\\r";
				case '\t':
					return "\\t";
				case '\v':
					return "\\v";
				case '\u0085':
				case '\u2028':
				case '\u2029':
					return string.Format("\\x{0:X4}", (int)c);
				default:
					return c.ToString();
				}
			}
		}

		private class TestIDFragment : NameFragment
		{
			public override string GetText(MethodInfo method, object[] args)
			{
				return "{i}";
			}

			public override string GetText(TestMethod testMethod, object[] args)
			{
				return testMethod.Id;
			}
		}

		private class FixedTextFragment : NameFragment
		{
			private string _text;

			public FixedTextFragment(string text)
			{
				_text = text;
			}

			public override string GetText(MethodInfo method, object[] args)
			{
				return _text;
			}
		}

		private class MethodNameFragment : NameFragment
		{
			public override string GetText(MethodInfo method, object[] args)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(method.Name);
				if (method.IsGenericMethod)
				{
					NameFragment.AppendGenericTypeNames(stringBuilder, method);
				}
				return stringBuilder.ToString();
			}
		}

		private class NamespaceFragment : NameFragment
		{
			public override string GetText(MethodInfo method, object[] args)
			{
				return method.DeclaringType.Namespace;
			}
		}

		private class MethodFullNameFragment : NameFragment
		{
			public override string GetText(MethodInfo method, object[] args)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(method.DeclaringType.FullName);
				stringBuilder.Append('.');
				stringBuilder.Append(method.Name);
				if (method.IsGenericMethod)
				{
					NameFragment.AppendGenericTypeNames(stringBuilder, method);
				}
				return stringBuilder.ToString();
			}
		}

		private class ClassNameFragment : NameFragment
		{
			public override string GetText(MethodInfo method, object[] args)
			{
				return method.DeclaringType.Name;
			}
		}

		private class ClassFullNameFragment : NameFragment
		{
			public override string GetText(MethodInfo method, object[] args)
			{
				return method.DeclaringType.FullName;
			}
		}

		private class ArgListFragment : NameFragment
		{
			private int _maxStringLength;

			public ArgListFragment(int maxStringLength)
			{
				_maxStringLength = maxStringLength;
			}

			public override string GetText(MethodInfo method, object[] arglist)
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (arglist != null)
				{
					stringBuilder.Append('(');
					for (int i = 0; i < arglist.Length; i++)
					{
						if (i > 0)
						{
							stringBuilder.Append(",");
						}
						stringBuilder.Append(NameFragment.GetDisplayString(arglist[i], _maxStringLength));
					}
					stringBuilder.Append(')');
				}
				return stringBuilder.ToString();
			}
		}

		private class ArgumentFragment : NameFragment
		{
			private int _index;

			private int _maxStringLength;

			public ArgumentFragment(int index, int maxStringLength)
			{
				_index = index;
				_maxStringLength = maxStringLength;
			}

			public override string GetText(MethodInfo method, object[] args)
			{
				return (_index < args.Length) ? NameFragment.GetDisplayString(args[_index], _maxStringLength) : string.Empty;
			}
		}

		public static string DefaultTestNamePattern = "{m}{a}";

		private string _pattern;

		private List<NameFragment> _fragments;

		public TestNameGenerator()
		{
			_pattern = DefaultTestNamePattern;
		}

		public TestNameGenerator(string pattern)
		{
			_pattern = pattern;
		}

		public string GetDisplayName(TestMethod testMethod)
		{
			return GetDisplayName(testMethod, null);
		}

		public string GetDisplayName(TestMethod testMethod, object[] args)
		{
			if (_fragments == null)
			{
				_fragments = BuildFragmentList(_pattern);
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (NameFragment fragment in _fragments)
			{
				stringBuilder.Append(fragment.GetText(testMethod, args));
			}
			return stringBuilder.ToString();
		}

		private static List<NameFragment> BuildFragmentList(string pattern)
		{
			List<NameFragment> list = new List<NameFragment>();
			int num = 0;
			while (num < pattern.Length)
			{
				int num2 = pattern.IndexOf('{', num);
				if (num2 < 0)
				{
					break;
				}
				int num3 = pattern.IndexOf('}', num2);
				if (num3 < 0)
				{
					break;
				}
				if (num2 > num)
				{
					list.Add(new FixedTextFragment(pattern.Substring(num, num2 - num)));
				}
				string text = pattern.Substring(num2, num3 - num2 + 1);
				switch (text)
				{
				case "{m}":
					list.Add(new MethodNameFragment());
					break;
				case "{i}":
					list.Add(new TestIDFragment());
					break;
				case "{n}":
					list.Add(new NamespaceFragment());
					break;
				case "{c}":
					list.Add(new ClassNameFragment());
					break;
				case "{C}":
					list.Add(new ClassFullNameFragment());
					break;
				case "{M}":
					list.Add(new MethodFullNameFragment());
					break;
				case "{a}":
					list.Add(new ArgListFragment(0));
					break;
				case "{0}":
				case "{1}":
				case "{2}":
				case "{3}":
				case "{4}":
				case "{5}":
				case "{6}":
				case "{7}":
				case "{8}":
				case "{9}":
				{
					int index = text[1] - 48;
					list.Add(new ArgumentFragment(index, 40));
					break;
				}
				default:
				{
					char c = text[1];
					if (text.Length >= 5 && text[2] == ':' && (c == 'a' || char.IsDigit(c)))
					{
						int num4;
						try
						{
							num4 = int.Parse(text.Substring(3, text.Length - 4));
						}
						catch
						{
							num4 = -1;
						}
						if (num4 > 0)
						{
							if (c == 'a')
							{
								list.Add(new ArgListFragment(num4));
							}
							else
							{
								list.Add(new ArgumentFragment(c - 48, num4));
							}
							break;
						}
					}
					list.Add(new FixedTextFragment(text));
					break;
				}
				}
				num = num3 + 1;
			}
			if (num < pattern.Length)
			{
				list.Add(new FixedTextFragment(pattern.Substring(num)));
			}
			return list;
		}
	}
}
