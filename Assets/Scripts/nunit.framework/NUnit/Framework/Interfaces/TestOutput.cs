namespace NUnit.Framework.Interfaces
{
	public class TestOutput
	{
		public string Text { get; private set; }

		public string Stream { get; private set; }

		public string TestName { get; private set; }

		public TestOutput(string text, string stream, string testName)
		{
			Text = text;
			Stream = stream;
			TestName = testName;
		}

		public override string ToString()
		{
			return Stream + ": " + Text;
		}

		public string ToXml()
		{
			TNode tNode = new TNode("test-output", Text, true);
			tNode.AddAttribute("stream", Stream);
			if (TestName != null)
			{
				tNode.AddAttribute("testname", TestName);
			}
			return tNode.OuterXml;
		}
	}
}
