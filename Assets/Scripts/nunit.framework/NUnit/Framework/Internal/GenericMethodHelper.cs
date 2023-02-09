using System;
using System.Reflection;
using NUnit.Compatibility;

namespace NUnit.Framework.Internal
{
	public class GenericMethodHelper
	{
		private MethodInfo Method { get; set; }

		private Type[] TypeParms { get; set; }

		private Type[] TypeArgs { get; set; }

		private Type[] ParmTypes { get; set; }

		public GenericMethodHelper(MethodInfo method)
		{
			Guard.ArgumentValid(method.IsGenericMethod, "Specified method must be generic", "method");
			Method = method;
			TypeParms = Method.GetGenericArguments();
			TypeArgs = new Type[TypeParms.Length];
			ParameterInfo[] parameters = Method.GetParameters();
			ParmTypes = new Type[parameters.Length];
			for (int i = 0; i < parameters.Length; i++)
			{
				ParmTypes[i] = parameters[i].ParameterType;
			}
		}

		public Type[] GetTypeArguments(object[] argList)
		{
			Guard.ArgumentValid(argList.Length == ParmTypes.Length, "Supplied arguments do not match required method parameters", "argList");
			for (int i = 0; i < ParmTypes.Length; i++)
			{
				object obj = argList[i];
				if (obj != null)
				{
					Type type = obj.GetType();
					TryApplyArgType(ParmTypes[i], type);
				}
			}
			return TypeArgs;
		}

		private void TryApplyArgType(Type parmType, Type argType)
		{
			if (parmType.IsGenericParameter)
			{
				ApplyArgType(parmType, argType);
			}
			else
			{
				if (!TypeExtensions.GetTypeInfo(parmType).ContainsGenericParameters)
				{
					return;
				}
				Type[] genericArguments = parmType.GetGenericArguments();
				if (argType.HasElementType)
				{
					ApplyArgType(genericArguments[0], argType.GetElementType());
				}
				else
				{
					if (!TypeExtensions.GetTypeInfo(argType).IsGenericType || !IsAssignableToGenericType(argType, parmType))
					{
						return;
					}
					Type[] genericArguments2 = argType.GetGenericArguments();
					if (genericArguments2.Length == genericArguments.Length)
					{
						for (int i = 0; i < genericArguments.Length; i++)
						{
							TryApplyArgType(genericArguments[i], genericArguments2[i]);
						}
					}
				}
			}
		}

		private void ApplyArgType(Type parmType, Type argType)
		{
			int genericParameterPosition = parmType.GenericParameterPosition;
			TypeArgs[genericParameterPosition] = TypeHelper.BestCommonType(TypeArgs[genericParameterPosition], argType);
		}

		private bool IsAssignableToGenericType(Type givenType, Type genericType)
		{
			Type[] interfaces = givenType.GetInterfaces();
			Type[] array = interfaces;
			foreach (Type type in array)
			{
				if (TypeExtensions.GetTypeInfo(type).IsGenericType)
				{
					Type genericTypeDefinition = type.GetGenericTypeDefinition();
					if (genericTypeDefinition.Name == genericType.Name && genericTypeDefinition.Namespace == genericType.Namespace)
					{
						return true;
					}
				}
			}
			if (TypeExtensions.GetTypeInfo(givenType).IsGenericType)
			{
				Type genericTypeDefinition = givenType.GetGenericTypeDefinition();
				if (genericTypeDefinition.Name == genericType.Name && genericTypeDefinition.Namespace == genericType.Namespace)
				{
					return true;
				}
			}
			Type baseType = TypeExtensions.GetTypeInfo(givenType).BaseType;
			if ((object)baseType == null)
			{
				return false;
			}
			return IsAssignableToGenericType(baseType, genericType);
		}
	}
}
