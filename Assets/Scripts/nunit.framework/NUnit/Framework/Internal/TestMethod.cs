using System.Collections.Generic;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
	public class TestMethod : Test
	{
		public TestCaseParameters parms;

		internal bool HasExpectedResult
		{
			get
			{
				return parms != null && parms.HasExpectedResult;
			}
		}

		internal object ExpectedResult
		{
			get
			{
				return (parms != null) ? parms.ExpectedResult : null;
			}
		}

		internal object[] Arguments
		{
			get
			{
				return (parms != null) ? parms.Arguments : null;
			}
		}

		public override bool HasChildren
		{
			get
			{
				return false;
			}
		}

		public override IList<ITest> Tests
		{
			get
			{
				return new ITest[0];
			}
		}

		public override string XmlElementName
		{
			get
			{
				return "test-case";
			}
		}

		public override string MethodName
		{
			get
			{
				return base.Method.Name;
			}
		}

		public TestMethod(IMethodInfo method)
			: base(method)
		{
		}

		public TestMethod(IMethodInfo method, Test parentSuite)
			: base(method)
		{
			if (parentSuite != null)
			{
				base.FullName = parentSuite.FullName + "." + base.Name;
			}
		}

		public override TestResult MakeTestResult()
		{
			return new TestCaseResult(this);
		}

		public override TNode AddToXml(TNode parentNode, bool recursive)
		{
			TNode tNode = parentNode.AddElement(XmlElementName);
			PopulateTestNode(tNode, recursive);
			tNode.AddAttribute("seed", base.Seed.ToString());
			return tNode;
		}
	}
}
