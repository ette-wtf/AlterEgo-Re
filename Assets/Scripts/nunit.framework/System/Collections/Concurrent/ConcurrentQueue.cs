using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace System.Collections.Concurrent
{
	[DebuggerDisplay("Count={Count}")]
	[DebuggerTypeProxy(typeof(CollectionDebuggerView<>))]
	internal class ConcurrentQueue<T> : IProducerConsumerCollection<T>, IEnumerable<T>, ICollection, IEnumerable
	{
		private class Node
		{
			public T Value;

			public Node Next;
		}

		private Node head = new Node();

		private Node tail;

		private int count;

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public int Count
		{
			get
			{
				return count;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return count == 0;
			}
		}

		public ConcurrentQueue()
		{
			tail = head;
		}

		public ConcurrentQueue(IEnumerable<T> collection)
			: this()
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			foreach (T item in collection)
			{
				Enqueue(item);
			}
		}

		public void Enqueue(T item)
		{
			Node node = new Node();
			node.Value = item;
			Node node2 = null;
			Node node3 = null;
			bool flag = false;
			while (!flag)
			{
				node2 = tail;
				node3 = node2.Next;
				if (tail == node2)
				{
					if (node3 == null)
					{
						flag = Interlocked.CompareExchange(ref tail.Next, node, null) == null;
					}
					else
					{
						Interlocked.CompareExchange(ref tail, node3, node2);
					}
				}
			}
			Interlocked.CompareExchange(ref tail, node, node2);
			Interlocked.Increment(ref count);
		}

		bool IProducerConsumerCollection<T>.TryAdd(T item)
		{
			Enqueue(item);
			return true;
		}

		public bool TryDequeue(out T result)
		{
			result = default(T);
			Node node = null;
			bool flag = false;
			while (!flag)
			{
				Node node2 = head;
				Node node3 = tail;
				node = node2.Next;
				if (node2 != head)
				{
					continue;
				}
				if (node2 == node3)
				{
					if (node == null)
					{
						result = default(T);
						return false;
					}
					Interlocked.CompareExchange(ref tail, node, node3);
				}
				else
				{
					result = node.Value;
					flag = Interlocked.CompareExchange(ref head, node, node2) == node2;
				}
			}
			node.Value = default(T);
			Interlocked.Decrement(ref count);
			return true;
		}

		public bool TryPeek(out T result)
		{
			result = default(T);
			bool flag = true;
			while (flag)
			{
				Node node = head;
				Node next = node.Next;
				if (next == null)
				{
					result = default(T);
					return false;
				}
				result = next.Value;
				flag = head != node;
			}
			return true;
		}

		internal void Clear()
		{
			count = 0;
			tail = (head = new Node());
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return InternalGetEnumerator();
		}

		public IEnumerator<T> GetEnumerator()
		{
			return InternalGetEnumerator();
		}

		private IEnumerator<T> InternalGetEnumerator()
		{
			Node my_head = head;
			while (true)
			{
				Node next;
				my_head = (next = my_head.Next);
				if (next != null)
				{
					yield return my_head.Value;
					continue;
				}
				break;
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (array.Rank > 1)
			{
				throw new ArgumentException("The array can't be multidimensional");
			}
			if (array.GetLowerBound(0) != 0)
			{
				throw new ArgumentException("The array needs to be 0-based");
			}
			T[] array2 = array as T[];
			if (array2 == null)
			{
				throw new ArgumentException("The array cannot be cast to the collection element type", "array");
			}
			CopyTo(array2, index);
		}

		public void CopyTo(T[] array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (index >= array.Length)
			{
				throw new ArgumentException("index is equals or greather than array length", "index");
			}
			IEnumerator<T> enumerator = InternalGetEnumerator();
			int num = index;
			while (enumerator.MoveNext())
			{
				if (num == array.Length - index)
				{
					throw new ArgumentException("The number of elememts in the collection exceeds the capacity of array", "array");
				}
				array[num++] = enumerator.Current;
			}
		}

		public T[] ToArray()
		{
			return new List<T>(this).ToArray();
		}

		bool IProducerConsumerCollection<T>.TryTake(out T item)
		{
			return TryDequeue(out item);
		}
	}
}
