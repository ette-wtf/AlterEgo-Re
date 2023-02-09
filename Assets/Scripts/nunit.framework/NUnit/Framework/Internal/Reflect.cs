using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using NUnit.Compatibility;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
	public static class Reflect
	{
		private class BaseTypesFirstComparer : IComparer<MethodInfo>
		{
			public int Compare(MethodInfo m1, MethodInfo m2)
			{
				if ((object)m1 == null || (object)m2 == null)
				{
					return 0;
				}
				Type declaringType = m1.DeclaringType;
				Type declaringType2 = m2.DeclaringType;
				if ((object)declaringType == declaringType2)
				{
					return 0;
				}
				if (declaringType.IsAssignableFrom(declaringType2))
				{
					return -1;
				}
				return 1;
			}
		}

		private static readonly BindingFlags AllMembers = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		private static readonly Type[] EmptyTypes = new Type[0];

		public static Func<Func<object>, object> MethodCallWrapper { get; set; }

		public static Func<Type, object[], object> ConstructorCallWrapper { get; set; }

		public static MethodInfo[] GetMethodsWithAttribute(Type fixtureType, Type attributeType, bool inherit)
		{
			List<MethodInfo> list = new List<MethodInfo>();
			BindingFlags bindingAttr = AllMembers | (inherit ? BindingFlags.FlattenHierarchy : BindingFlags.DeclaredOnly);
			MethodInfo[] methods = fixtureType.GetMethods(bindingAttr);
			foreach (MethodInfo methodInfo in methods)
			{
				if (methodInfo.IsDefined(attributeType, inherit))
				{
					list.Add(methodInfo);
				}
			}
			list.Sort(new BaseTypesFirstComparer());
			return list.ToArray();
		}

		public static bool HasMethodWithAttribute(Type fixtureType, Type attributeType)
		{
			MethodInfo[] methods = fixtureType.GetMethods(AllMembers | BindingFlags.FlattenHierarchy);
			foreach (MethodInfo methodInfo in methods)
			{
				if (methodInfo.IsDefined(attributeType, false))
				{
					return true;
				}
			}
			return false;
		}

		public static object Construct(Type type)
		{
			if (ConstructorCallWrapper != null)
			{
				return ConstructorCallWrapper(type, null);
			}
			ConstructorInfo constructor = type.GetConstructor(EmptyTypes);
			if ((object)constructor == null)
			{
				throw new InvalidTestFixtureException(type.FullName + " does not have a default constructor");
			}
			return constructor.Invoke(null);
		}

		public static object Construct(Type type, object[] arguments)
		{
			if (ConstructorCallWrapper != null)
			{
				return ConstructorCallWrapper(type, arguments);
			}
			if (arguments == null)
			{
				return Construct(type);
			}
			Type[] typeArray = GetTypeArray(arguments);
			ITypeInfo typeInfo = new TypeWrapper(type);
			ConstructorInfo constructor = typeInfo.GetConstructor(typeArray);
			if ((object)constructor == null)
			{
				throw new InvalidTestFixtureException(type.FullName + " does not have a suitable constructor");
			}
			return constructor.Invoke(arguments);
		}

		internal static Type[] GetTypeArray(object[] objects)
		{
			Type[] array = new Type[objects.Length];
			int num = 0;
			foreach (object obj in objects)
			{
				array[num++] = ((obj == null) ? typeof(NUnitNullType) : obj.GetType());
			}
			return array;
		}

		public static object InvokeMethod(MethodInfo method, object fixture)
		{
			return InvokeMethod(method, fixture, null);
		}

		public static object InvokeMethod(MethodInfo method, object fixture, params object[] args)
		{
			if ((object)method != null)
			{
				try
				{
					if (MethodCallWrapper != null)
					{
						return MethodCallWrapper(() => method.Invoke(fixture, args));
					}
					return method.Invoke(fixture, args);
				}
				catch (ThreadAbortException)
				{
					return null;
				}
				catch (TargetInvocationException ex2)
				{
					throw new NUnitException("Rethrown", ex2.InnerException);
				}
				catch (Exception inner)
				{
					throw new NUnitException("Rethrown", inner);
				}
			}
			return null;
		}
	}
}
