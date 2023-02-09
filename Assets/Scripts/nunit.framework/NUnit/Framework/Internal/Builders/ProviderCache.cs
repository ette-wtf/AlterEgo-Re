using System;
using System.Collections.Generic;

namespace NUnit.Framework.Internal.Builders
{
	internal class ProviderCache
	{
		private class CacheEntry
		{
			private Type providerType;

			public CacheEntry(Type providerType, object[] providerArgs)
			{
				this.providerType = providerType;
			}

			public override bool Equals(object obj)
			{
				CacheEntry cacheEntry = obj as CacheEntry;
				if (cacheEntry == null)
				{
					return false;
				}
				return (object)providerType == cacheEntry.providerType;
			}

			public override int GetHashCode()
			{
				return providerType.GetHashCode();
			}
		}

		private static Dictionary<CacheEntry, object> instances = new Dictionary<CacheEntry, object>();

		public static object GetInstanceOf(Type providerType)
		{
			return GetInstanceOf(providerType, null);
		}

		public static object GetInstanceOf(Type providerType, object[] providerArgs)
		{
			CacheEntry key = new CacheEntry(providerType, providerArgs);
			object obj = (instances.ContainsKey(key) ? instances[key] : null);
			if (obj == null)
			{
				obj = (instances[key] = Reflect.Construct(providerType, providerArgs));
			}
			return obj;
		}

		public static void Clear()
		{
			foreach (CacheEntry key in instances.Keys)
			{
				IDisposable disposable = instances[key] as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			instances.Clear();
		}
	}
}
