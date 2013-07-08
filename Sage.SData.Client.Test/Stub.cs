using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace System.Xml.XPath
{
    internal class XPathNavigator
    {
        private readonly XObject _obj;
        private readonly XmlNameTable _nameTable;

        public XPathNavigator(XObject obj)
        {
            _obj = obj;

            _nameTable = new NameTable();
            _nameTable.Add(string.Empty);
            _nameTable.Add("http://www.w3.org/2000/xmlns/");
            _nameTable.Add("http://www.w3.org/XML/1998/namespace");
        }

        public XmlNameTable NameTable
        {
            get { return _nameTable; }
        }

        public string Value
        {
            get
            {
                var elem = _obj as XElement;
                if (elem != null)
                {
                    return elem.Value;
                }
                var attr = _obj as XAttribute;
                if (attr != null)
                {
                    return attr.Value;
                }

                return _obj.ToString();
            }
        }

        public bool IsEmptyElement
        {
            get
            {
                var source = _obj as XElement;
                return source != null && source.IsEmpty;
            }
        }

        public bool HasChildren
        {
            get
            {
                var source = _obj as XElement;
                return source != null && source.HasElements;
            }
        }

        public XPathNavigator SelectSingleNode(string path, XmlNamespaceManager manager = null)
        {
            return new XPathNavigator(SelectInternal(path, manager).FirstOrDefault());
        }

        public XPathNodeIterator Select(string path, XmlNamespaceManager manager = null)
        {
            return new XPathNodeIterator(SelectInternal(path, manager));
        }

        private IEnumerable<XObject> SelectInternal(string path, IXmlNamespaceResolver manager)
        {
            if (path.StartsWith("/"))
            {
                path = path.Substring(1);
            }

            var nodes = (IEnumerable<XObject>) new[] {_obj};
            foreach (var part in path.Split('/'))
            {
                if (part == "*")
                {
                    nodes = nodes.Cast<XContainer>().Elements();
                }
                else if (part == "")
                {
                    nodes = nodes.Cast<XContainer>().Descendants();
                }
                else
                {
                    var isAttribute = part.StartsWith("@");
                    var str = isAttribute ? part.Substring(1) : part;
                    var pos = str.IndexOf(":", StringComparison.Ordinal);
                    var name = pos >= 0
                                   ? XName.Get(str.Substring(pos + 1), manager.LookupNamespace(str.Substring(0, pos)))
                                   : XName.Get(str);
                    if (isAttribute)
                    {
                        nodes = nodes.Cast<XElement>().Attributes(name);
                    }
                    else
                    {
                        nodes = nodes.Cast<XContainer>().Elements(name);
                    }
                }
            }
            return nodes;
        }
    }

    internal class XPathNodeIterator : IEnumerable
    {
        private readonly IEnumerable<XObject> _items;

        public XPathNodeIterator(IEnumerable<XObject> items)
        {
            _items = items;
        }

        public int Count
        {
            get { return _items.Count(); }
        }

        public IEnumerator GetEnumerator()
        {
            return _items.Select(item => new XPathNavigator(item)).GetEnumerator();
        }
    }

    internal static class XDocumentExtensions
    {
        public static XPathNavigator CreateNavigator(this XDocument doc)
        {
            return new XPathNavigator(doc);
        }
    }
}