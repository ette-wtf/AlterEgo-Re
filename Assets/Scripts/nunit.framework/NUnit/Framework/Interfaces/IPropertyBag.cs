using System.Collections;
using System.Collections.Generic;

namespace NUnit.Framework.Interfaces
{
	public interface IPropertyBag : IXmlNodeBuilder
	{
		IList this[string key] { get; set; }

		ICollection<string> Keys { get; }

		void Add(string key, object value);

		void Set(string key, object value);

		object Get(string key);

		bool ContainsKey(string key);
	}
}
