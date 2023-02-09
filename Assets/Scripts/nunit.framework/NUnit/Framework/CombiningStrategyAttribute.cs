using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public abstract class CombiningStrategyAttribute : NUnitAttribute, ITestBuilder, IApplyToTest
	{
		private NUnitTestCaseBuilder _builder = new NUnitTestCaseBuilder();

		private ICombiningStrategy _strategy;

		private IParameterDataProvider _dataProvider;

		protected CombiningStrategyAttribute(ICombiningStrategy strategy, IParameterDataProvider provider)
		{
			_strategy = strategy;
			_dataProvider = provider;
		}

		protected CombiningStrategyAttribute(object strategy, object provider)
			: this((ICombiningStrategy)strategy, (IParameterDataProvider)provider)
		{
		}

		public IEnumerable<TestMethod> BuildFrom(IMethodInfo method, Test suite)
		{
			List<TestMethod> list = new List<TestMethod>();
			IParameterInfo[] parameters = method.GetParameters();
			if (parameters.Length > 0)
			{
				IEnumerable[] array = new IEnumerable[parameters.Length];
				try
				{
					for (int i = 0; i < parameters.Length; i++)
					{
						array[i] = _dataProvider.GetDataFor(parameters[i]);
					}
				}
				catch (InvalidDataSourceException ex)
				{
					TestCaseParameters testCaseParameters = new TestCaseParameters();
					testCaseParameters.RunState = RunState.NotRunnable;
					testCaseParameters.Properties.Set("_SKIPREASON", ex.Message);
					list.Add(_builder.BuildTestMethod(method, suite, testCaseParameters));
					return list;
				}
				foreach (ITestCaseData testCase in _strategy.GetTestCases(array))
				{
					list.Add(_builder.BuildTestMethod(method, suite, (TestCaseParameters)testCase));
				}
			}
			return list;
		}

		public void ApplyToTest(Test test)
		{
			string text = _strategy.GetType().Name;
			if (text.EndsWith("Strategy"))
			{
				text = text.Substring(0, text.Length - 8);
			}
			test.Properties.Set("_JOINTYPE", text);
		}
	}
}
