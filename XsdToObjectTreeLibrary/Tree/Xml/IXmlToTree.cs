using XsdToObjectTreeLibrary.Tree.Model;

namespace XsdToObjectTreeLibrary.Tree.Xml
{
    public interface IXmlToTree
    {
        Node GetTree(string xml);
    }
}