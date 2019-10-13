using System.Collections.Generic;

namespace XsdToObjectTreeLibrary.Merge
{
    public interface IXmlMerger
    {
        void Merge(IEnumerable<string> sourcePaths, string targetPath);
    }
}