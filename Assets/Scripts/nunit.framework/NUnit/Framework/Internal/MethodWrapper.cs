using System;
using System.Reflection;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
	public class MethodWrapper : IMethodInfo, IReflectionInfo
	{
		public ITypeInfo TypeInfo { get; private set; }

		public MethodInfo MethodInfo { get; private set; }

		public string Name
		{
			get
			{
				return MethodInfo.Name;
			}
		}

		public bool IsAbstract
		{
			get
			{
				return MethodInfo.IsAbstract;
			}
		}

		public bool IsPublic
		{
			get
			{
				return MethodInfo.IsPublic;
			}
		}

		public bool ContainsGenericParameters
		{
			get
			{
				return MethodInfo.ContainsGenericParameters;
			}
		}

		public bool IsGenericMethod
		{
			get
			{
				return MethodInfo.IsGenericMethod;
			}
		}

		public bool IsGenericMethodDefinition
		{
			get
			{
				return MethodInfo.IsGenericMethodDefinition;
			}
		}

		public ITypeInfo ReturnType
		{
			get
			{
				return new TypeWrapper(MethodInfo.ReturnType);
			}
		}

		public MethodWrapper(Type type, MethodInfo method)
		{
			TypeInfo = new TypeWrapper(type);
			MethodInfo = method;
		}

		public MethodWrapper(Type type, string methodName)
		{
			TypeInfo = new TypeWrapper(type);
			MethodInfo = type.GetMethod(methodName);
		}

		public IParameterInfo[] GetParameters()
		{
			ParameterInfo[] parameters = MethodInfo.GetParameters();
			IParameterInfo[] array = new IParameterInfo[parameters.Length];
			for (int i = 0; i < parameters.Length; i++)
			{
				array[i] = new ParameterWrapper(this, parameters[i]);
			}
			return array;
		}

		public Type[] GetGenericArguments()
		{
			return MethodInfo.GetGenericArguments();
		}

		public IMethodInfo MakeGenericMethod(params Type[] typeArguments)
		{
			return new MethodWrapper(TypeInfo.Type, MethodInfo.MakeGenericMethod(typeArguments));
		}

		public T[] GetCustomAttributes<T>(bool inherit) where T : class
		{
			return (T[])MethodInfo.GetCustomAttributes(typeof(T), inherit);
		}

		public bool IsDefined<T>(bool inherit)
		{
			return MethodInfo.IsDefined(typeof(T), inherit);
		}

		public object Invoke(object fixture, params object[] args)
		{
			return Reflect.InvokeMethod(MethodInfo, fixture, args);
		}

		public override string ToString()
		{
			return MethodInfo.Name;
		}
	}
}
