using System.IO;
using System.Text;

namespace NUnit.Framework.Internal
{
	public class InternalTraceWriter : TextWriter
	{
		private TextWriter writer;

		private object myLock = new object();

		public override Encoding Encoding
		{
			get
			{
				return writer.Encoding;
			}
		}

		public InternalTraceWriter(string logPath)
		{
			writer = new StreamWriter(new FileStream(logPath, FileMode.Append, FileAccess.Write, FileShare.Write))
			{
				AutoFlush = true
			};
		}

		public InternalTraceWriter(TextWriter writer)
		{
			this.writer = writer;
		}

		public override void Write(char value)
		{
			lock (myLock)
			{
				writer.Write(value);
			}
		}

		public override void Write(string value)
		{
			lock (myLock)
			{
				base.Write(value);
			}
		}

		public override void WriteLine(string value)
		{
			writer.WriteLine(value);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && writer != null)
			{
				writer.Flush();
				writer.Dispose();
				writer = null;
			}
			base.Dispose(disposing);
		}

		public override void Flush()
		{
			if (writer != null)
			{
				writer.Flush();
			}
		}
	}
}
