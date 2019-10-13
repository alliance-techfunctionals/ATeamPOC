using System;
using System.Collections.Generic;
using System.Text;

namespace XsdToObjectTreeLibrary.Merge
{
    public class XmlMerger : IXmlMerger
    {
        public void Merge(IEnumerable<string> sourcePaths, string targetPath)
        {
            //merge the xml in source paths, in order and write to target path
            //files may be large, so dont load entire document in memory, Use XmlReader and XmlWriter

            //I have included IXmlElementInfo.GetRepeatingElementParent. This may be helpful for you.
            //I use this in another part of the Xml processing chain. You can use this to determine the parent XmlNode of the Xml Elements to be merged. 
            throw new NotImplementedException();
        }
    }
}
