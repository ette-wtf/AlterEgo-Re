using System.Collections.Generic;

namespace System.Collections.Concurrent
{
	internal interface IProducerConsumerCollection<T> : IEnumerable<T>, ICollection, IEnumerable
	{
		bool TryAdd(T item);

		bool TryTake(out T item);

		T[] ToArray();

		void CopyTo(T[] array, int index);
	}
}
