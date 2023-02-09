using System;
using System.IO;
using System.Xml.Serialization;

namespace NUnit.Framework.Constraints
{
	public class XmlSerializableConstraint : Constraint
	{
		private XmlSerializer serializer;

		public override string Description
		{
			get
			{
				return "xml serializable";
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
				serializer = new XmlSerializer(actual.GetType());
				serializer.Serialize(memoryStream, actual);
				memoryStream.Seek(0L, SeekOrigin.Begin);
				isSuccess = serializer.Deserialize(memoryStream) != null;
			}
			catch (NotSupportedException)
			{
			}
			catch (InvalidOperationException)
			{
			}
			return new ConstraintResult(this, actual.GetType(), isSuccess);
		}

		protected override string GetStringRepresentation()
		{
			return "<xmlserializable>";
		}
	}
}
