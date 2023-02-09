using System;
using System.Collections;
using System.Globalization;
using NUnit.Compatibility;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
	public class ValuesAttribute : DataAttribute, IParameterDataSource
	{
		protected object[] data;

		public ValuesAttribute()
		{
			data = new object[0];
		}

		public ValuesAttribute(object arg1)
		{
			data = new object[1] { arg1 };
		}

		public ValuesAttribute(object arg1, object arg2)
		{
			data = new object[2] { arg1, arg2 };
		}

		public ValuesAttribute(object arg1, object arg2, object arg3)
		{
			data = new object[3] { arg1, arg2, arg3 };
		}

		public ValuesAttribute(params object[] args)
		{
			data = args;
		}

		public IEnumerable GetData(IParameterInfo parameter)
		{
			Type parameterType = parameter.ParameterType;
			if (parameterType.GetTypeInfo().IsEnum && data.Length == 0)
			{
				return TypeHelper.GetEnumValues(parameterType);
			}
			if ((object)parameterType == typeof(bool) && data.Length == 0)
			{
				return new object[2] { true, false };
			}
			return GetData(parameterType);
		}

		private IEnumerable GetData(Type targetType)
		{
			for (int i = 0; i < data.Length; i++)
			{
				object obj = data[i];
				if (obj == null)
				{
					continue;
				}
				if (obj.GetType().FullName == "NUnit.Framework.SpecialValue" && obj.ToString() == "Null")
				{
					data[i] = null;
				}
				else
				{
					if (targetType.GetTypeInfo().IsAssignableFrom(obj.GetType().GetTypeInfo()))
					{
						continue;
					}
					if (obj is DBNull)
					{
						data[i] = null;
						continue;
					}
					bool flag = false;
					if ((object)targetType == typeof(short) || (object)targetType == typeof(byte) || (object)targetType == typeof(sbyte))
					{
						flag = obj is int;
					}
					else if ((object)targetType == typeof(decimal))
					{
						flag = obj is double || obj is string || obj is int;
					}
					else if ((object)targetType == typeof(DateTime) || (object)targetType == typeof(TimeSpan))
					{
						flag = obj is string;
					}
					if (flag)
					{
						data[i] = Convert.ChangeType(obj, targetType, CultureInfo.InvariantCulture);
					}
				}
			}
			return data;
		}
	}
}
