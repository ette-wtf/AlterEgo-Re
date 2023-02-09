using System.Collections.Generic;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Builders
{
	public class DefaultTestCaseBuilder : ITestCaseBuilder
	{
		private NUnitTestCaseBuilder _nunitTestCaseBuilder = new NUnitTestCaseBuilder();

		public bool CanBuildFrom(IMethodInfo method)
		{
			return method.IsDefined<ITestBuilder>(false) || method.IsDefined<ISimpleTestBuilder>(false);
		}

		public Test BuildFrom(IMethodInfo method)
		{
			return BuildFrom(method, null);
		}

		public bool CanBuildFrom(IMethodInfo method, Test parentSuite)
		{
			return CanBuildFrom(method);
		}

		public Test BuildFrom(IMethodInfo method, Test parentSuite)
		{
			List<TestMethod> list = new List<TestMethod>();
			List<ITestBuilder> list2 = new List<ITestBuilder>(method.GetCustomAttributes<ITestBuilder>(false));
			bool flag = true;
			foreach (ITestBuilder item in list2)
			{
				if (item is CombiningStrategyAttribute)
				{
					flag = false;
				}
			}
			if (flag)
			{
				list2.Add(new CombinatorialAttribute());
			}
			foreach (ITestBuilder item2 in list2)
			{
				foreach (TestMethod item3 in item2.BuildFrom(method, parentSuite))
				{
					list.Add(item3);
				}
			}
			return (list.Count > 0) ? BuildParameterizedMethodSuite(method, list) : BuildSingleTestMethod(method, parentSuite);
		}

		private Test BuildParameterizedMethodSuite(IMethodInfo method, IEnumerable<TestMethod> tests)
		{
			ParameterizedMethodSuite parameterizedMethodSuite = new ParameterizedMethodSuite(method);
			parameterizedMethodSuite.ApplyAttributesToTest(method.MethodInfo);
			foreach (TestMethod test in tests)
			{
				parameterizedMethodSuite.Add(test);
			}
			return parameterizedMethodSuite;
		}

		private Test BuildSingleTestMethod(IMethodInfo method, Test suite)
		{
			ISimpleTestBuilder[] customAttributes = method.GetCustomAttributes<ISimpleTestBuilder>(false);
			return (customAttributes.Length > 0) ? customAttributes[0].BuildFrom(method, suite) : _nunitTestCaseBuilder.BuildTestMethod(method, suite, null);
		}
	}
}
