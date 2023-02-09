using System;
using System.Runtime.Serialization;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework
{
	[Serializable]
	public class IgnoreException : ResultStateException
	{
		public override ResultState ResultState
		{
			get
			{
				return ResultState.Ignored;
			}
		}

		public IgnoreException(string message)
			: base(message)
		{
		}

		public IgnoreException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected IgnoreException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
