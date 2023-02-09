using System;
using System.IO;
using System.Text;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Execution
{
	public class EventListenerTextWriter : TextWriter
	{
		private TextWriter _defaultWriter;

		private string _streamName;

		public override Encoding Encoding
		{
			get
			{
				return Encoding.Default;
			}
		}

		public EventListenerTextWriter(string streamName, TextWriter defaultWriter)
		{
			_streamName = streamName;
			_defaultWriter = defaultWriter;
		}

		public override void Write(char aChar)
		{
			if (!TrySendToListener(aChar.ToString()))
			{
				_defaultWriter.Write(aChar);
			}
		}

		public override void Write(string aString)
		{
			if (!TrySendToListener(aString))
			{
				_defaultWriter.Write(aString);
			}
		}

		public override void WriteLine(string aString)
		{
			if (!TrySendToListener(aString + Environment.NewLine))
			{
				_defaultWriter.WriteLine(aString);
			}
		}

		private bool TrySendToListener(string text)
		{
			TestExecutionContext testExecutionContext = TestExecutionContext.GetTestExecutionContext();
			if (testExecutionContext == null || testExecutionContext.Listener == null)
			{
				return false;
			}
			string testName = ((testExecutionContext.CurrentTest != null) ? testExecutionContext.CurrentTest.FullName : null);
			testExecutionContext.Listener.TestOutput(new TestOutput(text, _streamName, testName));
			return true;
		}
	}
}
