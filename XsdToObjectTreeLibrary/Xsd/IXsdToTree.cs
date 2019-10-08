using System.Xml.Schema;
using XsdToObjectTreeLibrary.Model;

namespace XsdToObjectTreeLibrary.Xsd
{
    public interface IXsdToTree
    {
        Node GetTree(XmlSchemaSet set);
    }
}