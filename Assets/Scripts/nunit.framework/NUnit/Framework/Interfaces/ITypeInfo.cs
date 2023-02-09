using System;
using System.Reflection;

namespace NUnit.Framework.Interfaces
{
	public interface ITypeInfo : IReflectionInfo
	{
		Type Type { get; }

		ITypeInfo BaseType { get; }

		string Name { get; }

		string FullName { get; }

		Assembly Assembly { get; }

		string Namespace { get; }

		bool IsAbstract { get; }

		bool IsGenericType { get; }

		bool ContainsGenericParameters { get; }

		bool IsGenericTypeDefinition { get; }

		bool IsSealed { get; }

		bool IsStaticClass { get; }

		bool IsType(Type type);

		string GetDisplayName();

		string GetDisplayName(object[] args);

		Type GetGenericTypeDefinition();

		ITypeInfo MakeGenericType(Type[] typeArgs);

		bool HasMethodWithAttribute(Type attrType);

		IMethodInfo[] GetMethods(BindingFlags flags);

		ConstructorInfo GetConstructor(Type[] argTypes);

		bool HasConstructor(Type[] argTypes);

		object Construct(object[] args);
	}
}
