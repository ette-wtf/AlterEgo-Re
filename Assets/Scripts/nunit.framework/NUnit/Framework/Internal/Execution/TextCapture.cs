using System.IO;
using System.Text;

namespace NUnit.Framework.Internal.Execution
{
	public class TextCapture : TextWriter
	{
		private TextWriter _defaultWriter;

		public override Encoding Encoding
		{
			get
			{
				return _defaultWriter.Encoding;
			}
		}

		public TextCapture(TextWriter defaultWriter)
		{
			_defaultWriter = defaultWriter;
		}

		public override void Write(char value)
		{
			TestExecutionContext testExecutionContext = TestExecutionContext.GetTestExecutionContext();
			if (testExecutionContext != null && testExecutionContext.CurrentResult != null)
			{
				testExecutionContext.CurrentResult.OutWriter.Write(value);
			}
			else
			{
				_defaultWriter.Write(value);
			}
		}

		public override void Write(string value)
		{
			TestExecutionContext testExecutionContext = TestExecutionContext.GetTestExecutionContext();
			if (testExecutionContext != null && testExecutionContext.CurrentResult != null)
			{
				testExecutionContext.CurrentResult.OutWriter.Write(value);
			}
			else
			{
				_defaultWriter.Write(value);
			}
		}

		public override void WriteLine(string value)
		{
			TestExecutionContext testExecutionContext = TestExecutionContext.GetTestExecutionContext();
			if (testExecutionContext != null && testExecutionContext.CurrentResult != null)
			{
				testExecutionContext.CurrentResult.OutWriter.WriteLine(value);
			}
			else
			{
				_defaultWriter.WriteLine(value);
			}
		}
	}
}
