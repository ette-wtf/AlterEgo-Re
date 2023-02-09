using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace NUnit.Framework.Constraints
{
	public class BinarySerializableConstraint : Constraint
	{
		private readonly BinaryFormatter serializer = new BinaryFormatter();

		public override string Description
		{
			get
			{
				return "binary serializable";
			}
		}

		public override ConstraintResult ApplyTo(object actual)
		{
			if (actual == null)
			{
				throw new ArgumentNullException("actual");
			}
			MemoryStream memoryStream = new MemoryStream();
			bool isSuccess = false;
			try
			{
				serializer.Serialize(memoryStream, actual);
				memoryStream.Seek(0L, SeekOrigin.Begin);
				isSuccess = serializer.Deserialize(memoryStream) != null;
			}
			catch (SerializationException)
			{
			}
			return new ConstraintResult(this, actual.GetType(), isSuccess);
		}

		protected override string GetStringRepresentation()
		{
			return "<binaryserializable>";
		}
	}
}
