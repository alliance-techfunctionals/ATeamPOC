using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using XsdToObjectTreeLibrary;
using XsdToObjectTreeLibrary.Merge.Models;
using System.Xml.Linq;

namespace XsdToObjectTreeLibrary.Merge
{
    public class XmlMerger : IXmlMerger
    {
        private IXmlElementInfo _elementInfo;

        public XmlMerger(IXmlElementInfo elementInfo)
        {
            _elementInfo = elementInfo;
        }

        public void Merge(IEnumerable<string> sourcePaths, string targetPath)
        {
            IEnumerable<XElement> repeatingElements = new List<XElement>();
            var firstPath = sourcePaths.First();

            var parentNode = _elementInfo.GetRepeatingElementParent(firstPath);
            if (parentNode != null)
            {
                var parentElements = ReadAncestorsAndSelf(firstPath, parentNode);

                foreach (var path in sourcePaths)
                {
                    repeatingElements = repeatingElements.Concat(ReadRepeatingElements(path, parentNode));
                }

                WriteElementsToFile(targetPath, parentElements, repeatingElements);
            }
        }

        internal IEnumerable<XElement> ReadAncestorsAndSelf(string sourcePath, XmlElementNode selectedNode)
        {
            var settings = new XmlReaderSettings { Async = false };
            using (var reader = XmlReader.Create(sourcePath, settings))
            {
                while (reader.Read() && reader.Depth <= selectedNode.Depth)
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:

                            var elementName = reader.Name;
                            var elementValue = reader.Value;

                            var attributes = new List<XAttribute>();
                            while (reader.MoveToNextAttribute())
                            {
                                var attribute = new XAttribute(reader.Name, reader.Value);
                                attributes.Add(attribute);
                            }

                            yield return new XElement(elementName, attributes, elementValue);
                            break;
                    }
                }
            }
        }

        internal IEnumerable<XElement> ReadRepeatingElements(string sourcePath, XmlElementNode parentNode)
        {
            var settings = new XmlReaderSettings { Async = false };
            using (var reader = XmlReader.Create(sourcePath, settings))
            {
                reader.ReadToDescendant(parentNode.Name);

                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Depth > parentNode.Depth)
                    {
                        using (var elementReader = reader.ReadSubtree())
                        {
                            yield return XElement.Load(elementReader);
                        }

                        reader.Skip();
                    }
                }
            }
        }

        internal void WriteElementsToFile(string fullPath, IEnumerable<XElement> parentElements, IEnumerable<XElement> repeatingElements)
        {
            var elements = parentElements.ToList();
            var xws = new XmlWriterSettings { Indent = true };
            using (XmlWriter xw = XmlWriter.Create(fullPath, xws))
            {
                foreach (var element in elements)
                {
                    xw.WriteStartElement(element.Name.LocalName);

                    foreach (var attribute in element.Attributes())
                    {
                        xw.WriteAttributeString(attribute.Name.LocalName, attribute.Value);
                    }
                }

                foreach (var childElement in repeatingElements)
                {
                    childElement.WriteTo(xw);
                }

                foreach (var element in elements)
                {
                    xw.WriteEndElement();
                }

                xw.Flush();
            }
        }
    }
}
