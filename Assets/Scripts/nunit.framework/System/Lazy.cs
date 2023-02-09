using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading;

namespace System
{
	[Serializable]
	[DebuggerDisplay("ThreadSafetyMode={mode}, IsValueCreated={IsValueCreated}, IsValueFaulted={exception != null}, Value={value}")]
	[ComVisible(false)]
	[HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
	internal class Lazy<T>
	{
		private T value;

		private Func<T> factory;

		private object monitor;

		private Exception exception;

		private LazyThreadSafetyMode mode;

		private bool inited;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public T Value
		{
			get
			{
				if (inited)
				{
					return value;
				}
				if (exception != null)
				{
					throw exception;
				}
				return InitValue();
			}
		}

		public bool IsValueCreated
		{
			get
			{
				return inited;
			}
		}

		public Lazy()
			: this(LazyThreadSafetyMode.ExecutionAndPublication)
		{
		}

		public Lazy(Func<T> valueFactory)
			: this(valueFactory, LazyThreadSafetyMode.ExecutionAndPublication)
		{
		}

		public Lazy(bool isThreadSafe)
			: this((Func<T>)Activator.CreateInstance<T>, isThreadSafe ? LazyThreadSafetyMode.ExecutionAndPublication : LazyThreadSafetyMode.None)
		{
		}

		public Lazy(Func<T> valueFactory, bool isThreadSafe)
			: this(valueFactory, isThreadSafe ? LazyThreadSafetyMode.ExecutionAndPublication : LazyThreadSafetyMode.None)
		{
		}

		public Lazy(LazyThreadSafetyMode mode)
			: this((Func<T>)Activator.CreateInstance<T>, mode)
		{
		}

		public Lazy(Func<T> valueFactory, LazyThreadSafetyMode mode)
		{
			if (valueFactory == null)
			{
				throw new ArgumentNullException("valueFactory");
			}
			factory = valueFactory;
			if (mode != 0)
			{
				monitor = new object();
			}
			this.mode = mode;
		}

		private T InitValue()
		{
			switch (mode)
			{
			case LazyThreadSafetyMode.None:
			{
				Func<T> func = factory;
				if (func == null)
				{
					if (exception == null)
					{
						throw new InvalidOperationException("The initialization function tries to access Value on this instance");
					}
					throw exception;
				}
				try
				{
					factory = null;
					T val = func();
					value = val;
					Thread.MemoryBarrier();
					inited = true;
				}
				catch (Exception ex)
				{
					exception = ex;
					throw;
				}
				break;
			}
			case LazyThreadSafetyMode.PublicationOnly:
			{
				Func<T> func = factory;
				T val = ((func == null) ? default(T) : func());
				lock (monitor)
				{
					if (inited)
					{
						return value;
					}
					value = val;
					Thread.MemoryBarrier();
					inited = true;
					factory = null;
				}
				break;
			}
			case LazyThreadSafetyMode.ExecutionAndPublication:
				lock (monitor)
				{
					if (inited)
					{
						return value;
					}
					if (factory == null)
					{
						if (exception == null)
						{
							throw new InvalidOperationException("The initialization function tries to access Value on this instance");
						}
						throw exception;
					}
					Func<T> func = factory;
					try
					{
						factory = null;
						T val = func();
						value = val;
						Thread.MemoryBarrier();
						inited = true;
					}
					catch (Exception ex)
					{
						exception = ex;
						throw;
					}
				}
				break;
			default:
				throw new InvalidOperationException("Invalid LazyThreadSafetyMode " + mode);
			}
			return value;
		}

		public override string ToString()
		{
			if (inited)
			{
				return value.ToString();
			}
			return "Value is not created";
		}
	}
}
