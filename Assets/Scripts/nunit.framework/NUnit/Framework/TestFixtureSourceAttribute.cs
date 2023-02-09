using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public class TestFixtureSourceAttribute : NUnitAttribute, IFixtureBuilder
	{
		public const string MUST_BE_STATIC = "The sourceName specified on a TestCaseSourceAttribute must refer to a static field, property or method.";

		private readonly NUnitTestFixtureBuilder _builder = new NUnitTestFixtureBuilder();

		public string SourceName { get; private set; }

		public Type SourceType { get; private set; }

		public string Category { get; set; }

		public TestFixtureSourceAttribute(string sourceName)
		{
			SourceName = sourceName;
		}

		public TestFixtureSourceAttribute(Type sourceType, string sourceName)
		{
			SourceType = sourceType;
			SourceName = sourceName;
		}

		public TestFixtureSourceAttribute(Type sourceType)
		{
			SourceType = sourceType;
		}

		public IEnumerable<TestSuite> BuildFrom(ITypeInfo typeInfo)
		{
			Type sourceType = SourceType ?? typeInfo.Type;
			foreach (TestFixtureParameters parms in GetParametersFor(sourceType))
			{
				yield return _builder.BuildFrom(typeInfo, parms);
			}
		}

		public IEnumerable<ITestFixtureData> GetParametersFor(Type sourceType)
		{
			List<ITestFixtureData> list = new List<ITestFixtureData>();
			try
			{
				IEnumerable testFixtureSource = GetTestFixtureSource(sourceType);
				if (testFixtureSource != null)
				{
					foreach (object item in testFixtureSource)
					{
						ITestFixtureData testFixtureData = item as ITestFixtureData;
						if (testFixtureData == null)
						{
							object[] array = item as object[];
							if (array == null)
							{
								array = new object[1] { item };
							}
							testFixtureData = new TestFixtureParameters(array);
						}
						if (Category != null)
						{
							string[] array2 = Category.Split(',');
							foreach (string value in array2)
							{
								testFixtureData.Properties.Add("Category", value);
							}
						}
						list.Add(testFixtureData);
					}
				}
			}
			catch (Exception exception)
			{
				list.Clear();
				list.Add(new TestFixtureParameters(exception));
			}
			return list;
		}

		private IEnumerable GetTestFixtureSource(Type sourceType)
		{
			if (SourceName == null)
			{
				return Reflect.Construct(sourceType) as IEnumerable;
			}
			MemberInfo[] member = sourceType.GetMember(SourceName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			if (member.Length == 1)
			{
				MemberInfo memberInfo = member[0];
				FieldInfo fieldInfo = memberInfo as FieldInfo;
				if ((object)fieldInfo != null)
				{
					return fieldInfo.IsStatic ? ((IEnumerable)fieldInfo.GetValue(null)) : SourceMustBeStaticError();
				}
				PropertyInfo propertyInfo = memberInfo as PropertyInfo;
				if ((object)propertyInfo != null)
				{
					return propertyInfo.GetGetMethod(true).IsStatic ? ((IEnumerable)propertyInfo.GetValue(null, null)) : SourceMustBeStaticError();
				}
				MethodInfo methodInfo = memberInfo as MethodInfo;
				if ((object)methodInfo != null)
				{
					return methodInfo.IsStatic ? ((IEnumerable)methodInfo.Invoke(null, null)) : SourceMustBeStaticError();
				}
			}
			return null;
		}

		private static IEnumerable SourceMustBeStaticError()
		{
			TestFixtureParameters testFixtureParameters = new TestFixtureParameters();
			testFixtureParameters.RunState = RunState.NotRunnable;
			testFixtureParameters.Properties.Set("_SKIPREASON", "The sourceName specified on a TestCaseSourceAttribute must refer to a static field, property or method.");
			return new TestFixtureParameters[1] { testFixtureParameters };
		}
	}
}
