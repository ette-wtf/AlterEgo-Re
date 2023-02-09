using System.Diagnostics;

namespace System.Collections.Generic
{
	internal sealed class CollectionDebuggerView<T>
	{
		private readonly ICollection<T> c;

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public T[] Items
		{
			get
			{
				T[] array = new T[c.Count];
				c.CopyTo(array, 0);
				return array;
			}
		}

		public CollectionDebuggerView(ICollection<T> col)
		{
			c = col;
		}
	}
	internal sealed class CollectionDebuggerView<T, U>
	{
		private readonly ICollection<KeyValuePair<T, U>> c;

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public KeyValuePair<T, U>[] Items
		{
			get
			{
				KeyValuePair<T, U>[] array = new KeyValuePair<T, U>[c.Count];
				c.CopyTo(array, 0);
				return array;
			}
		}

		public CollectionDebuggerView(ICollection<KeyValuePair<T, U>> col)
		{
			c = col;
		}
	}
}
