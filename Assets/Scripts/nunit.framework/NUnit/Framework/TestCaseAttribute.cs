using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using NUnit.Compatibility;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	public class TestCaseAttribute : NUnitAttribute, ITestBuilder, ITestCaseData, ITestData, IImplyFixture
	{
		private object _expectedResult;

		private Type _testOf;

		public string TestName { get; set; }

		public RunState RunState { get; private set; }

		public object[] Arguments { get; private set; }

		public IPropertyBag Properties { get; private set; }

		public object ExpectedResult
		{
			get
			{
				return _expectedResult;
			}
			set
			{
				_expectedResult = value;
				HasExpectedResult = true;
			}
		}

		public bool HasExpectedResult { get; private set; }

		public string Description
		{
			get
			{
				return Properties.Get("Description") as string;
			}
			set
			{
				Properties.Set("Description", value);
			}
		}

		public string Author
		{
			get
			{
				return Properties.Get("Author") as string;
			}
			set
			{
				Properties.Set("Author", value);
			}
		}

		public Type TestOf
		{
			get
			{
				return _testOf;
			}
			set
			{
				_testOf = value;
				Properties.Set("TestOf", value.FullName);
			}
		}

		public string Ignore
		{
			get
			{
				return IgnoreReason;
			}
			set
			{
				IgnoreReason = value;
			}
		}

		public bool Explicit
		{
			get
			{
				return RunState == RunState.Explicit;
			}
			set
			{
				RunState = ((!value) ? RunState.Runnable : RunState.Explicit);
			}
		}

		public string Reason
		{
			get
			{
				return Properties.Get("_SKIPREASON") as string;
			}
			set
			{
				Properties.Set("_SKIPREASON", value);
			}
		}

		public string IgnoreReason
		{
			get
			{
				return Reason;
			}
			set
			{
				RunState = RunState.Ignored;
				Reason = value;
			}
		}

		public string IncludePlatform { get; set; }

		public string ExcludePlatform { get; set; }

		public string Category
		{
			get
			{
				return Properties.Get("Category") as string;
			}
			set
			{
				string[] array = value.Split(',');
				foreach (string value2 in array)
				{
					Properties.Add("Category", value2);
				}
			}
		}

		public TestCaseAttribute(params object[] arguments)
		{
			RunState = RunState.Runnable;
			if (arguments == null)
			{
				object[] arguments2 = new object[1];
				Arguments = arguments2;
			}
			else
			{
				Arguments = arguments;
			}
			Properties = new PropertyBag();
		}

		public TestCaseAttribute(object arg)
		{
			RunState = RunState.Runnable;
			Arguments = new object[1] { arg };
			Properties = new PropertyBag();
		}

		public TestCaseAttribute(object arg1, object arg2)
		{
			RunState = RunState.Runnable;
			Arguments = new object[2] { arg1, arg2 };
			Properties = new PropertyBag();
		}

		public TestCaseAttribute(object arg1, object arg2, object arg3)
		{
			RunState = RunState.Runnable;
			Arguments = new object[3] { arg1, arg2, arg3 };
			Properties = new PropertyBag();
		}

		private TestCaseParameters GetParametersForTestCase(IMethodInfo method)
		{
			TestCaseParameters testCaseParameters;
			try
			{
				IParameterInfo[] parameters = method.GetParameters();
				int num = parameters.Length;
				int num2 = Arguments.Length;
				testCaseParameters = new TestCaseParameters(this);
				if (num > 0 && num2 >= num - 1)
				{
					IParameterInfo parameterInfo = parameters[num - 1];
					Type parameterType = parameterInfo.ParameterType;
					Type elementType = parameterType.GetElementType();
					if (parameterType.IsArray && parameterInfo.IsDefined<ParamArrayAttribute>(false))
					{
						if (num2 == num)
						{
							Type type = testCaseParameters.Arguments[num2 - 1].GetType();
							if (!TypeExtensions.GetTypeInfo(parameterType).IsAssignableFrom(TypeExtensions.GetTypeInfo(type)))
							{
								Array array = Array.CreateInstance(elementType, 1);
								array.SetValue(testCaseParameters.Arguments[num2 - 1], 0);
								testCaseParameters.Arguments[num2 - 1] = array;
							}
						}
						else
						{
							object[] array2 = new object[num];
							for (int i = 0; i < num && i < num2; i++)
							{
								array2[i] = testCaseParameters.Arguments[i];
							}
							int num3 = num2 - num + 1;
							Array array = Array.CreateInstance(elementType, num3);
							for (int i = 0; i < num3; i++)
							{
								array.SetValue(testCaseParameters.Arguments[num + i - 1], i);
							}
							array2[num - 1] = array;
							testCaseParameters.Arguments = array2;
							num2 = num;
						}
					}
				}
				if (testCaseParameters.Arguments.Length < num)
				{
					object[] array3 = new object[parameters.Length];
					Array.Copy(testCaseParameters.Arguments, array3, testCaseParameters.Arguments.Length);
					for (int i = testCaseParameters.Arguments.Length; i < parameters.Length; i++)
					{
						if (parameters[i].IsOptional)
						{
							array3[i] = Type.Missing;
							continue;
						}
						if (i < testCaseParameters.Arguments.Length)
						{
							array3[i] = testCaseParameters.Arguments[i];
							continue;
						}
						throw new TargetParameterCountException("Incorrect number of parameters specified for TestCase");
					}
					testCaseParameters.Arguments = array3;
				}
				if (num == 1 && (object)method.GetParameters()[0].ParameterType == typeof(object[]) && (num2 > 1 || (num2 == 1 && (object)testCaseParameters.Arguments[0].GetType() != typeof(object[]))))
				{
					testCaseParameters.Arguments = new object[1] { testCaseParameters.Arguments };
				}
				if (num2 == num)
				{
					PerformSpecialConversions(testCaseParameters.Arguments, parameters);
				}
			}
			catch (Exception exception)
			{
				testCaseParameters = new TestCaseParameters(exception);
			}
			return testCaseParameters;
		}

		private static void PerformSpecialConversions(object[] arglist, IParameterInfo[] parameters)
		{
			for (int i = 0; i < arglist.Length; i++)
			{
				object obj = arglist[i];
				Type parameterType = parameters[i].ParameterType;
				if (obj == null)
				{
					continue;
				}
				if (obj is SpecialValue && (SpecialValue)obj == SpecialValue.Null)
				{
					arglist[i] = null;
				}
				else
				{
					if (parameterType.IsAssignableFrom(obj.GetType()))
					{
						continue;
					}
					if (obj is DBNull)
					{
						arglist[i] = null;
						continue;
					}
					bool flag = false;
					if ((object)parameterType == typeof(short) || (object)parameterType == typeof(byte) || (object)parameterType == typeof(sbyte) || (object)parameterType == typeof(short?) || (object)parameterType == typeof(byte?) || (object)parameterType == typeof(sbyte?) || (object)parameterType == typeof(double?))
					{
						flag = obj is int;
					}
					else if ((object)parameterType == typeof(decimal) || (object)parameterType == typeof(decimal?))
					{
						flag = obj is double || obj is string || obj is int;
					}
					else if ((object)parameterType == typeof(DateTime) || (object)parameterType == typeof(DateTime?))
					{
						flag = obj is string;
					}
					if (flag)
					{
						Type conversionType = ((TypeExtensions.GetTypeInfo(parameterType).IsGenericType && (object)parameterType.GetGenericTypeDefinition() == typeof(Nullable<>)) ? parameterType.GetGenericArguments()[0] : parameterType);
						arglist[i] = Convert.ChangeType(obj, conversionType, CultureInfo.InvariantCulture);
					}
					else if (((object)parameterType == typeof(TimeSpan) || (object)parameterType == typeof(TimeSpan?)) && obj is string)
					{
						arglist[i] = TimeSpan.Parse((string)obj);
					}
				}
			}
		}

		public IEnumerable<TestMethod> BuildFrom(IMethodInfo method, Test suite)
		{
			TestMethod test = new NUnitTestCaseBuilder().BuildTestMethod(method, suite, GetParametersForTestCase(method));
			if (test.RunState != 0 && test.RunState != RunState.Ignored)
			{
				PlatformHelper platformHelper = new PlatformHelper();
				if (!platformHelper.IsPlatformSupported(this))
				{
					test.RunState = RunState.Skipped;
					test.Properties.Add("_SKIPREASON", platformHelper.Reason);
				}
			}
			yield return test;
		}
	}
}
