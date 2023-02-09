using System;
using System.Reflection;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
	public class ParameterWrapper : IParameterInfo, IReflectionInfo
	{
		public bool IsOptional
		{
			get
			{
				return ParameterInfo.IsOptional;
			}
		}

		public IMethodInfo Method { get; private set; }

		public ParameterInfo ParameterInfo { get; private set; }

		public Type ParameterType
		{
			get
			{
				return ParameterInfo.ParameterType;
			}
		}

		public ParameterWrapper(IMethodInfo method, ParameterInfo parameterInfo)
		{
			Method = method;
			ParameterInfo = parameterInfo;
		}

		public T[] GetCustomAttributes<T>(bool inherit) where T : class
		{
			return (T[])ParameterInfo.GetCustomAttributes(typeof(T), inherit);
		}

		public bool IsDefined<T>(bool inherit)
		{
			return ParameterInfo.IsDefined(typeof(T), inherit);
		}
	}
}
