using System;
using System.Linq;
using System.Reflection;
using NUnit.Compatibility;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
	public class TypeWrapper : ITypeInfo, IReflectionInfo
	{
		public Type Type { get; private set; }

		public ITypeInfo BaseType
		{
			get
			{
				Type baseType = TypeExtensions.GetTypeInfo(Type).BaseType;
				return ((object)baseType != null) ? new TypeWrapper(baseType) : null;
			}
		}

		public string Name
		{
			get
			{
				return Type.Name;
			}
		}

		public string FullName
		{
			get
			{
				return Type.FullName;
			}
		}

		public Assembly Assembly
		{
			get
			{
				return TypeExtensions.GetTypeInfo(Type).Assembly;
			}
		}

		public string Namespace
		{
			get
			{
				return Type.Namespace;
			}
		}

		public bool IsAbstract
		{
			get
			{
				return TypeExtensions.GetTypeInfo(Type).IsAbstract;
			}
		}

		public bool IsGenericType
		{
			get
			{
				return TypeExtensions.GetTypeInfo(Type).IsGenericType;
			}
		}

		public bool ContainsGenericParameters
		{
			get
			{
				return TypeExtensions.GetTypeInfo(Type).ContainsGenericParameters;
			}
		}

		public bool IsGenericTypeDefinition
		{
			get
			{
				return TypeExtensions.GetTypeInfo(Type).IsGenericTypeDefinition;
			}
		}

		public bool IsSealed
		{
			get
			{
				return TypeExtensions.GetTypeInfo(Type).IsSealed;
			}
		}

		public bool IsStaticClass
		{
			get
			{
				return TypeExtensions.GetTypeInfo(Type).IsSealed && TypeExtensions.GetTypeInfo(Type).IsAbstract;
			}
		}

		public TypeWrapper(Type type)
		{
			Guard.ArgumentNotNull(type, "Type");
			Type = type;
		}

		public bool IsType(Type type)
		{
			return (object)Type == type;
		}

		public string GetDisplayName()
		{
			return TypeHelper.GetDisplayName(Type);
		}

		public string GetDisplayName(object[] args)
		{
			return TypeHelper.GetDisplayName(Type, args);
		}

		public ITypeInfo MakeGenericType(Type[] typeArgs)
		{
			return new TypeWrapper(Type.MakeGenericType(typeArgs));
		}

		public Type GetGenericTypeDefinition()
		{
			return Type.GetGenericTypeDefinition();
		}

		public T[] GetCustomAttributes<T>(bool inherit) where T : class
		{
			return (T[])Type.GetCustomAttributes(typeof(T), inherit);
		}

		public bool IsDefined<T>(bool inherit)
		{
			return TypeExtensions.GetTypeInfo(Type).IsDefined(typeof(T), inherit);
		}

		public bool HasMethodWithAttribute(Type attributeType)
		{
			return Reflect.HasMethodWithAttribute(Type, attributeType);
		}

		public IMethodInfo[] GetMethods(BindingFlags flags)
		{
			MethodInfo[] methods = Type.GetMethods(flags);
			MethodWrapper[] array = new MethodWrapper[methods.Length];
			for (int i = 0; i < methods.Length; i++)
			{
				array[i] = new MethodWrapper(Type, methods[i]);
			}
			return array;
		}

		public ConstructorInfo GetConstructor(Type[] argTypes)
		{
			return (from c in Type.GetConstructors()
				where c.GetParameters().ParametersMatch(argTypes)
				select c).FirstOrDefault();
		}

		public bool HasConstructor(Type[] argTypes)
		{
			return (object)GetConstructor(argTypes) != null;
		}

		public object Construct(object[] args)
		{
			return Reflect.Construct(Type, args);
		}

		public override string ToString()
		{
			return Type.ToString();
		}
	}
}
