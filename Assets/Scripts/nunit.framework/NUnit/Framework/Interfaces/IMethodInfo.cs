using System;
using System.Reflection;

namespace NUnit.Framework.Interfaces
{
	public interface IMethodInfo : IReflectionInfo
	{
		ITypeInfo TypeInfo { get; }

		MethodInfo MethodInfo { get; }

		string Name { get; }

		bool IsAbstract { get; }

		bool IsPublic { get; }

		bool ContainsGenericParameters { get; }

		bool IsGenericMethod { get; }

		bool IsGenericMethodDefinition { get; }

		ITypeInfo ReturnType { get; }

		IParameterInfo[] GetParameters();

		Type[] GetGenericArguments();

		IMethodInfo MakeGenericMethod(params Type[] typeArguments);

		object Invoke(object fixture, params object[] args);
	}
}
