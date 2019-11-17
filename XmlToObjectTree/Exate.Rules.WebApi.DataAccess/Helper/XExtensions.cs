using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using System.Linq;
using Exate.Rules.WebApi.Models;

namespace Exate.Rules.WebApi.DataAccess.Helper
{
    public static class XExtensions
    {
        public static IEnumerable<XPathResponseModel> GetDistinctAttributesAndLeafElements(this XDocument document)
        {
            var results = new IEnumerable<XPathResponseModel>[] { GetLeafElementsXPaths(document, true), GetAttributesXPaths(document, true) };
            return results.SelectMany(it => it).OrderBy(e => e.XPathValue);
        }

        public static IEnumerable<XPathResponseModel> GetLeafElementsXPaths(this XDocument document, bool distinct = false)
        {
            var leaves = GetLeafElements(document);
            var result = leaves.Select(e => new XPathResponseModel
            {
                NodeName = e.Name.LocalName,
                XPathValue = e.GetAbsoluteXPath(!distinct)
            });

            return distinct ? result.Distinct() : result;
        }

        public static IEnumerable<XPathResponseModel> GetAttributesXPaths(this XDocument document, bool distinct = false)
        {
            var attributes = GetAttributes(document);
            var result = attributes.Select(e => new XPathResponseModel
            {
                NodeName = e.Name.LocalName,
                XPathValue = e.GetAbsoluteXPath(false)
            }).Distinct();

            return distinct ? result.Distinct() : result;
        }

        internal static IEnumerable<XAttribute> GetAttributes(this XDocument document)
        {
            var attributes =
                from e in document.Descendants()
                where e.Attributes().Any()
                select e.Attributes();

            return attributes.SelectMany(a => a);
        }

        internal static IEnumerable<XElement> GetLeafElements(this XDocument document)
        {
            var leaves =
                from e in document.Descendants()
                where !e.Elements().Any()
                select e;

            return leaves;
        }

        /// <summary>
        /// Get the absolute XPath to a given XAttribute
        /// (e.g. "/people/person[6]/name[1]/last[1]/@code").
        /// </summary>
        public static string GetAbsoluteXPath(this XAttribute attribute, bool includeIndex = true)
        {
            var result = GetAbsoluteXPath(attribute.Parent, includeIndex);
            return string.Format("{0}/@{1}", result, attribute.Name.LocalName);
        }

        /// <summary>
        /// Get the absolute XPath to a given XElement, including the namespace.
        /// (e.g. "/a:people/b:person[6]/c:name[1]/d:last[1]").
        /// </summary>
        public static string GetAbsoluteXPath(this XElement element, bool includeIndex = true)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            Func<XElement, string> relativeXPath = e =>
            {
                int index = e.IndexPosition();

                var currentNamespace = e.Name.Namespace;

                string name;
                if (string.IsNullOrEmpty(currentNamespace.ToString()))
                {
                    name = e.Name.LocalName; //no namespace
                }
                else if (string.IsNullOrEmpty(e.GetPrefixOfNamespace(currentNamespace)))
                {
                    name = $"*[local-name()='{e.Name.LocalName}']"; //default namespace
                }
                else
                {
                    string namespacePrefix = e.GetPrefixOfNamespace(currentNamespace);
                    name = $"{namespacePrefix}:{ e.Name.LocalName}"; //prefix namespace
                }

                // If the element is the root or has no sibling elements, no index is required
                return ((index == -1) || (index == -2) || (!includeIndex)) ? "/" + name : string.Format
                (
                    "/{0}[{1}]",
                    name,
                    index.ToString()
                );
            };

            var ancestors = from e in element.Ancestors()
                            select relativeXPath(e);

            return string.Concat(ancestors.Reverse().ToArray()) +
                   relativeXPath(element);
        }

        /// <summary>
        /// Get the index of the given XElement relative to its
        /// siblings with identical names. If the given element is
        /// the root, -1 is returned or -2 if element has no sibling elements.
        /// </summary>
        /// <param name="element">
        /// The element to get the index of.
        /// </param>
        internal static int IndexPosition(this XElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            if (element.Parent == null)
            {
                // Element is root
                return -1;
            }

            if (element.Parent.Elements(element.Name).Count() == 1)
            {
                // Element has no sibling elements
                return -2;
            }

            int i = 1; // Indexes for nodes start at 1, not 0

            foreach (var sibling in element.Parent.Elements(element.Name))
            {
                if (sibling == element)
                {
                    return i;
                }

                i++;
            }

            throw new InvalidOperationException("element has been removed from its parent.");
        }
    }

}
