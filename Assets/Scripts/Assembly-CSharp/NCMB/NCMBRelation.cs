using System;
using System.Collections.Generic;
using NCMB.Internal;

namespace NCMB
{
	public class NCMBRelation<T> where T : NCMBObject
	{
		private NCMBObject _parent;

		private string _key;

		private string _targetClass;

		internal string TargetClass
		{
			get
			{
				return _targetClass;
			}
			set
			{
				_targetClass = value;
			}
		}

		internal NCMBRelation(NCMBObject parent, string key)
		{
			_parent = parent;
			_key = key;
			_targetClass = null;
		}

		internal NCMBRelation(string targetClass)
		{
			_parent = null;
			_key = null;
			_targetClass = targetClass;
		}

		public void Add(T obj)
		{
			_addDuplicationCheck(obj);
			NCMBRelationOperation<T> nCMBRelationOperation = new NCMBRelationOperation<T>(new HashSet<T> { obj }, null);
			_targetClass = nCMBRelationOperation.TargetClass;
			_parent._performOperation(_key, nCMBRelationOperation);
		}

		public void Remove(T obj)
		{
			_removeDuplicationCheck(obj);
			HashSet<T> hashSet = new HashSet<T>();
			hashSet.Add(obj);
			NCMBRelationOperation<T> nCMBRelationOperation = new NCMBRelationOperation<T>(null, hashSet);
			_targetClass = nCMBRelationOperation.TargetClass;
			_parent._performOperation(_key, nCMBRelationOperation);
		}

		private void _removeDuplicationCheck(T obj)
		{
			if (!_parent._currentOperations.ContainsKey(_key) || !(_parent._currentOperations[_key] is NCMBRelationOperation<T>))
			{
				return;
			}
			NCMBRelationOperation<T> nCMBRelationOperation = (NCMBRelationOperation<T>)_parent._currentOperations[_key];
			if (nCMBRelationOperation._relationsToAdd.Count <= 0)
			{
				return;
			}
			bool flag = false;
			foreach (string item in nCMBRelationOperation._relationsToAdd)
			{
				if (item == obj.ObjectId)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				throw new NCMBException(new ArgumentException("Remove objects in a Add Must be the same. Call SaveAsync() to send the data."));
			}
		}

		private void _addDuplicationCheck(T obj)
		{
			if (!_parent._currentOperations.ContainsKey(_key) || !(_parent._currentOperations[_key] is NCMBRelationOperation<T>))
			{
				return;
			}
			NCMBRelationOperation<T> nCMBRelationOperation = (NCMBRelationOperation<T>)_parent._currentOperations[_key];
			if (nCMBRelationOperation._relationsToRemove.Count <= 0)
			{
				return;
			}
			bool flag = false;
			foreach (string item in nCMBRelationOperation._relationsToRemove)
			{
				if (item == obj.ObjectId)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				throw new NCMBException(new ArgumentException("Add objects in a Remove Must be the same. Call SaveAsync() to send the data."));
			}
		}

		internal void _ensureParentAndKey(NCMBObject someParent, string someKey)
		{
			if (_parent == null)
			{
				_parent = someParent;
			}
			if (_key == null)
			{
				_key = someKey;
			}
			if (_parent != someParent)
			{
				throw new NCMBException(new ArgumentException("IInternal error. One NCMBRelation retrieved from two different NCMBObjects."));
			}
			if (!_key.Equals(someKey))
			{
				throw new NCMBException(new ArgumentException("Internal error. One NCMBRelation retrieved from two different keys."));
			}
		}

		internal Dictionary<string, object> _encodeToJSON()
		{
			return new Dictionary<string, object>
			{
				{ "__type", "Relation" },
				{ "className", _targetClass }
			};
		}

		public NCMBQuery<T> GetQuery()
		{
			NCMBQuery<T> nCMBQuery = ((_targetClass != null) ? NCMBQuery<T>.GetQuery(_targetClass) : NCMBQuery<T>.GetQuery(_parent.ClassName));
			nCMBQuery._whereRelatedTo(_parent, _key);
			return nCMBQuery;
		}
	}
}
