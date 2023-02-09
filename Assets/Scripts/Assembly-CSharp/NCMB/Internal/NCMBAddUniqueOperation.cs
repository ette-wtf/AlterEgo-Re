using System;
using System.Collections;
using System.Collections.Generic;

namespace NCMB.Internal
{
	internal class NCMBAddUniqueOperation : INCMBFieldOperation
	{
		private ArrayList objects = new ArrayList();

		public NCMBAddUniqueOperation(object values)
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
				{ "__op", "AddUnique" },
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
					return new NCMBSetOperation(Apply(value, null, null));
				}
				throw new InvalidOperationException("You can only add an item to a List.");
			}
			if (previous is NCMBAddUniqueOperation)
			{
				IList oldValue = new ArrayList(((NCMBAddUniqueOperation)previous).objects);
				return new NCMBAddUniqueOperation((IList)Apply(oldValue, null, null));
			}
			throw new InvalidOperationException("Operation is invalid after previous operation.");
		}

		public object Apply(object oldValue, NCMBObject obj, string key)
		{
			if (oldValue == null)
			{
				return new ArrayList(objects);
			}
			if (oldValue is IList)
			{
				ArrayList arrayList = new ArrayList((IList)oldValue);
				Hashtable hashtable = new Hashtable();
				foreach (object item in arrayList)
				{
					int num = 0;
					if (item is NCMBObject)
					{
						NCMBObject nCMBObject = (NCMBObject)item;
						hashtable.Add(nCMBObject.ObjectId, num);
					}
				}
				IEnumerator enumerator2 = objects.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					object current2 = enumerator2.Current;
					if (current2 is NCMBObject)
					{
						NCMBObject nCMBObject2 = (NCMBObject)current2;
						if (hashtable.ContainsKey(nCMBObject2.ObjectId))
						{
							int index = Convert.ToInt32(hashtable[nCMBObject2.ObjectId]);
							arrayList.Insert(index, current2);
						}
						else
						{
							arrayList.Add(current2);
						}
					}
					else if (!arrayList.Contains(current2))
					{
						arrayList.Add(current2);
					}
				}
				return arrayList;
			}
			throw new InvalidOperationException("Operation is invalid after previous operation.");
		}
	}
}
