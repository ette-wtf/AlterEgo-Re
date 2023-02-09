using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
	public class TestSuite : Test
	{
		private List<ITest> tests = new List<ITest>();

		public override IList<ITest> Tests
		{
			get
			{
				return tests;
			}
		}

		public override int TestCaseCount
		{
			get
			{
				int num = 0;
				foreach (Test test in Tests)
				{
					num += test.TestCaseCount;
				}
				return num;
			}
		}

		public object[] Arguments { get; internal set; }

		protected bool MaintainTestOrder { get; set; }

		public override bool HasChildren
		{
			get
			{
				return tests.Count > 0;
			}
		}

		public override string XmlElementName
		{
			get
			{
				return "test-suite";
			}
		}

		public TestSuite(string name)
			: base(name)
		{
			Arguments = new object[0];
		}

		public TestSuite(string parentSuiteName, string name)
			: base(parentSuiteName, name)
		{
			Arguments = new object[0];
		}

		public TestSuite(ITypeInfo fixtureType)
			: base(fixtureType)
		{
			Arguments = new object[0];
		}

		public TestSuite(Type fixtureType)
			: base(new TypeWrapper(fixtureType))
		{
			Arguments = new object[0];
		}

		public void Sort()
		{
			if (MaintainTestOrder)
			{
				return;
			}
			tests.Sort();
			foreach (Test test in Tests)
			{
				TestSuite testSuite = test as TestSuite;
				if (testSuite != null)
				{
					testSuite.Sort();
				}
			}
		}

		public void Add(Test test)
		{
			test.Parent = this;
			tests.Add(test);
		}

		public override TestResult MakeTestResult()
		{
			return new TestSuiteResult(this);
		}

		public override TNode AddToXml(TNode parentNode, bool recursive)
		{
			TNode tNode = parentNode.AddElement("test-suite");
			tNode.AddAttribute("type", TestType);
			PopulateTestNode(tNode, recursive);
			tNode.AddAttribute("testcasecount", TestCaseCount.ToString());
			if (recursive)
			{
				foreach (Test test in Tests)
				{
					test.AddToXml(tNode, recursive);
				}
			}
			return tNode;
		}

		protected void CheckSetUpTearDownMethods(Type attrType)
		{
			MethodInfo[] methodsWithAttribute = Reflect.GetMethodsWithAttribute(base.TypeInfo.Type, attrType, true);
			foreach (MethodInfo methodInfo in methodsWithAttribute)
			{
				if (methodInfo.IsAbstract || (!methodInfo.IsPublic && !methodInfo.IsFamily) || methodInfo.GetParameters().Length > 0 || (object)methodInfo.ReturnType != typeof(void))
				{
					base.Properties.Set("_SKIPREASON", string.Format("Invalid signature for SetUp or TearDown method: {0}", methodInfo.Name));
					base.RunState = RunState.NotRunnable;
					break;
				}
			}
		}
	}
}
