using System;
using System.Runtime.Serialization;

namespace NUnit.Framework.Internal
{
	[Serializable]
	public class InvalidDataSourceException : Exception
	{
		public InvalidDataSourceException()
		{
		}

		public InvalidDataSourceException(string message)
			: base(message)
		{
		}

		public InvalidDataSourceException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected InvalidDataSourceException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
