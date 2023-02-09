using System;
using System.Reflection;

namespace NUnit.Framework.Interfaces
{
	public interface IParameterInfo : IReflectionInfo
	{
		bool IsOptional { get; }

		IMethodInfo Method { get; }

		ParameterInfo ParameterInfo { get; }

		Type ParameterType { get; }
	}
}
