using System;
using System.Runtime.Serialization;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework
{
	[Serializable]
	public class AssertionException : ResultStateException
	{
		public override ResultState ResultState
		{
			get
			{
				return ResultState.Failure;
			}
		}

		public AssertionException(string message)
			: base(message)
		{
		}

		public AssertionException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected AssertionException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
