using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NUnit.Compatibility
{
	public static class AdditionalTypeExtensions
	{
		private static Dictionary<Type, List<Type>> convertibleValueTypes = new Dictionary<Type, List<Type>>
		{
			{
				typeof(decimal),
				new List<Type>
				{
					typeof(sbyte),
					typeof(byte),
					typeof(short),
					typeof(ushort),
					typeof(int),
					typeof(uint),
					typeof(long),
					typeof(ulong),
					typeof(char)
				}
			},
			{
				typeof(double),
				new List<Type>
				{
					typeof(sbyte),
					typeof(byte),
					typeof(short),
					typeof(ushort),
					typeof(int),
					typeof(uint),
					typeof(long),
					typeof(ulong),
					typeof(char),
					typeof(float)
				}
			},
			{
				typeof(float),
				new List<Type>
				{
					typeof(sbyte),
					typeof(byte),
					typeof(short),
					typeof(ushort),
					typeof(int),
					typeof(uint),
					typeof(long),
					typeof(ulong),
					typeof(char),
					typeof(float)
				}
			},
			{
				typeof(ulong),
				new List<Type>
				{
					typeof(byte),
					typeof(ushort),
					typeof(uint),
					typeof(char)
				}
			},
			{
				typeof(long),
				new List<Type>
				{
					typeof(sbyte),
					typeof(byte),
					typeof(short),
					typeof(ushort),
					typeof(int),
					typeof(uint),
					typeof(char)
				}
			},
			{
				typeof(uint),
				new List<Type>
				{
					typeof(byte),
					typeof(ushort),
					typeof(char)
				}
			},
			{
				typeof(int),
				new List<Type>
				{
					typeof(sbyte),
					typeof(byte),
					typeof(short),
					typeof(ushort),
					typeof(char)
				}
			},
			{
				typeof(ushort),
				new List<Type>
				{
					typeof(byte),
					typeof(char)
				}
			},
			{
				typeof(short),
				new List<Type> { typeof(byte) }
			}
		};

		public static bool ParametersMatch(this ParameterInfo[] pinfos, Type[] ptypes)
		{
			if (pinfos.Length != ptypes.Length)
			{
				return false;
			}
			for (int i = 0; i < pinfos.Length; i++)
			{
				if (!pinfos[i].ParameterType.IsCastableFrom(ptypes[i]))
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsCastableFrom(this Type to, Type from)
		{
			if (to.IsAssignableFrom(from))
			{
				return true;
			}
			if ((object)from == typeof(NUnitNullType) && (to.GetTypeInfo().IsClass || to.FullName.StartsWith("System.Nullable")))
			{
				return true;
			}
			if (convertibleValueTypes.ContainsKey(to) && convertibleValueTypes[to].Contains(from))
			{
				return true;
			}
			return from.GetMethods(BindingFlags.Static | BindingFlags.Public).Any((MethodInfo m) => (object)m.ReturnType == to && m.Name == "op_Implicit");
		}
	}
}
