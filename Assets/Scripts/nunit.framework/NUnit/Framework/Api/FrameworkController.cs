using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Web.UI;
using NUnit.Compatibility;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Api
{
	public class FrameworkController : LongLivedMarshalByRefObject
	{
		private class ActionCallback : ICallbackEventHandler
		{
			private Action<string> _callback;

			public ActionCallback(Action<string> callback)
			{
				_callback = callback;
			}

			public string GetCallbackResult()
			{
				throw new NotImplementedException();
			}

			public void RaiseCallbackEvent(string report)
			{
				if (_callback != null)
				{
					_callback(report);
				}
			}
		}

		public abstract class FrameworkControllerAction : LongLivedMarshalByRefObject
		{
		}

		public class LoadTestsAction : FrameworkControllerAction
		{
			public LoadTestsAction(FrameworkController controller, object handler)
			{
				controller.LoadTests((ICallbackEventHandler)handler);
			}
		}

		public class ExploreTestsAction : FrameworkControllerAction
		{
			public ExploreTestsAction(FrameworkController controller, string filter, object handler)
			{
				controller.ExploreTests((ICallbackEventHandler)handler, filter);
			}
		}

		public class CountTestsAction : FrameworkControllerAction
		{
			public CountTestsAction(FrameworkController controller, string filter, object handler)
			{
				controller.CountTests((ICallbackEventHandler)handler, filter);
			}
		}

		public class RunTestsAction : FrameworkControllerAction
		{
			public RunTestsAction(FrameworkController controller, string filter, object handler)
			{
				controller.RunTests((ICallbackEventHandler)handler, filter);
			}
		}

		public class RunAsyncAction : FrameworkControllerAction
		{
			public RunAsyncAction(FrameworkController controller, string filter, object handler)
			{
				controller.RunAsync((ICallbackEventHandler)handler, filter);
			}
		}

		public class StopRunAction : FrameworkControllerAction
		{
			public StopRunAction(FrameworkController controller, bool force, object handler)
			{
				controller.StopRun((ICallbackEventHandler)handler, force);
			}
		}

		private const string LOG_FILE_FORMAT = "InternalTrace.{0}.{1}.log";

		private Assembly _testAssembly;

		public ITestAssemblyBuilder Builder { get; private set; }

		public ITestAssemblyRunner Runner { get; private set; }

		public string AssemblyNameOrPath { get; private set; }

		public Assembly Assembly { get; private set; }

		internal IDictionary<string, object> Settings { get; private set; }

		public FrameworkController(string assemblyNameOrPath, string idPrefix, IDictionary settings)
		{
			Initialize(assemblyNameOrPath, settings);
			Builder = new DefaultTestAssemblyBuilder();
			Runner = new NUnitTestAssemblyRunner(Builder);
			Test.IdPrefix = idPrefix;
		}

		public FrameworkController(Assembly assembly, string idPrefix, IDictionary settings)
			: this(assembly.FullName, idPrefix, settings)
		{
			_testAssembly = assembly;
		}

		public FrameworkController(string assemblyNameOrPath, string idPrefix, IDictionary settings, string runnerType, string builderType)
		{
			Initialize(assemblyNameOrPath, settings);
			Builder = (ITestAssemblyBuilder)Reflect.Construct(Type.GetType(builderType));
			Runner = (ITestAssemblyRunner)Reflect.Construct(Type.GetType(runnerType), new object[1] { Builder });
			Test.IdPrefix = idPrefix ?? "";
		}

		public FrameworkController(Assembly assembly, string idPrefix, IDictionary settings, string runnerType, string builderType)
			: this(assembly.FullName, idPrefix, settings, runnerType, builderType)
		{
			_testAssembly = assembly;
		}

		[SecuritySafeCritical]
		private void Initialize(string assemblyPath, IDictionary settings)
		{
			AssemblyNameOrPath = assemblyPath;
			IDictionary<string, object> dictionary = settings as IDictionary<string, object>;
			Settings = dictionary ?? settings.Cast<DictionaryEntry>().ToDictionary((DictionaryEntry de) => (string)de.Key, (DictionaryEntry de) => de.Value);
			if (Settings.ContainsKey("InternalTraceLevel"))
			{
				InternalTraceLevel level = (InternalTraceLevel)Enum.Parse(typeof(InternalTraceLevel), (string)Settings["InternalTraceLevel"], true);
				if (Settings.ContainsKey("InternalTraceWriter"))
				{
					InternalTrace.Initialize((TextWriter)Settings["InternalTraceWriter"], level);
					return;
				}
				string path = (Settings.ContainsKey("WorkDirectory") ? ((string)Settings["WorkDirectory"]) : Env.DefaultWorkDirectory);
				string path2 = string.Format("InternalTrace.{0}.{1}.log", Process.GetCurrentProcess().Id, Path.GetFileName(assemblyPath));
				InternalTrace.Initialize(Path.Combine(path, path2), level);
			}
		}

		public string LoadTests()
		{
			if ((object)_testAssembly != null)
			{
				Runner.Load(_testAssembly, Settings);
			}
			else
			{
				Runner.Load(AssemblyNameOrPath, Settings);
			}
			return Runner.LoadedTest.ToXml(false).OuterXml;
		}

		public string ExploreTests(string filter)
		{
			Guard.ArgumentNotNull(filter, "filter");
			if (Runner.LoadedTest == null)
			{
				throw new InvalidOperationException("The Explore method was called but no test has been loaded");
			}
			return Runner.LoadedTest.ToXml(true).OuterXml;
		}

		public string RunTests(string filter)
		{
			Guard.ArgumentNotNull(filter, "filter");
			TNode tNode = Runner.Run(new TestProgressReporter(null), TestFilter.FromXml(filter)).ToXml(true);
			if (Settings != null)
			{
				InsertSettingsElement(tNode, Settings);
			}
			InsertEnvironmentElement(tNode);
			TestExecutionContext.ClearCurrentContext();
			return tNode.OuterXml;
		}

		public string RunTests(Action<string> callback, string filter)
		{
			Guard.ArgumentNotNull(filter, "filter");
			ActionCallback handler = new ActionCallback(callback);
			TNode tNode = Runner.Run(new TestProgressReporter(handler), TestFilter.FromXml(filter)).ToXml(true);
			if (Settings != null)
			{
				InsertSettingsElement(tNode, Settings);
			}
			InsertEnvironmentElement(tNode);
			TestExecutionContext.ClearCurrentContext();
			return tNode.OuterXml;
		}

		private void RunAsync(Action<string> callback, string filter)
		{
			Guard.ArgumentNotNull(filter, "filter");
			ActionCallback handler = new ActionCallback(callback);
			Runner.RunAsync(new TestProgressReporter(handler), TestFilter.FromXml(filter));
		}

		public void StopRun(bool force)
		{
			Runner.StopRun(force);
		}

		public int CountTests(string filter)
		{
			Guard.ArgumentNotNull(filter, "filter");
			return Runner.CountTestCases(TestFilter.FromXml(filter));
		}

		private void LoadTests(ICallbackEventHandler handler)
		{
			handler.RaiseCallbackEvent(LoadTests());
		}

		private void ExploreTests(ICallbackEventHandler handler, string filter)
		{
			Guard.ArgumentNotNull(filter, "filter");
			if (Runner.LoadedTest == null)
			{
				throw new InvalidOperationException("The Explore method was called but no test has been loaded");
			}
			handler.RaiseCallbackEvent(Runner.LoadedTest.ToXml(true).OuterXml);
		}

		private void RunTests(ICallbackEventHandler handler, string filter)
		{
			Guard.ArgumentNotNull(filter, "filter");
			TNode tNode = Runner.Run(new TestProgressReporter(handler), TestFilter.FromXml(filter)).ToXml(true);
			if (Settings != null)
			{
				InsertSettingsElement(tNode, Settings);
			}
			InsertEnvironmentElement(tNode);
			TestExecutionContext.ClearCurrentContext();
			handler.RaiseCallbackEvent(tNode.OuterXml);
		}

		private void RunAsync(ICallbackEventHandler handler, string filter)
		{
			Guard.ArgumentNotNull(filter, "filter");
			Runner.RunAsync(new TestProgressReporter(handler), TestFilter.FromXml(filter));
		}

		private void StopRun(ICallbackEventHandler handler, bool force)
		{
			StopRun(force);
		}

		private void CountTests(ICallbackEventHandler handler, string filter)
		{
			handler.RaiseCallbackEvent(CountTests(filter).ToString());
		}

		public static TNode InsertEnvironmentElement(TNode targetNode)
		{
			TNode tNode = new TNode("environment");
			targetNode.ChildNodes.Insert(0, tNode);
			tNode.AddAttribute("framework-version", Assembly.GetExecutingAssembly().GetName().Version.ToString());
			tNode.AddAttribute("clr-version", Environment.Version.ToString());
			tNode.AddAttribute("os-version", Environment.OSVersion.ToString());
			tNode.AddAttribute("platform", Environment.OSVersion.Platform.ToString());
			tNode.AddAttribute("cwd", Environment.CurrentDirectory);
			tNode.AddAttribute("machine-name", Environment.MachineName);
			tNode.AddAttribute("user", Environment.UserName);
			tNode.AddAttribute("user-domain", Environment.UserDomainName);
			tNode.AddAttribute("culture", CultureInfo.CurrentCulture.ToString());
			tNode.AddAttribute("uiculture", CultureInfo.CurrentUICulture.ToString());
			tNode.AddAttribute("os-architecture", GetProcessorArchitecture());
			return tNode;
		}

		private static string GetProcessorArchitecture()
		{
			return (IntPtr.Size == 8) ? "x64" : "x86";
		}

		public static TNode InsertSettingsElement(TNode targetNode, IDictionary<string, object> settings)
		{
			TNode tNode = new TNode("settings");
			targetNode.ChildNodes.Insert(0, tNode);
			foreach (string key in settings.Keys)
			{
				AddSetting(tNode, key, settings[key]);
			}
			return tNode;
		}

		private static void AddSetting(TNode settingsNode, string name, object value)
		{
			TNode tNode = new TNode("setting");
			tNode.AddAttribute("name", name);
			tNode.AddAttribute("value", value.ToString());
			settingsNode.ChildNodes.Add(tNode);
		}
	}
}
