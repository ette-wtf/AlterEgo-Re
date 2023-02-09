using System.Collections;

namespace NUnit.Framework.Constraints
{
	public class EmptyCollectionConstraint : CollectionConstraint
	{
		public override string Description
		{
			get
			{
				return "<empty>";
			}
		}

		protected override bool Matches(IEnumerable collection)
		{
			return CollectionConstraint.IsEmpty(collection);
		}
	}
}
