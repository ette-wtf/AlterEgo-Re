#define DEBUG
using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using NUnit.Compatibility;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
	public class TypeHelper
	{
		internal sealed class NonmatchingTypeClass
		{
		}

		private const int STRING_MAX = 40;

		private const int STRING_LIMIT = 37;

		private const string THREE_DOTS = "...";

		public static readonly Type NonmatchingType = typeof(NonmatchingTypeClass);

		public static string GetDisplayName(Type type)
		{
			if (type.IsGenericParameter)
			{
				return type.Name;
			}
			if (TypeExtensions.GetTypeInfo(type).IsGenericType)
			{
				string text = type.FullName;
				int num = text.IndexOf('[');
				if (num >= 0)
				{
					text = text.Substring(0, num);
				}
				num = text.LastIndexOf('.');
				if (num >= 0)
				{
					text = text.Substring(num + 1);
				}
				Type[] genericArguments = type.GetGenericArguments();
				int num2 = 0;
				StringBuilder stringBuilder = new StringBuilder();
				bool flag = false;
				string[] array = text.Split('+');
				foreach (string text2 in array)
				{
					if (flag)
					{
						stringBuilder.Append("+");
					}
					flag = true;
					num = text2.IndexOf('`');
					if (num >= 0)
					{
						string value = text2.Substring(0, num);
						stringBuilder.Append(value);
						stringBuilder.Append("<");
						int num3 = int.Parse(text2.Substring(num + 1));
						for (int j = 0; j < num3; j++)
						{
							if (j > 0)
							{
								stringBuilder.Append(",");
							}
							stringBuilder.Append(GetDisplayName(genericArguments[num2++]));
						}
						stringBuilder.Append(">");
					}
					else
					{
						stringBuilder.Append(text2);
					}
				}
				return stringBuilder.ToString();
			}
			int num4 = type.FullName.LastIndexOf('.');
			return (num4 >= 0) ? type.FullName.Substring(num4 + 1) : type.FullName;
		}

		public static string GetDisplayName(Type type, object[] arglist)
		{
			string displayName = GetDisplayName(type);
			if (arglist == null || arglist.Length == 0)
			{
				return displayName;
			}
			StringBuilder stringBuilder = new StringBuilder(displayName);
			stringBuilder.Append("(");
			for (int i = 0; i < arglist.Length; i++)
			{
				if (i > 0)
				{
					stringBuilder.Append(",");
				}
				object obj = arglist[i];
				string text = ((obj == null) ? "null" : obj.ToString());
				if (obj is double || obj is float)
				{
					if (text.IndexOf('.') == -1)
					{
						text += ".0";
					}
					text += ((obj is double) ? "d" : "f");
				}
				else if (obj is decimal)
				{
					text += "m";
				}
				else if (obj is long)
				{
					text += "L";
				}
				else if (obj is ulong)
				{
					text += "UL";
				}
				else if (obj is string)
				{
					if (text.Length > 40)
					{
						text = text.Substring(0, 37) + "...";
					}
					text = "\"" + text + "\"";
				}
				stringBuilder.Append(text);
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		public static Type BestCommonType(Type type1, Type type2)
		{
			if ((object)type1 == NonmatchingType)
			{
				return NonmatchingType;
			}
			if ((object)type2 == NonmatchingType)
			{
				return NonmatchingType;
			}
			if ((object)type1 == type2)
			{
				return type1;
			}
			if ((object)type1 == null)
			{
				return type2;
			}
			if ((object)type2 == null)
			{
				return type1;
			}
			if (IsNumeric(type1) && IsNumeric(type2))
			{
				if ((object)type1 == typeof(double))
				{
					return type1;
				}
				if ((object)type2 == typeof(double))
				{
					return type2;
				}
				if ((object)type1 == typeof(float))
				{
					return type1;
				}
				if ((object)type2 == typeof(float))
				{
					return type2;
				}
				if ((object)type1 == typeof(decimal))
				{
					return type1;
				}
				if ((object)type2 == typeof(decimal))
				{
					return type2;
				}
				if ((object)type1 == typeof(ulong))
				{
					return type1;
				}
				if ((object)type2 == typeof(ulong))
				{
					return type2;
				}
				if ((object)type1 == typeof(long))
				{
					return type1;
				}
				if ((object)type2 == typeof(long))
				{
					return type2;
				}
				if ((object)type1 == typeof(uint))
				{
					return type1;
				}
				if ((object)type2 == typeof(uint))
				{
					return type2;
				}
				if ((object)type1 == typeof(int))
				{
					return type1;
				}
				if ((object)type2 == typeof(int))
				{
					return type2;
				}
				if ((object)type1 == typeof(ushort))
				{
					return type1;
				}
				if ((object)type2 == typeof(ushort))
				{
					return type2;
				}
				if ((object)type1 == typeof(short))
				{
					return type1;
				}
				if ((object)type2 == typeof(short))
				{
					return type2;
				}
				if ((object)type1 == typeof(byte))
				{
					return type1;
				}
				if ((object)type2 == typeof(byte))
				{
					return type2;
				}
				if ((object)type1 == typeof(sbyte))
				{
					return type1;
				}
				if ((object)type2 == typeof(sbyte))
				{
					return type2;
				}
			}
			if (type1.IsAssignableFrom(type2))
			{
				return type1;
			}
			if (type2.IsAssignableFrom(type1))
			{
				return type2;
			}
			return NonmatchingType;
		}

		public static bool IsNumeric(Type type)
		{
			return (object)type == typeof(double) || (object)type == typeof(float) || (object)type == typeof(decimal) || (object)type == typeof(long) || (object)type == typeof(int) || (object)type == typeof(short) || (object)type == typeof(ulong) || (object)type == typeof(uint) || (object)type == typeof(ushort) || (object)type == typeof(byte) || (object)type == typeof(sbyte);
		}

		public static void ConvertArgumentList(object[] arglist, IParameterInfo[] parameters)
		{
			Debug.Assert(arglist.Length <= parameters.Length);
			for (int i = 0; i < arglist.Length; i++)
			{
				object obj = arglist[i];
				if (obj == null || !(obj is IConvertible))
				{
					continue;
				}
				Type type = obj.GetType();
				Type parameterType = parameters[i].ParameterType;
				bool flag = false;
				if ((object)type != parameterType && !type.IsAssignableFrom(parameterType) && IsNumeric(type) && IsNumeric(parameterType))
				{
					if ((object)parameterType == typeof(double) || (object)parameterType == typeof(float))
					{
						flag = obj is int || obj is long || obj is short || obj is byte || obj is sbyte;
					}
					else if ((object)parameterType == typeof(long))
					{
						flag = obj is int || obj is short || obj is byte || obj is sbyte;
					}
					else if ((object)parameterType == typeof(short))
					{
						flag = obj is byte || obj is sbyte;
					}
				}
				if (flag)
				{
					arglist[i] = Convert.ChangeType(obj, parameterType, CultureInfo.InvariantCulture);
				}
			}
		}

		public static bool CanDeduceTypeArgsFromArgs(Type type, object[] arglist, ref Type[] typeArgsOut)
		{
			Type[] genericArguments = type.GetGenericArguments();
			ConstructorInfo[] constructors = type.GetConstructors();
			foreach (ConstructorInfo constructorInfo in constructors)
			{
				ParameterInfo[] parameters = constructorInfo.GetParameters();
				if (parameters.Length != arglist.Length)
				{
					continue;
				}
				Type[] array = new Type[genericArguments.Length];
				for (int j = 0; j < array.Length; j++)
				{
					for (int k = 0; k < arglist.Length; k++)
					{
						if (genericArguments[j].IsGenericParameter || parameters[k].ParameterType.Equals(genericArguments[j]))
						{
							array[j] = BestCommonType(array[j], arglist[k].GetType());
						}
					}
					if ((object)array[j] == null)
					{
						array = null;
						break;
					}
				}
				if (array != null)
				{
					typeArgsOut = array;
					return true;
				}
			}
			return false;
		}

		public static Array GetEnumValues(Type enumType)
		{
			return Enum.GetValues(enumType);
		}

		public static string[] GetEnumNames(Type enumType)
		{
			return Enum.GetNames(enumType);
		}
	}
}
