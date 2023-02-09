using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace NUnit.Framework.Interfaces
{
	public class TNode
	{
		private class NodeFilter
		{
			private string _nodeName;

			private string _propName;

			private string _propValue;

			public NodeFilter(string xpath)
			{
				_nodeName = xpath;
				int num = xpath.IndexOf('[');
				if (num >= 0)
				{
					if (!xpath.EndsWith("]"))
					{
						throw new ArgumentException("Invalid property expression", "xpath");
					}
					_nodeName = xpath.Substring(0, num);
					string text = xpath.Substring(num + 1, xpath.Length - num - 2);
					int num2 = text.IndexOf('=');
					if (num2 < 0 || text[0] != '@')
					{
						throw new ArgumentException("Invalid property expression", "xpath");
					}
					_propName = text.Substring(1, num2 - 1).Trim();
					_propValue = text.Substring(num2 + 1).Trim(' ', '"', '\'');
				}
			}

			public bool Pass(TNode node)
			{
				if (node.Name != _nodeName)
				{
					return false;
				}
				if (_propName == null)
				{
					return true;
				}
				return node.Attributes[_propName] == _propValue;
			}
		}

		private static readonly Regex InvalidXmlCharactersRegex = new Regex("[^\t\n\r -\ufffd]|([\ud800-\udbff](?![\udc00-\udfff]))|((?<![\ud800-\udbff])[\udc00-\udfff])");

		public string Name { get; private set; }

		public string Value { get; set; }

		public bool ValueIsCDATA { get; private set; }

		public AttributeDictionary Attributes { get; private set; }

		public NodeList ChildNodes { get; private set; }

		public TNode FirstChild
		{
			get
			{
				return (ChildNodes.Count == 0) ? null : ChildNodes[0];
			}
		}

		public string OuterXml
		{
			get
			{
				StringWriter stringWriter = new StringWriter();
				XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
				xmlWriterSettings.ConformanceLevel = ConformanceLevel.Fragment;
				using (XmlWriter writer = XmlWriter.Create(stringWriter, xmlWriterSettings))
				{
					WriteTo(writer);
				}
				return stringWriter.ToString();
			}
		}

		public TNode(string name)
		{
			Name = name;
			Attributes = new AttributeDictionary();
			ChildNodes = new NodeList();
		}

		public TNode(string name, string value)
			: this(name, value, false)
		{
		}

		public TNode(string name, string value, bool valueIsCDATA)
			: this(name)
		{
			Value = value;
			ValueIsCDATA = valueIsCDATA;
		}

		public static TNode FromXml(string xmlText)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(xmlText);
			return FromXml(xmlDocument.FirstChild);
		}

		public TNode AddElement(string name)
		{
			TNode tNode = new TNode(name);
			ChildNodes.Add(tNode);
			return tNode;
		}

		public TNode AddElement(string name, string value)
		{
			TNode tNode = new TNode(name, EscapeInvalidXmlCharacters(value));
			ChildNodes.Add(tNode);
			return tNode;
		}

		public TNode AddElementWithCDATA(string name, string value)
		{
			TNode tNode = new TNode(name, EscapeInvalidXmlCharacters(value), true);
			ChildNodes.Add(tNode);
			return tNode;
		}

		public void AddAttribute(string name, string value)
		{
			Attributes.Add(name, EscapeInvalidXmlCharacters(value));
		}

		public TNode SelectSingleNode(string xpath)
		{
			NodeList nodeList = SelectNodes(xpath);
			return (nodeList.Count > 0) ? nodeList[0] : null;
		}

		public NodeList SelectNodes(string xpath)
		{
			NodeList nodeList = new NodeList();
			nodeList.Add(this);
			return ApplySelection(nodeList, xpath);
		}

		public void WriteTo(XmlWriter writer)
		{
			writer.WriteStartElement(Name);
			foreach (string key in Attributes.Keys)
			{
				writer.WriteAttributeString(key, Attributes[key]);
			}
			if (Value != null)
			{
				if (ValueIsCDATA)
				{
					WriteCDataTo(writer);
				}
				else
				{
					writer.WriteString(Value);
				}
			}
			foreach (TNode childNode in ChildNodes)
			{
				childNode.WriteTo(writer);
			}
			writer.WriteEndElement();
		}

		private static TNode FromXml(XmlNode xmlNode)
		{
			TNode tNode = new TNode(xmlNode.Name, xmlNode.InnerText);
			foreach (XmlAttribute attribute in xmlNode.Attributes)
			{
				tNode.AddAttribute(attribute.Name, attribute.Value);
			}
			foreach (XmlNode childNode in xmlNode.ChildNodes)
			{
				if (childNode.NodeType == XmlNodeType.Element)
				{
					tNode.ChildNodes.Add(FromXml(childNode));
				}
			}
			return tNode;
		}

		private static NodeList ApplySelection(NodeList nodeList, string xpath)
		{
			Guard.ArgumentNotNullOrEmpty(xpath, "xpath");
			if (xpath[0] == '/')
			{
				throw new ArgumentException("XPath expressions starting with '/' are not supported", "xpath");
			}
			if (xpath.IndexOf("//") >= 0)
			{
				throw new ArgumentException("XPath expressions with '//' are not supported", "xpath");
			}
			string xpath2 = xpath;
			string text = null;
			int num = xpath.IndexOf('/');
			if (num >= 0)
			{
				xpath2 = xpath.Substring(0, num);
				text = xpath.Substring(num + 1);
			}
			NodeList nodeList2 = new NodeList();
			NodeFilter nodeFilter = new NodeFilter(xpath2);
			foreach (TNode node in nodeList)
			{
				foreach (TNode childNode in node.ChildNodes)
				{
					if (nodeFilter.Pass(childNode))
					{
						nodeList2.Add(childNode);
					}
				}
			}
			return (text != null) ? ApplySelection(nodeList2, text) : nodeList2;
		}

		private static string EscapeInvalidXmlCharacters(string str)
		{
			if (str == null)
			{
				return null;
			}
			return InvalidXmlCharactersRegex.Replace(str, (Match match) => CharToUnicodeSequence(match.Value[0]));
		}

		private static string CharToUnicodeSequence(char symbol)
		{
			int num = symbol;
			return string.Format("\\u{0}", num.ToString("x4"));
		}

		private void WriteCDataTo(XmlWriter writer)
		{
			int num = 0;
			string value = Value;
			while (true)
			{
				bool flag = true;
				int num2 = value.IndexOf("]]>", num);
				if (num2 < 0)
				{
					break;
				}
				writer.WriteCData(value.Substring(num, num2 - num + 2));
				num = num2 + 2;
				if (num >= value.Length)
				{
					return;
				}
			}
			if (num > 0)
			{
				writer.WriteCData(value.Substring(num));
			}
			else
			{
				writer.WriteCData(value);
			}
		}
	}
}
