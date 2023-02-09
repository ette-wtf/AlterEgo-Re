using System;
using System.Collections.Generic;
using System.Reflection;

namespace NUnit.Framework.Internal.Commands
{
	public class SetUpTearDownItem
	{
		private IList<MethodInfo> _setUpMethods;

		private IList<MethodInfo> _tearDownMethods;

		private bool _setUpWasRun;

		public bool HasMethods
		{
			get
			{
				return _setUpMethods.Count > 0 || _tearDownMethods.Count > 0;
			}
		}

		public SetUpTearDownItem(IList<MethodInfo> setUpMethods, IList<MethodInfo> tearDownMethods)
		{
			_setUpMethods = setUpMethods;
			_tearDownMethods = tearDownMethods;
		}

		public void RunSetUp(ITestExecutionContext context)
		{
			_setUpWasRun = true;
			foreach (MethodInfo setUpMethod in _setUpMethods)
			{
				RunSetUpOrTearDownMethod(context, setUpMethod);
			}
		}

		public void RunTearDown(ITestExecutionContext context)
		{
			if (!_setUpWasRun)
			{
				return;
			}
			try
			{
				int num = _tearDownMethods.Count;
				while (--num >= 0)
				{
					RunSetUpOrTearDownMethod(context, _tearDownMethods[num]);
				}
			}
			catch (Exception ex)
			{
				context.CurrentResult.RecordTearDownException(ex);
			}
		}

		private void RunSetUpOrTearDownMethod(ITestExecutionContext context, MethodInfo method)
		{
			RunNonAsyncMethod(method, context);
		}

		private object RunNonAsyncMethod(MethodInfo method, ITestExecutionContext context)
		{
			return Reflect.InvokeMethod(method, method.IsStatic ? null : context.TestObject);
		}
	}
}
