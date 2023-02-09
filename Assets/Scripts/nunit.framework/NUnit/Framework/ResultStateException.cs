using System;
using System.Runtime.Serialization;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework
{
	[Serializable]
	public abstract class ResultStateException : Exception
	{
		public abstract ResultState ResultState { get; }

		public ResultStateException(string message)
			: base(message)
		{
		}

		public ResultStateException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected ResultStateException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
