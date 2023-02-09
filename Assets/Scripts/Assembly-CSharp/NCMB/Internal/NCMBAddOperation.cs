using System;
using System.Collections;
using System.Collections.Generic;

namespace NCMB.Internal
{
	internal class NCMBAddOperation : INCMBFieldOperation
	{
		private ArrayList objects = new ArrayList();

		public NCMBAddOperation(object values)
		{
			if (values is IEnumerable)
			{
				IEnumerator enumerator = ((IEnumerable)values).GetEnumerator();
				while (enumerator.MoveNext())
				{
					object current = enumerator.Current;
					objects.Add(current);
				}
			}
			else
			{
				objects.Add(values);
			}
		}

		public object Encode()
		{
			return new Dictionary<string, object>
			{
				{ "__op", "Add" },
				{
					"objects",
					NCMBUtility._maybeEncodeJSONObject(objects, true)
				}
			};
		}

		public INCMBFieldOperation MergeWithPrevious(INCMBFieldOperation previous)
		{
			if (previous == null)
			{
				return this;
			}
			if (previous is NCMBDeleteOperation)
			{
				return new NCMBSetOperation(objects);
			}
			if (previous is NCMBSetOperation)
			{
				object value = ((NCMBSetOperation)previous).getValue();
				if (value is IList)
				{
					ArrayList arrayList = new ArrayList((IList)value);
					arrayList.AddRange(objects);
					return new NCMBSetOperation(arrayList);
				}
				throw new InvalidOperationException("You can only add an item to a List.");
			}
			if (previous is NCMBAddOperation)
			{
				ArrayList arrayList2 = new ArrayList(((NCMBAddOperation)previous).objects);
				arrayList2.AddRange(objects);
				return new NCMBAddOperation(arrayList2);
			}
			throw new InvalidOperationException("Operation is invalid after previous operation.");
		}

		public object Apply(object oldValue, NCMBObject obj, string key)
		{
			if (oldValue == null)
			{
				return objects;
			}
			if (oldValue is IList)
			{
				ArrayList arrayList = new ArrayList((IList)oldValue);
				arrayList.AddRange(objects);
				return arrayList;
			}
			throw new InvalidOperationException("Operation is invalid after previous operation.");
		}
	}
}
