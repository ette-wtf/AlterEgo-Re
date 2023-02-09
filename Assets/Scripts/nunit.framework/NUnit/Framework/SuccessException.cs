using System;
using System.Runtime.Serialization;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework
{
	[Serializable]
	public class SuccessException : ResultStateException
	{
		public override ResultState ResultState
		{
			get
			{
				return ResultState.Success;
			}
		}

		public SuccessException(string message)
			: base(message)
		{
		}

		public SuccessException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected SuccessException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
