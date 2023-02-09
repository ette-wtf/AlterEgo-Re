using System;
using System.IO;

namespace NUnit.Framework.Internal
{
	public static class InternalTrace
	{
		private static InternalTraceLevel traceLevel;

		private static InternalTraceWriter traceWriter;

		public static bool Initialized { get; private set; }

		public static void Initialize(string logName, InternalTraceLevel level)
		{
			if (!Initialized)
			{
				traceLevel = level;
				if (traceWriter == null && traceLevel > InternalTraceLevel.Off)
				{
					traceWriter = new InternalTraceWriter(logName);
					traceWriter.WriteLine("InternalTrace: Initializing at level {0}", traceLevel);
				}
				Initialized = true;
			}
			else
			{
				traceWriter.WriteLine("InternalTrace: Ignoring attempted re-initialization at level {0}", level);
			}
		}

		public static void Initialize(TextWriter writer, InternalTraceLevel level)
		{
			if (!Initialized)
			{
				traceLevel = level;
				if (traceWriter == null && traceLevel > InternalTraceLevel.Off)
				{
					traceWriter = new InternalTraceWriter(writer);
					traceWriter.WriteLine("InternalTrace: Initializing at level " + traceLevel);
				}
				Initialized = true;
			}
		}

		public static Logger GetLogger(string name)
		{
			return new Logger(name, traceLevel, traceWriter);
		}

		public static Logger GetLogger(Type type)
		{
			return GetLogger(type.FullName);
		}
	}
}
