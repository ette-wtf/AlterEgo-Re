using System;
using System.Security;

namespace NUnit.Compatibility
{
	public class LongLivedMarshalByRefObject : MarshalByRefObject
	{
		[SecurityCritical]
		public override object InitializeLifetimeService()
		{
			return null;
		}
	}
}
