using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Security;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;

namespace NUnit.Framework.Api
{
	public class DefaultTestAssemblyBuilder : ITestAssemblyBuilder
	{
		private static Logger log = InternalTrace.GetLogger(typeof(DefaultTestAssemblyBuilder));

		private ISuiteBuilder _defaultSuiteBuilder;

		public DefaultTestAssemblyBuilder()
		{
			_defaultSuiteBuilder = new DefaultSuiteBuilder();
		}

		public ITest Build(Assembly assembly, IDictionary<string, object> options)
		{
			log.Debug("Loading {0} in AppDomain {1}", assembly.FullName, AppDomain.CurrentDomain.FriendlyName);
			string assemblyPath = AssemblyHelper.GetAssemblyPath(assembly);
			return Build(assembly, assemblyPath, options);
		}

		public ITest Build(string assemblyName, IDictionary<string, object> options)
		{
			log.Debug("Loading {0} in AppDomain {1}", assemblyName, AppDomain.CurrentDomain.FriendlyName);
			TestSuite testSuite = null;
			try
			{
				Assembly assembly = AssemblyHelper.Load(assemblyName);
				testSuite = Build(assembly, assemblyName, options);
			}
			catch (Exception ex)
			{
				testSuite = new TestAssembly(assemblyName);
				testSuite.RunState = RunState.NotRunnable;
				testSuite.Properties.Set("_SKIPREASON", ex.Message);
			}
			return testSuite;
		}

		private TestSuite Build(Assembly assembly, string assemblyPath, IDictionary<string, object> options)
		{
			TestSuite testSuite = null;
			try
			{
				if (options.ContainsKey("DefaultTestNamePattern"))
				{
					TestNameGenerator.DefaultTestNamePattern = options["DefaultTestNamePattern"] as string;
				}
				if (options.ContainsKey("TestParameters"))
				{
					string text = options["TestParameters"] as string;
					if (!string.IsNullOrEmpty(text))
					{
						string[] array = text.Split(';');
						foreach (string text2 in array)
						{
							int num = text2.IndexOf("=");
							if (num > 0 && num < text2.Length - 1)
							{
								string name = text2.Substring(0, num);
								string value = text2.Substring(num + 1);
								TestContext.Parameters.Add(name, value);
							}
						}
					}
				}
				IList names = null;
				if (options.ContainsKey("LOAD"))
				{
					names = options["LOAD"] as IList;
				}
				IList<Test> fixtures = GetFixtures(assembly, names);
				testSuite = BuildTestAssembly(assembly, assemblyPath, fixtures);
			}
			catch (Exception ex)
			{
				testSuite = new TestAssembly(assemblyPath);
				testSuite.RunState = RunState.NotRunnable;
				testSuite.Properties.Set("_SKIPREASON", ex.Message);
			}
			return testSuite;
		}

		private IList<Test> GetFixtures(Assembly assembly, IList names)
		{
			List<Test> list = new List<Test>();
			log.Debug("Examining assembly for test fixtures");
			IList<Type> candidateFixtureTypes = GetCandidateFixtureTypes(assembly, names);
			log.Debug("Found {0} classes to examine", candidateFixtureTypes.Count);
			int num = 0;
			foreach (Type item in candidateFixtureTypes)
			{
				TypeWrapper typeInfo = new TypeWrapper(item);
				try
				{
					if (_defaultSuiteBuilder.CanBuildFrom(typeInfo))
					{
						Test test = _defaultSuiteBuilder.BuildFrom(typeInfo);
						list.Add(test);
						num += test.TestCaseCount;
					}
				}
				catch (Exception ex)
				{
					log.Error(ex.ToString());
				}
			}
			log.Debug("Found {0} fixtures with {1} test cases", list.Count, num);
			return list;
		}

		private IList<Type> GetCandidateFixtureTypes(Assembly assembly, IList names)
		{
			Type[] types = assembly.GetTypes();
			if (names == null || names.Count == 0)
			{
				return types;
			}
			List<Type> list = new List<Type>();
			foreach (string name in names)
			{
				Type type = assembly.GetType(name);
				if ((object)type != null)
				{
					list.Add(type);
					continue;
				}
				string value = name + ".";
				Type[] array = types;
				foreach (Type type2 in array)
				{
					if (type2.FullName.StartsWith(value))
					{
						list.Add(type2);
					}
				}
			}
			return list;
		}

		[SecuritySafeCritical]
		private TestSuite BuildTestAssembly(Assembly assembly, string assemblyName, IList<Test> fixtures)
		{
			TestSuite testSuite = new TestAssembly(assembly, assemblyName);
			if (fixtures.Count == 0)
			{
				testSuite.RunState = RunState.NotRunnable;
				testSuite.Properties.Set("_SKIPREASON", "Has no TestFixtures");
			}
			else
			{
				NamespaceTreeBuilder namespaceTreeBuilder = new NamespaceTreeBuilder(testSuite);
				namespaceTreeBuilder.Add(fixtures);
				testSuite = namespaceTreeBuilder.RootSuite;
			}
			testSuite.ApplyAttributesToTest(assembly);
			testSuite.Properties.Set("_PID", Process.GetCurrentProcess().Id);
			testSuite.Properties.Set("_APPDOMAIN", AppDomain.CurrentDomain.FriendlyName);
			testSuite.Sort();
			return testSuite;
		}
	}
}
