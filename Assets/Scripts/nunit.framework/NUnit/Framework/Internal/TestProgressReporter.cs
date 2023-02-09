using System;
using System.Web.UI;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
	public class TestProgressReporter : ITestListener
	{
		private static Logger log = InternalTrace.GetLogger("TestProgressReporter");

		private ICallbackEventHandler handler;

		public TestProgressReporter(ICallbackEventHandler handler)
		{
			this.handler = handler;
		}

		public void TestStarted(ITest test)
		{
			string text = ((test is TestSuite) ? "start-suite" : "start-test");
			ITest parent = GetParent(test);
			try
			{
				string report = string.Format("<{0} id=\"{1}\" parentId=\"{2}\" name=\"{3}\" fullname=\"{4}\"/>", text, test.Id, (parent != null) ? parent.Id : string.Empty, FormatAttributeValue(test.Name), FormatAttributeValue(test.FullName));
				handler.RaiseCallbackEvent(report);
			}
			catch (Exception ex)
			{
				log.Error("Exception processing " + test.FullName + Env.NewLine + ex.ToString());
			}
		}

		public void TestFinished(ITestResult result)
		{
			try
			{
				TNode tNode = result.ToXml(false);
				ITest parent = GetParent(result.Test);
				tNode.Attributes.Add("parentId", (parent != null) ? parent.Id : string.Empty);
				handler.RaiseCallbackEvent(tNode.OuterXml);
			}
			catch (Exception ex)
			{
				log.Error("Exception processing " + result.FullName + Env.NewLine + ex.ToString());
			}
		}

		public void TestOutput(TestOutput output)
		{
			try
			{
				handler.RaiseCallbackEvent(output.ToXml());
			}
			catch (Exception ex)
			{
				log.Error("Exception processing TestOutput event" + Env.NewLine + ex.ToString());
			}
		}

		private static ITest GetParent(ITest test)
		{
			if (test == null || test.Parent == null)
			{
				return null;
			}
			return test.Parent.IsSuite ? test.Parent : GetParent(test.Parent);
		}

		private static string FormatAttributeValue(string original)
		{
			return original.Replace("&", "&amp;").Replace("\"", "&quot;").Replace("'", "&apos;")
				.Replace("<", "&lt;")
				.Replace(">", "&gt;");
		}
	}
}
