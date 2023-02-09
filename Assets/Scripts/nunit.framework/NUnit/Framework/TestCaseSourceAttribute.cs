using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	public class TestCaseSourceAttribute : NUnitAttribute, ITestBuilder, IImplyFixture
	{
		private const string SourceMustBeStatic = "The sourceName specified on a TestCaseSourceAttribute must refer to a static field, property or method.";

		private const string ParamGivenToField = "You have specified a data source field but also given a set of parameters. Fields cannot take parameters, please revise the 3rd parameter passed to the TestCaseSourceAttribute and either remove it or specify a method.";

		private const string ParamGivenToProperty = "You have specified a data source property but also given a set of parameters. Properties cannot take parameters, please revise the 3rd parameter passed to the TestCaseSource attribute and either remove it or specify a method.";

		private const string NumberOfArgsDoesNotMatch = "You have given the wrong number of arguments to the method in the TestCaseSourceAttribute, please check the number of parameters passed in the object is correct in the 3rd parameter for the TestCaseSourceAttribute and this matches the number of parameters in the target method and try again.";

		private NUnitTestCaseBuilder _builder = new NUnitTestCaseBuilder();

		public object[] MethodParams { get; private set; }

		public string SourceName { get; private set; }

		public Type SourceType { get; private set; }

		public string Category { get; set; }

		public TestCaseSourceAttribute(string sourceName)
		{
			SourceName = sourceName;
		}

		public TestCaseSourceAttribute(Type sourceType, string sourceName, object[] methodParams)
		{
			MethodParams = methodParams;
			SourceType = sourceType;
			SourceName = sourceName;
		}

		public TestCaseSourceAttribute(Type sourceType, string sourceName)
		{
			SourceType = sourceType;
			SourceName = sourceName;
		}

		public TestCaseSourceAttribute(Type sourceType)
		{
			SourceType = sourceType;
		}

		public IEnumerable<TestMethod> BuildFrom(IMethodInfo method, Test suite)
		{
			foreach (TestCaseParameters parms in GetTestCasesFor(method))
			{
				yield return _builder.BuildTestMethod(method, suite, parms);
			}
		}

		private IEnumerable<ITestCaseData> GetTestCasesFor(IMethodInfo method)
		{
			List<ITestCaseData> list = new List<ITestCaseData>();
			try
			{
				IEnumerable testCaseSource = GetTestCaseSource(method);
				if (testCaseSource != null)
				{
					foreach (object item in testCaseSource)
					{
						object obj;
						if (item != null)
						{
							obj = item as ITestCaseData;
						}
						else
						{
							object[] args = new object[1];
							obj = new TestCaseParameters(args);
						}
						ITestCaseData testCaseData = (ITestCaseData)obj;
						if (testCaseData == null)
						{
							object[] array = item as object[];
							if (array == null && item is Array)
							{
								Array array2 = item as Array;
								int num = (false ? array2.Length : method.GetParameters().Length);
								if (array2 != null && array2.Rank == 1 && array2.Length == num)
								{
									array = new object[array2.Length];
									for (int i = 0; i < array2.Length; i++)
									{
										array[i] = array2.GetValue(i);
									}
								}
							}
							if (array != null)
							{
								IParameterInfo[] parameters = method.GetParameters();
								int num2 = parameters.Length;
								int num3 = array.Length;
								if (num2 == 1)
								{
									Type parameterType = parameters[0].ParameterType;
									if ((num3 == 0 || typeof(object[]).IsAssignableFrom(parameterType)) && (num3 > 1 || parameterType.IsAssignableFrom(array.GetType())))
									{
										array = new object[1] { item };
									}
								}
							}
							else
							{
								array = new object[1] { item };
							}
							testCaseData = new TestCaseParameters(array);
						}
						if (Category != null)
						{
							string[] array3 = Category.Split(',');
							foreach (string value in array3)
							{
								testCaseData.Properties.Add("Category", value);
							}
						}
						list.Add(testCaseData);
					}
				}
			}
			catch (Exception exception)
			{
				list.Clear();
				list.Add(new TestCaseParameters(exception));
			}
			return list;
		}

		private IEnumerable GetTestCaseSource(IMethodInfo method)
		{
			Type type = SourceType ?? method.TypeInfo.Type;
			if (SourceName == null)
			{
				return Reflect.Construct(type, null) as IEnumerable;
			}
			MemberInfo[] member = type.GetMember(SourceName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			if (member.Length == 1)
			{
				MemberInfo memberInfo = member[0];
				FieldInfo fieldInfo = memberInfo as FieldInfo;
				if ((object)fieldInfo != null)
				{
					return (!fieldInfo.IsStatic) ? ReturnErrorAsParameter("The sourceName specified on a TestCaseSourceAttribute must refer to a static field, property or method.") : ((MethodParams == null) ? ((IEnumerable)fieldInfo.GetValue(null)) : ReturnErrorAsParameter("You have specified a data source field but also given a set of parameters. Fields cannot take parameters, please revise the 3rd parameter passed to the TestCaseSourceAttribute and either remove it or specify a method."));
				}
				PropertyInfo propertyInfo = memberInfo as PropertyInfo;
				if ((object)propertyInfo != null)
				{
					return (!propertyInfo.GetGetMethod(true).IsStatic) ? ReturnErrorAsParameter("The sourceName specified on a TestCaseSourceAttribute must refer to a static field, property or method.") : ((MethodParams == null) ? ((IEnumerable)propertyInfo.GetValue(null, null)) : ReturnErrorAsParameter("You have specified a data source property but also given a set of parameters. Properties cannot take parameters, please revise the 3rd parameter passed to the TestCaseSource attribute and either remove it or specify a method."));
				}
				MethodInfo methodInfo = memberInfo as MethodInfo;
				if ((object)methodInfo != null)
				{
					return (!methodInfo.IsStatic) ? ReturnErrorAsParameter("The sourceName specified on a TestCaseSourceAttribute must refer to a static field, property or method.") : ((MethodParams == null || methodInfo.GetParameters().Length == MethodParams.Length) ? ((IEnumerable)methodInfo.Invoke(null, MethodParams)) : ReturnErrorAsParameter("You have given the wrong number of arguments to the method in the TestCaseSourceAttribute, please check the number of parameters passed in the object is correct in the 3rd parameter for the TestCaseSourceAttribute and this matches the number of parameters in the target method and try again."));
				}
			}
			return null;
		}

		private static IEnumerable ReturnErrorAsParameter(string errorMessage)
		{
			TestCaseParameters testCaseParameters = new TestCaseParameters();
			testCaseParameters.RunState = RunState.NotRunnable;
			testCaseParameters.Properties.Set("_SKIPREASON", errorMessage);
			return new TestCaseParameters[1] { testCaseParameters };
		}
	}
}
