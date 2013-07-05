using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.XPath;
using Sage.SData.Client.Common;

namespace Sage.SData.Client.Extensions
{
    [Serializable]
    public class SDataSimpleCollection : List<object>
    {
        public SDataSimpleCollection(string itemElementName)
        {
            ItemElementName = itemElementName;
        }

        public SDataSimpleCollection()
        {
        }

        /// <summary>
        /// Specifies the local element name of the item when serialized to the XML Array.
        /// </summary>
        /// <remarks>This is inferred from the first element's <see cref="XPathNavigator.LocalName">local name</see>.</remarks>
        public string ItemElementName { get; set; }

        public bool Load(XPathNavigator source)
        {
            //------------------------------------------------------------
            //	Validate parameter
            //------------------------------------------------------------
            Guard.ArgumentNotNull(source, "source");

            var items = source.SelectChildren(XPathNodeType.Element).Cast<XPathNavigator>();
            return InternalLoad(items);
        }

        private bool InternalLoad(IEnumerable<XPathNavigator> items)
        {
            var list = items.ToList();
            if (list.Count == 0)
            {
                return true;
            }

            ItemElementName = list[0].LocalName;

            foreach (var item in list)
            {
                string nilValue;
                object value;

                if (item.TryGetAttribute("nil", Framework.Common.Xsi.Namespace, out nilValue) && XmlConvert.ToBoolean(nilValue))
                {
                    value = null;
                }
                else
                {
                    value = item.Value;
                }

                Add(value);
            }

            return true;
        }

        public void WriteTo(string name, string ns, XmlWriter writer)
        {
            if (string.IsNullOrEmpty(ItemElementName))
            {
                throw new InvalidOperationException("ItemElementName must be set");
            }

            writer.WriteStartElement(name, ns);

            foreach (var item in this)
            {
                WriteItemTo(ItemElementName, ns, item, writer);
            }

            writer.WriteEndElement();
        }

        private static void WriteItemTo(string name, string ns, object value, XmlWriter writer)
        {
            if (value == null)
            {
                writer.WriteStartElement(name, ns);
                writer.WriteAttributeString("nil", Framework.Common.Xsi.Namespace, XmlConvert.ToString(true));
                writer.WriteEndElement();
            }
            else
            {
                writer.WriteElementString(name, ns, SDataPayload.ValueToString(value));
            }
        }
    }
}