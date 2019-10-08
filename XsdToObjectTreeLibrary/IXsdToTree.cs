using System.Xml.Schema;
using XsdToObjectTreeLibrary.Model;

namespace XsdToObjectTreeLibrary
{
    public interface IXsdToTree
    {
        Node GetTree(XmlSchemaSet set);
    }
}