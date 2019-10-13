using XsdToObjectTreeLibrary.Merge.Models;

namespace XsdToObjectTreeLibrary
{
    public interface IXmlElementInfo
    {
        XmlElementNode GetRepeatingElementParent(string uri);
    }
}