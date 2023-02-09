using System;
using System.Runtime.Serialization;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework
{
	[Serializable]
	public class InconclusiveException : ResultStateException
	{
		public override ResultState ResultState
		{
			get
			{
				return ResultState.Inconclusive;
			}
		}

		public InconclusiveException(string message)
			: base(message)
		{
		}

		public InconclusiveException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected InconclusiveException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
