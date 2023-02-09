using System.Collections.Generic;

namespace NUnit.Framework.Internal.Builders
{
	public class NamespaceTreeBuilder
	{
		private Dictionary<string, TestSuite> namespaceSuites = new Dictionary<string, TestSuite>();

		private TestSuite rootSuite;

		public TestSuite RootSuite
		{
			get
			{
				return rootSuite;
			}
		}

		public NamespaceTreeBuilder(TestSuite rootSuite)
		{
			this.rootSuite = rootSuite;
		}

		public void Add(IList<Test> fixtures)
		{
			foreach (TestSuite fixture in fixtures)
			{
				Add(fixture);
			}
		}

		public void Add(TestSuite fixture)
		{
			string namespaceForFixture = GetNamespaceForFixture(fixture);
			TestSuite testSuite = BuildFromNameSpace(namespaceForFixture);
			if (fixture is SetUpFixture)
			{
				AddSetUpFixture(fixture, testSuite, namespaceForFixture);
			}
			else
			{
				testSuite.Add(fixture);
			}
		}

		private static string GetNamespaceForFixture(TestSuite fixture)
		{
			string text = fixture.FullName;
			int num = text.IndexOfAny(new char[2] { '[', '(' });
			if (num >= 0)
			{
				text = text.Substring(0, num);
			}
			num = text.LastIndexOf('.');
			return (num > 0) ? text.Substring(0, num) : string.Empty;
		}

		private TestSuite BuildFromNameSpace(string ns)
		{
			if (ns == null || ns == "")
			{
				return rootSuite;
			}
			TestSuite testSuite = (namespaceSuites.ContainsKey(ns) ? namespaceSuites[ns] : null);
			if (testSuite != null)
			{
				return testSuite;
			}
			int num = ns.LastIndexOf(".");
			if (num == -1)
			{
				testSuite = new TestSuite(ns);
				if (rootSuite == null)
				{
					rootSuite = testSuite;
				}
				else
				{
					rootSuite.Add(testSuite);
				}
			}
			else
			{
				string text = ns.Substring(0, num);
				TestSuite testSuite2 = BuildFromNameSpace(text);
				string name = ns.Substring(num + 1);
				testSuite = new TestSuite(text, name);
				testSuite2.Add(testSuite);
			}
			namespaceSuites[ns] = testSuite;
			return testSuite;
		}

		private void AddSetUpFixture(TestSuite newSetupFixture, TestSuite containingSuite, string ns)
		{
			foreach (TestSuite test in containingSuite.Tests)
			{
				newSetupFixture.Add(test);
			}
			if (containingSuite is SetUpFixture)
			{
				containingSuite.Tests.Clear();
				containingSuite.Add(newSetupFixture);
			}
			else
			{
				TestSuite testSuite = (TestSuite)containingSuite.Parent;
				if (testSuite == null)
				{
					newSetupFixture.Name = rootSuite.Name;
					rootSuite = newSetupFixture;
				}
				else
				{
					testSuite.Tests.Remove(containingSuite);
					testSuite.Add(newSetupFixture);
				}
			}
			namespaceSuites[ns] = newSetupFixture;
		}
	}
}
