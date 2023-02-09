using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
	public class PropertyBag : IPropertyBag, IXmlNodeBuilder
	{
		private Dictionary<string, IList> inner = new Dictionary<string, IList>();

		public ICollection<string> Keys
		{
			get
			{
				return inner.Keys;
			}
		}

		public IList this[string key]
		{
			get
			{
				IList value;
				if (!inner.TryGetValue(key, out value))
				{
					value = new List<object>();
					inner.Add(key, value);
				}
				return value;
			}
			set
			{
				inner[key] = value;
			}
		}

		public void Add(string key, object value)
		{
			IList value2;
			if (!inner.TryGetValue(key, out value2))
			{
				value2 = new List<object>();
				inner.Add(key, value2);
			}
			value2.Add(value);
		}

		public void Set(string key, object value)
		{
			Guard.ArgumentNotNull(key, "key");
			Guard.ArgumentNotNull(value, "value");
			IList list = new List<object>();
			list.Add(value);
			inner[key] = list;
		}

		public object Get(string key)
		{
			IList value;
			return (inner.TryGetValue(key, out value) && value.Count > 0) ? value[0] : null;
		}

		public bool ContainsKey(string key)
		{
			return inner.ContainsKey(key);
		}

		public TNode ToXml(bool recursive)
		{
			return AddToXml(new TNode("dummy"), recursive);
		}

		public TNode AddToXml(TNode parentNode, bool recursive)
		{
			TNode tNode = parentNode.AddElement("properties");
			foreach (string key in Keys)
			{
				foreach (object item in this[key])
				{
					TNode tNode2 = tNode.AddElement("property");
					tNode2.AddAttribute("name", key.ToString());
					tNode2.AddAttribute("value", item.ToString());
				}
			}
			return tNode;
		}
	}
}
