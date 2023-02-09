using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Compatibility;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Builders
{
	public class DatapointProvider : IParameterDataProvider
	{
		public bool HasDataFor(IParameterInfo parameter)
		{
			IMethodInfo method = parameter.Method;
			if (!method.IsDefined<TheoryAttribute>(true))
			{
				return false;
			}
			Type parameterType = parameter.ParameterType;
			if ((object)parameterType == typeof(bool) || TypeExtensions.GetTypeInfo(parameterType).IsEnum)
			{
				return true;
			}
			Type type = method.TypeInfo.Type;
			MemberInfo[] members = type.GetMembers(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			foreach (MemberInfo memberInfo in members)
			{
				if (memberInfo.IsDefined(typeof(DatapointAttribute), true) && (object)GetTypeFromMemberInfo(memberInfo) == parameterType)
				{
					return true;
				}
				if (memberInfo.IsDefined(typeof(DatapointSourceAttribute), true) && (object)GetElementTypeFromMemberInfo(memberInfo) == parameterType)
				{
					return true;
				}
			}
			return false;
		}

		public IEnumerable GetDataFor(IParameterInfo parameter)
		{
			List<object> list = new List<object>();
			Type parameterType = parameter.ParameterType;
			Type type = parameter.Method.TypeInfo.Type;
			MemberInfo[] members = type.GetMembers(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			foreach (MemberInfo memberInfo in members)
			{
				if (memberInfo.IsDefined(typeof(DatapointAttribute), true))
				{
					FieldInfo fieldInfo = memberInfo as FieldInfo;
					if ((object)GetTypeFromMemberInfo(memberInfo) == parameterType && (object)fieldInfo != null)
					{
						if (fieldInfo.IsStatic)
						{
							list.Add(fieldInfo.GetValue(null));
						}
						else
						{
							list.Add(fieldInfo.GetValue(ProviderCache.GetInstanceOf(type)));
						}
					}
				}
				else
				{
					if (!memberInfo.IsDefined(typeof(DatapointSourceAttribute), true) || (object)GetElementTypeFromMemberInfo(memberInfo) != parameterType)
					{
						continue;
					}
					FieldInfo fieldInfo = memberInfo as FieldInfo;
					PropertyInfo propertyInfo = memberInfo as PropertyInfo;
					MethodInfo methodInfo = memberInfo as MethodInfo;
					if ((object)fieldInfo != null)
					{
						object obj = (fieldInfo.IsStatic ? null : ProviderCache.GetInstanceOf(type));
						foreach (object item in (IEnumerable)fieldInfo.GetValue(obj))
						{
							list.Add(item);
						}
					}
					else if ((object)propertyInfo != null)
					{
						MethodInfo getMethod = propertyInfo.GetGetMethod(true);
						object obj = (getMethod.IsStatic ? null : ProviderCache.GetInstanceOf(type));
						foreach (object item2 in (IEnumerable)propertyInfo.GetValue(obj, null))
						{
							list.Add(item2);
						}
					}
					else
					{
						if ((object)methodInfo == null)
						{
							continue;
						}
						object obj = (methodInfo.IsStatic ? null : ProviderCache.GetInstanceOf(type));
						foreach (object item3 in (IEnumerable)methodInfo.Invoke(obj, new Type[0]))
						{
							list.Add(item3);
						}
					}
				}
			}
			if (list.Count == 0)
			{
				if ((object)parameterType == typeof(bool))
				{
					list.Add(true);
					list.Add(false);
				}
				else if (TypeExtensions.GetTypeInfo(parameterType).IsEnum)
				{
					foreach (object enumValue in TypeHelper.GetEnumValues(parameterType))
					{
						list.Add(enumValue);
					}
				}
			}
			return list;
		}

		private Type GetTypeFromMemberInfo(MemberInfo member)
		{
			FieldInfo fieldInfo = member as FieldInfo;
			if ((object)fieldInfo != null)
			{
				return fieldInfo.FieldType;
			}
			PropertyInfo propertyInfo = member as PropertyInfo;
			if ((object)propertyInfo != null)
			{
				return propertyInfo.PropertyType;
			}
			MethodInfo methodInfo = member as MethodInfo;
			if ((object)methodInfo != null)
			{
				return methodInfo.ReturnType;
			}
			return null;
		}

		private Type GetElementTypeFromMemberInfo(MemberInfo member)
		{
			Type typeFromMemberInfo = GetTypeFromMemberInfo(member);
			if ((object)typeFromMemberInfo == null)
			{
				return null;
			}
			if (typeFromMemberInfo.IsArray)
			{
				return typeFromMemberInfo.GetElementType();
			}
			if (TypeExtensions.GetTypeInfo(typeFromMemberInfo).IsGenericType && typeFromMemberInfo.Name == "IEnumerable`1")
			{
				return typeFromMemberInfo.GetGenericArguments()[0];
			}
			return null;
		}
	}
}
