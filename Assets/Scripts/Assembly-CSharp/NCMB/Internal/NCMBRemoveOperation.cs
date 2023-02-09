using System;
using System.Collections;
using System.Collections.Generic;

namespace NCMB.Internal
{
	internal class NCMBRemoveOperation : INCMBFieldOperation
	{
		private ArrayList objects = new ArrayList();

		public NCMBRemoveOperation(object values)
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
				{ "__op", "Remove" },
				{
					"objects",
					NCMBUtility._maybeEncodeJSONObject(new ArrayList(objects), true)
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
				return new NCMBSetOperation(new ArrayList());
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
			if (previous is NCMBRemoveOperation)
			{
				ArrayList arrayList = new ArrayList(((NCMBRemoveOperation)previous).objects);
				foreach (object @object in objects)
				{
					arrayList.Add(@object);
				}
				return new NCMBRemoveOperation(arrayList);
			}
			throw new InvalidOperationException("Operation is invalid after previous operation.");
		}

		public object Apply(object oldValue, NCMBObject obj, string key)
		{
			if (oldValue == null)
			{
				return new ArrayList();
			}
			if (oldValue is IList)
			{
				ArrayList arrayList = new ArrayList((IList)oldValue);
				foreach (object @object in objects)
				{
					while (arrayList.Contains(@object))
					{
						arrayList.Remove(@object);
					}
				}
				ArrayList arrayList2 = new ArrayList(objects);
				foreach (object item in arrayList)
				{
					while (arrayList2.Contains(item))
					{
						arrayList2.Remove(item);
					}
				}
				HashSet<object> hashSet = new HashSet<object>();
				{
					foreach (object item2 in arrayList2)
					{
						if (item2 is NCMBObject)
						{
							NCMBObject nCMBObject = (NCMBObject)item2;
							hashSet.Add(nCMBObject.ObjectId);
						}
						for (int i = 0; i < arrayList.Count; i++)
						{
							object obj2 = arrayList[i];
							if (obj2 is NCMBObject)
							{
								NCMBObject nCMBObject2 = (NCMBObject)obj2;
								if (hashSet.Contains(nCMBObject2.ObjectId))
								{
									arrayList.RemoveAt(i);
								}
							}
						}
					}
					return arrayList;
				}
			}
			throw new InvalidOperationException("Operation is invalid after previous operation.");
		}
	}
}
