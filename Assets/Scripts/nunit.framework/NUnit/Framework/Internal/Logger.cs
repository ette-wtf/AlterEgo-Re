using System;
using System.IO;
using System.Threading;

namespace NUnit.Framework.Internal
{
	public class Logger : ILogger
	{
		private static readonly string TIME_FMT = "HH:mm:ss.fff";

		private static readonly string TRACE_FMT = "{0} {1,-5} [{2,2}] {3}: {4}";

		private string name;

		private string fullname;

		private InternalTraceLevel maxLevel;

		private TextWriter writer;

		public Logger(string name, InternalTraceLevel level, TextWriter writer)
		{
			maxLevel = level;
			this.writer = writer;
			fullname = (this.name = name);
			int num = fullname.LastIndexOf('.');
			if (num >= 0)
			{
				this.name = fullname.Substring(num + 1);
			}
		}

		public void Error(string message)
		{
			Log(InternalTraceLevel.Error, message);
		}

		public void Error(string message, params object[] args)
		{
			Log(InternalTraceLevel.Error, message, args);
		}

		public void Warning(string message)
		{
			Log(InternalTraceLevel.Warning, message);
		}

		public void Warning(string message, params object[] args)
		{
			Log(InternalTraceLevel.Warning, message, args);
		}

		public void Info(string message)
		{
			Log(InternalTraceLevel.Info, message);
		}

		public void Info(string message, params object[] args)
		{
			Log(InternalTraceLevel.Info, message, args);
		}

		public void Debug(string message)
		{
			Log(InternalTraceLevel.Debug, message);
		}

		public void Debug(string message, params object[] args)
		{
			Log(InternalTraceLevel.Debug, message, args);
		}

		private void Log(InternalTraceLevel level, string message)
		{
			if (writer != null && maxLevel >= level)
			{
				WriteLog(level, message);
			}
		}

		private void Log(InternalTraceLevel level, string format, params object[] args)
		{
			if (maxLevel >= level)
			{
				WriteLog(level, string.Format(format, args));
			}
		}

		private void WriteLog(InternalTraceLevel level, string message)
		{
			writer.WriteLine(TRACE_FMT, DateTime.Now.ToString(TIME_FMT), (level == InternalTraceLevel.Debug) ? "Debug" : level.ToString(), Thread.CurrentThread.ManagedThreadId, name, message);
		}
	}
}
