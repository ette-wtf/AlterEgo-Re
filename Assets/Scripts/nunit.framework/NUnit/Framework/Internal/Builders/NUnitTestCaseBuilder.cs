using System;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Builders
{
	public class NUnitTestCaseBuilder
	{
		private readonly Randomizer _randomizer = Randomizer.CreateRandomizer();

		private readonly TestNameGenerator _nameGenerator;

		public NUnitTestCaseBuilder()
		{
			_nameGenerator = new TestNameGenerator();
		}

		public TestMethod BuildTestMethod(IMethodInfo method, Test parentSuite, TestCaseParameters parms)
		{
			TestMethod testMethod = new TestMethod(method, parentSuite);
			testMethod.Seed = _randomizer.Next();
			TestMethod testMethod2 = testMethod;
			CheckTestMethodSignature(testMethod2, parms);
			if (parms == null || parms.Arguments == null)
			{
				testMethod2.ApplyAttributesToTest(method.MethodInfo);
			}
			string fullName = testMethod2.Method.TypeInfo.FullName;
			if (parentSuite != null)
			{
				fullName = parentSuite.FullName;
			}
			if (parms != null)
			{
				parms.ApplyToTest(testMethod2);
				if (parms.TestName != null)
				{
					testMethod2.Name = (parms.TestName.Contains("{") ? new TestNameGenerator(parms.TestName).GetDisplayName(testMethod2, parms.OriginalArguments) : parms.TestName);
				}
				else
				{
					testMethod2.Name = _nameGenerator.GetDisplayName(testMethod2, parms.OriginalArguments);
				}
			}
			else
			{
				testMethod2.Name = _nameGenerator.GetDisplayName(testMethod2, null);
			}
			testMethod2.FullName = fullName + "." + testMethod2.Name;
			return testMethod2;
		}

		private static bool CheckTestMethodSignature(TestMethod testMethod, TestCaseParameters parms)
		{
			if (testMethod.Method.IsAbstract)
			{
				return MarkAsNotRunnable(testMethod, "Method is abstract");
			}
			if (!testMethod.Method.IsPublic)
			{
				return MarkAsNotRunnable(testMethod, "Method is not public");
			}
			IParameterInfo[] parameters = testMethod.Method.GetParameters();
			int num = 0;
			IParameterInfo[] array = parameters;
			foreach (IParameterInfo parameterInfo in array)
			{
				if (!parameterInfo.IsOptional)
				{
					num++;
				}
			}
			int num2 = parameters.Length;
			object[] array2 = null;
			int num3 = 0;
			if (parms != null)
			{
				testMethod.parms = parms;
				testMethod.RunState = parms.RunState;
				array2 = parms.Arguments;
				if (array2 != null)
				{
					num3 = array2.Length;
				}
				if (testMethod.RunState != RunState.Runnable)
				{
					return false;
				}
			}
			ITypeInfo returnType = testMethod.Method.ReturnType;
			if (returnType.IsType(typeof(void)))
			{
				if (parms != null && parms.HasExpectedResult)
				{
					return MarkAsNotRunnable(testMethod, "Method returning void cannot have an expected result");
				}
			}
			else if (parms == null || !parms.HasExpectedResult)
			{
				return MarkAsNotRunnable(testMethod, "Method has non-void return value, but no result is expected");
			}
			if (num3 > 0 && num2 == 0)
			{
				return MarkAsNotRunnable(testMethod, "Arguments provided for method with no parameters");
			}
			if (num3 == 0 && num > 0)
			{
				return MarkAsNotRunnable(testMethod, "No arguments were provided");
			}
			if (num3 < num)
			{
				return MarkAsNotRunnable(testMethod, string.Format("Not enough arguments provided, provide at least {0} arguments.", num));
			}
			if (num3 > num2)
			{
				return MarkAsNotRunnable(testMethod, string.Format("Too many arguments provided, provide at most {0} arguments.", num2));
			}
			if (testMethod.Method.IsGenericMethodDefinition && array2 != null)
			{
				Type[] typeArguments = new GenericMethodHelper(testMethod.Method.MethodInfo).GetTypeArguments(array2);
				Type[] array3 = typeArguments;
				foreach (Type type in array3)
				{
					if ((object)type == null || (object)type == TypeHelper.NonmatchingType)
					{
						return MarkAsNotRunnable(testMethod, "Unable to determine type arguments for method");
					}
				}
				testMethod.Method = testMethod.Method.MakeGenericMethod(typeArguments);
				parameters = testMethod.Method.GetParameters();
			}
			if (array2 != null && parameters != null)
			{
				TypeHelper.ConvertArgumentList(array2, parameters);
			}
			return true;
		}

		private static bool MarkAsNotRunnable(TestMethod testMethod, string reason)
		{
			testMethod.RunState = RunState.NotRunnable;
			testMethod.Properties.Set("_SKIPREASON", reason);
			return false;
		}
	}
}
