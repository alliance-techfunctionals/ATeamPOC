using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using XsdToObjectTreeLibrary.Merge.Models;
using System.Linq;

namespace XsdToObjectTreeLibrary.Merge
{
    public class XmlMerger : IXmlMerger
    {
        public void Merge(IEnumerable<string> sourcePaths, string targetPath)
        {
            XmlElementInfo oXmlInfo = new XmlElementInfo();
            XmlElementNode root = oXmlInfo.GetElementTree(sourcePaths.First());
            XmlElementNode node = oXmlInfo.GetRepeatingElementParent(sourcePaths.First());
            string mergeElementName = node.Name;
            IEnumerable<XAttribute> mergeAttribute = XmlFileMergeAttributeReader(sourcePaths.First(), mergeElementName);
            IEnumerable<XElement> mergeElements = XmlFileMergeReader(sourcePaths, mergeElementName);
            XStreamingElement mergeXml = new XStreamingElement(root.Name, new XStreamingElement(mergeElementName, mergeAttribute, mergeElements));
            mergeXml.Save(targetPath);
        }
        private static IEnumerable<XAttribute> XmlFileMergeAttributeReader(string inputFile, string mergeElementName)
        {
            using (XmlReader reader = XmlReader.Create(inputFile))
            {
                do
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == mergeElementName)
                    {
                        if (reader.HasAttributes)
                        {
                            while (reader.MoveToNextAttribute())
                            {
                                yield return new XAttribute(reader.Name, reader.Value);
                            }
                        }
                        break;
                    }
                } while (reader.Read());

                reader.Close();
            }
        }
        private static IEnumerable<XElement> XmlFileMergeReader(IEnumerable<string> inputFiles, string mergeElementName)
        {
            foreach (string inputFile in inputFiles)
            {
                using (XmlReader reader = XmlReader.Create(inputFile))
                {
                    XmlReader subReader = XmlFileMergeElementsReader(reader, mergeElementName);
                    if (subReader != null)
                    {
                        do
                        {
                            if (subReader.NodeType == XmlNodeType.Element && subReader.Name != mergeElementName)
                            {
                                XElement el = XElement.ReadFrom(subReader) as XElement;
                                if (el != null)
                                {
                                    yield return el;
                                }
                            }
                            else
                            {
                                subReader.Read();
                            }
                        } while (!subReader.EOF);
                        subReader.Close();
                    }
                    reader.Close();
                }
            }
        }
        private static XmlReader XmlFileMergeElementsReader(XmlReader reader, string mergeElementName)
        {
            XmlReader subReader = null;

            if (reader == null || mergeElementName == "")
            {
                return subReader;
            }

            do
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == mergeElementName)
                {
                    subReader = reader.ReadSubtree();
                    break;
                }
                else
                {
                    reader.Read();
                }
            } while (!reader.EOF);
            return subReader;
        }
    }
}