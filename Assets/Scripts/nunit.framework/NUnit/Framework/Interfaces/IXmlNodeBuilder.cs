namespace NUnit.Framework.Interfaces
{
	public interface IXmlNodeBuilder
	{
		TNode ToXml(bool recursive);

		TNode AddToXml(TNode parentNode, bool recursive);
	}
}
