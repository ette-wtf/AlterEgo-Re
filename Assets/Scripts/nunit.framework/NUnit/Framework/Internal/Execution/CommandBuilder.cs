using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Compatibility;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Commands;

namespace NUnit.Framework.Internal.Execution
{
	public static class CommandBuilder
	{
		public static TestCommand MakeOneTimeSetUpCommand(TestSuite suite, List<SetUpTearDownItem> setUpTearDown, List<TestActionItem> actions)
		{
			if (suite.RunState != RunState.Runnable && suite.RunState != RunState.Explicit)
			{
				return MakeSkipCommand(suite);
			}
			TestCommand testCommand = new OneTimeSetUpCommand(suite, setUpTearDown, actions);
			IList<IApplyToContext> list = null;
			if (suite.TypeInfo != null)
			{
				list = suite.TypeInfo.GetCustomAttributes<IApplyToContext>(true);
			}
			else if (suite.Method != null)
			{
				list = suite.Method.GetCustomAttributes<IApplyToContext>(true);
			}
			else
			{
				TestAssembly testAssembly = suite as TestAssembly;
				if (testAssembly != null)
				{
					list = (IApplyToContext[])testAssembly.Assembly.GetCustomAttributes(typeof(IApplyToContext), true);
				}
			}
			if (list != null && list.Count > 0)
			{
				testCommand = new ApplyChangesToContextCommand(testCommand, list);
			}
			return testCommand;
		}

		public static TestCommand MakeOneTimeTearDownCommand(TestSuite suite, List<SetUpTearDownItem> setUpTearDownItems, List<TestActionItem> actions)
		{
			TestCommand testCommand = new OneTimeTearDownCommand(suite, setUpTearDownItems, actions);
			if (suite.TestType == "Theory")
			{
				testCommand = new TheoryResultCommand(testCommand);
			}
			return testCommand;
		}

		public static TestCommand MakeTestCommand(TestMethod test)
		{
			TestCommand testCommand = new TestMethodCommand(test);
			IWrapTestMethod[] customAttributes = test.Method.GetCustomAttributes<IWrapTestMethod>(true);
			foreach (IWrapTestMethod wrapTestMethod in customAttributes)
			{
				testCommand = wrapTestMethod.Wrap(testCommand);
			}
			testCommand = new TestActionCommand(testCommand);
			testCommand = new SetUpTearDownCommand(testCommand);
			IWrapSetUpTearDown[] customAttributes2 = test.Method.GetCustomAttributes<IWrapSetUpTearDown>(true);
			foreach (ICommandWrapper commandWrapper in customAttributes2)
			{
				testCommand = commandWrapper.Wrap(testCommand);
			}
			IApplyToContext[] customAttributes3 = test.Method.GetCustomAttributes<IApplyToContext>(true);
			if (customAttributes3.Length > 0)
			{
				testCommand = new ApplyChangesToContextCommand(testCommand, customAttributes3);
			}
			return testCommand;
		}

		public static SkipCommand MakeSkipCommand(Test test)
		{
			return new SkipCommand(test);
		}

		public static List<SetUpTearDownItem> BuildSetUpTearDownList(Type fixtureType, Type setUpType, Type tearDownType)
		{
			MethodInfo[] methodsWithAttribute = Reflect.GetMethodsWithAttribute(fixtureType, setUpType, true);
			MethodInfo[] methodsWithAttribute2 = Reflect.GetMethodsWithAttribute(fixtureType, tearDownType, true);
			List<SetUpTearDownItem> list = new List<SetUpTearDownItem>();
			while ((object)fixtureType != null && !fixtureType.Equals(typeof(object)))
			{
				SetUpTearDownItem setUpTearDownItem = BuildNode(fixtureType, methodsWithAttribute, methodsWithAttribute2);
				if (setUpTearDownItem.HasMethods)
				{
					list.Add(setUpTearDownItem);
				}
				fixtureType = TypeExtensions.GetTypeInfo(fixtureType).BaseType;
			}
			return list;
		}

		private static SetUpTearDownItem BuildNode(Type fixtureType, IList<MethodInfo> setUpMethods, IList<MethodInfo> tearDownMethods)
		{
			List<MethodInfo> setUpMethods2 = SelectMethodsByDeclaringType(fixtureType, setUpMethods);
			List<MethodInfo> tearDownMethods2 = SelectMethodsByDeclaringType(fixtureType, tearDownMethods);
			return new SetUpTearDownItem(setUpMethods2, tearDownMethods2);
		}

		private static List<MethodInfo> SelectMethodsByDeclaringType(Type type, IList<MethodInfo> methods)
		{
			List<MethodInfo> list = new List<MethodInfo>();
			foreach (MethodInfo method in methods)
			{
				if ((object)method.DeclaringType == type)
				{
					list.Add(method);
				}
			}
			return list;
		}
	}
}
