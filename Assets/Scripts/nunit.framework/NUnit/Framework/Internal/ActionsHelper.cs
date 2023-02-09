using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Compatibility;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
	public class ActionsHelper
	{
		private enum ActionPhase
		{
			Before = 0,
			After = 1
		}

		public static void ExecuteBeforeActions(IEnumerable<ITestAction> actions, ITest test)
		{
			ExecuteActions(ActionPhase.Before, actions, test);
		}

		public static void ExecuteAfterActions(IEnumerable<ITestAction> actions, ITest test)
		{
			ExecuteActions(ActionPhase.After, actions, test);
		}

		private static void ExecuteActions(ActionPhase phase, IEnumerable<ITestAction> actions, ITest test)
		{
			if (actions == null)
			{
				return;
			}
			ITestAction[] filteredAndSortedActions = GetFilteredAndSortedActions(actions, phase);
			foreach (ITestAction testAction in filteredAndSortedActions)
			{
				if (phase == ActionPhase.Before)
				{
					testAction.BeforeTest(test);
				}
				else
				{
					testAction.AfterTest(test);
				}
			}
		}

		public static ITestAction[] GetActionsFromTestAssembly(TestAssembly testAssembly)
		{
			return GetActionsFromAttributeProvider(testAssembly.Assembly);
		}

		public static ITestAction[] GetActionsFromTestMethodInfo(IMethodInfo testAssembly)
		{
			return GetActionsFromAttributeProvider(testAssembly.MethodInfo);
		}

		public static ITestAction[] GetActionsFromAttributeProvider(ICustomAttributeProvider attributeProvider)
		{
			if (attributeProvider == null)
			{
				return new ITestAction[0];
			}
			List<ITestAction> list = new List<ITestAction>((ITestAction[])attributeProvider.GetCustomAttributes(typeof(ITestAction), false));
			list.Sort(SortByTargetDescending);
			return list.ToArray();
		}

		public static ITestAction[] GetActionsFromTypesAttributes(Type type)
		{
			if ((object)type == null)
			{
				return new ITestAction[0];
			}
			if ((object)type == typeof(object))
			{
				return new ITestAction[0];
			}
			List<ITestAction> list = new List<ITestAction>();
			list.AddRange(GetActionsFromTypesAttributes(TypeExtensions.GetTypeInfo(type).BaseType));
			Type[] declaredInterfaces = GetDeclaredInterfaces(type);
			Type[] array = declaredInterfaces;
			foreach (Type type2 in array)
			{
				list.AddRange(GetActionsFromAttributeProvider(TypeExtensions.GetTypeInfo(type2)));
			}
			list.AddRange(GetActionsFromAttributeProvider(TypeExtensions.GetTypeInfo(type)));
			return list.ToArray();
		}

		private static Type[] GetDeclaredInterfaces(Type type)
		{
			List<Type> list = new List<Type>(type.GetInterfaces());
			if ((object)TypeExtensions.GetTypeInfo(type).BaseType == typeof(object))
			{
				return list.ToArray();
			}
			List<Type> list2 = new List<Type>(TypeExtensions.GetTypeInfo(type).BaseType.GetInterfaces());
			List<Type> list3 = new List<Type>();
			foreach (Type item in list)
			{
				if (!list2.Contains(item))
				{
					list3.Add(item);
				}
			}
			return list3.ToArray();
		}

		private static ITestAction[] GetFilteredAndSortedActions(IEnumerable<ITestAction> actions, ActionPhase phase)
		{
			List<ITestAction> list = new List<ITestAction>();
			foreach (ITestAction action in actions)
			{
				if (!list.Contains(action))
				{
					list.Add(action);
				}
			}
			if (phase == ActionPhase.After)
			{
				list.Reverse();
			}
			return list.ToArray();
		}

		private static int SortByTargetDescending(ITestAction x, ITestAction y)
		{
			return y.Targets.CompareTo(x.Targets);
		}
	}
}
