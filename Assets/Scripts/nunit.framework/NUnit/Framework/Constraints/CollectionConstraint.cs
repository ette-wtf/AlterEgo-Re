using System;
using System.Collections;

namespace NUnit.Framework.Constraints
{
	public abstract class CollectionConstraint : Constraint
	{
		protected CollectionConstraint()
		{
		}

		protected CollectionConstraint(object arg)
			: base(arg)
		{
		}

		protected static bool IsEmpty(IEnumerable enumerable)
		{
			ICollection collection = enumerable as ICollection;
			if (collection != null)
			{
				return collection.Count == 0;
			}
			IEnumerator enumerator = enumerable.GetEnumerator();
			try
			{
				if (enumerator.MoveNext())
				{
					object current = enumerator.Current;
					return false;
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			return true;
		}

		public override ConstraintResult ApplyTo(object actual)
		{
			IEnumerable enumerable = actual as IEnumerable;
			if (enumerable == null)
			{
				throw new ArgumentException("The actual value must be an IEnumerable", "actual");
			}
			return new ConstraintResult(this, actual, Matches(enumerable));
		}

		protected abstract bool Matches(IEnumerable collection);
	}
}
