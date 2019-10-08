using XsdToObjectTreeLibrary.Model;

namespace XsdToObjectTreeLibrary.Xml
{
    public interface IXmlToTree
    {
        Node GetTree(string xml);
    }
}