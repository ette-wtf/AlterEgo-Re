using System;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class AuthorAttribute : PropertyAttribute
	{
		public AuthorAttribute(string name)
			: base("Author", name)
		{
		}

		public AuthorAttribute(string name, string email)
			: base("Author", string.Format("{0} <{1}>", name, email))
		{
		}
	}
}
