using System.Xml.Schema;
using XsdToObjectTreeLibrary.Tree.Model;

namespace XsdToObjectTreeLibrary.Tree.Xsd
{
    public interface IXsdToTree
    {
        Node GetTree(XmlSchemaSet set);
    }
}