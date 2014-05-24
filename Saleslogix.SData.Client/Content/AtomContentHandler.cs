// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Saleslogix.SData.Client.Framework;
using Saleslogix.SData.Client.Utilities;

namespace Saleslogix.SData.Client.Content
{
    public class AtomContentHandler : IContentHandler
    {
        private static readonly XNamespace _atomNs = Common.Atom.Namespace;
        private static readonly XNamespace _sDataNs = Common.SData.Namespace;
        private static readonly XNamespace _syncNs = Common.Sync.Namespace;
        private static readonly XNamespace _httpNs = Common.Http.Namespace;
        private static readonly XNamespace _openSearchNs = Common.OpenSearch.Namespace;
        private static readonly XNamespace _xsiNs = Common.Xsi.Namespace;

        public object ReadFrom(Stream stream)
        {
            Guard.ArgumentNotNull(stream, "stream");

            XElement root;
            using (var reader = XmlReader.Create(stream))
            {
                root = XDocument.Load(reader).Root;
            }

            if (root == null)
            {
                throw new SDataClientException("No root element found");
            }

            if (root.Name == _atomNs + "feed")
            {
                return ReadFeed(root);
            }

            if (root.Name == _atomNs + "entry")
            {
                return ReadEntry(root);
            }

            throw new SDataClientException(string.Format("Unexpected root element '{0}' found", root.Name));
        }

        private static SDataCollection<SDataResource> ReadFeed(XContainer feed)
        {
            var collection = new SDataCollection<SDataResource>(feed.Elements(_atomNs + "entry").Select(ReadEntry))
                                 {
                                     Id = ReadElementValue<string>(feed, _atomNs + "id"),
                                     Title = ReadElementValue<string>(feed, _atomNs + "title"),
                                     Updated = ReadElementValue<DateTimeOffset?>(feed, _atomNs + "updated"),
                                     TotalResults = ReadElementValue<int?>(feed, _openSearchNs + "totalResults"),
                                     StartIndex = ReadElementValue<int?>(feed, _openSearchNs + "startIndex"),
                                     ItemsPerPage = ReadElementValue<int?>(feed, _openSearchNs + "itemsPerPage"),
                                     Schema = ReadElementValue<string>(feed, _sDataNs + "schema"),
                                     Links = ReadLinks(feed),
                                     SyncMode = ReadElementValue<SyncMode?>(feed, _syncNs + "syncMode")
                                 };

            var diagnoses = feed.Element(_sDataNs + "diagnoses");
            if (diagnoses != null)
            {
                collection.Diagnoses = DeserializeObject<Diagnoses>(diagnoses);
            }

            foreach (var diagnosis in feed.Elements(_sDataNs + "diagnosis"))
            {
                collection.Diagnoses.Add(DeserializeObject<Diagnosis>(diagnosis));
            }

            var digest = feed.Element(_syncNs + "digest");
            if (digest != null)
            {
                collection.SyncDigest = DeserializeObject<Digest>(digest);
            }

            return collection;
        }

        private static SDataResource ReadEntry(XContainer entry)
        {
            var payload = entry.Element(_sDataNs + "payload");
            if (payload != null)
            {
                payload = payload.Elements().FirstOrDefault();
            }

            return ReadResource(payload, entry);
        }

        private static SDataResource ReadResource(XElement payload, XContainer entry)
        {
            var resource = new SDataResource();

            if (payload != null)
            {
                resource.XmlLocalName = payload.Name.LocalName;
                resource.XmlNamespace = payload.Name.NamespaceName;
                resource.Key = ReadAttributeValue<string>(payload, _sDataNs + "key");
                resource.Url = ReadAttributeValue<Uri>(payload, _sDataNs + "uri");
                resource.Uuid = ReadAttributeValue<Guid?>(payload, _sDataNs + "uuid");
                resource.Lookup = ReadAttributeValue<string>(payload, _sDataNs + "lookup");
                resource.Descriptor = ReadAttributeValue<string>(payload, _sDataNs + "descriptor");
                resource.IsDeleted = ReadAttributeValue<bool?>(payload, _sDataNs + "isDeleted");

                foreach (var group in payload.Elements().GroupBy(item => item.Name))
                {
                    object value;
                    if (group.Count() > 1)
                    {
                        value = ReadResourceCollection(payload, group);
                    }
                    else
                    {
                        var item = group.First();
                        switch (InferItemType(item))
                        {
                            case ItemType.Property:
                                {
                                    var nilAttr = item.Attribute(_xsiNs + "nil");
                                    value = nilAttr != null && XmlConvert.ToBoolean(nilAttr.Value) ? null : item.Value;
                                    break;
                                }
                            case ItemType.Object:
                                {
                                    value = ReadResource(item, null);
                                    break;
                                }
                            case ItemType.PayloadCollection:
                                {
                                    value = ReadResourceCollection(item, null);
                                    break;
                                }
                            case ItemType.SimpleCollection:
                                {
                                    value = ReadSimpleCollection(item);
                                    break;
                                }
                            default:
                                continue;
                        }
                    }

                    resource.Add(group.Key.LocalName, value);
                }
            }

            if (entry != null)
            {
                resource.Id = ReadElementValue<string>(entry, _atomNs + "id");
                resource.Title = ReadElementValue<string>(entry, _atomNs + "title");
                resource.Updated = ReadElementValue<DateTimeOffset?>(entry, _atomNs + "updated");
                resource.Schema = ReadElementValue<string>(entry, _sDataNs + "schema");
                resource.Links = ReadLinks(entry);
                resource.HttpMethod = ReadElementValue<HttpMethod?>(entry, _httpNs + "httpMethod");
                resource.HttpStatus = (HttpStatusCode?) ReadElementValue<int?>(entry, _httpNs + "httpStatus");
                resource.HttpMessage = ReadElementValue<string>(entry, _httpNs + "httpMessage");
                resource.Location = ReadElementValue<string>(entry, _httpNs + "location");
                resource.ETag = ReadElementValue<string>(entry, _httpNs + "etag");
                resource.IfMatch = ReadElementValue<string>(entry, _httpNs + "ifMatch");

                var diagnoses = entry.Element(_sDataNs + "diagnoses");
                if (diagnoses != null)
                {
                    resource.Diagnoses = DeserializeObject<Diagnoses>(diagnoses);
                }

                foreach (var diagnosis in entry.Elements(_sDataNs + "diagnosis"))
                {
                    resource.Diagnoses.Add(DeserializeObject<Diagnosis>(diagnosis));
                }

                var syncState = entry.Element(_syncNs + "syncState");
                if (syncState != null)
                {
                    resource.SyncState = DeserializeObject<SyncState>(syncState);
                }
            }

            return resource;
        }

        private static SDataCollection<SDataResource> ReadResourceCollection(XElement source, IEnumerable<XElement> items)
        {
            var collection = new SDataCollection<SDataResource>
                                 {
                                     Url = ReadAttributeValue<Uri>(source, _sDataNs + "uri"),
                                     DeleteMissing = ReadAttributeValue<bool?>(source, _sDataNs + "deleteMissing"),
                                     XmlIsFlat = items != null
                                 };

            foreach (var item in items ?? source.Elements())
            {
                if (string.IsNullOrEmpty(collection.XmlLocalName))
                {
                    collection.XmlLocalName = item.Name.LocalName;
                }

                var child = ReadResource(item, null);
                if (InferItemType(item) == ItemType.Property)
                {
                    var nilAttr = item.Attribute(_xsiNs + "nil");
                    var value = nilAttr != null && XmlConvert.ToBoolean(nilAttr.Value) ? null : item.Value;
                    child.Add(item.Name.LocalName, value);
                    break;
                }
                collection.Add(child);
            }

            return collection;
        }

        private static SDataCollection<string> ReadSimpleCollection(XContainer source)
        {
            var collection = new SDataCollection<string>();

            foreach (var item in source.Elements())
            {
                if (string.IsNullOrEmpty(collection.XmlLocalName))
                {
                    collection.XmlLocalName = item.Name.LocalName;
                }

                var nilAttr = item.Attribute(_xsiNs + "nil");
                var value = nilAttr != null && XmlConvert.ToBoolean(nilAttr.Value) ? null : item.Value;
                collection.Add(value);
            }

            return collection;
        }

        private static ItemType InferItemType(XElement item)
        {
            var nilAttr = item.Attribute(_xsiNs + "nil");
            if (nilAttr != null && XmlConvert.ToBoolean(nilAttr.Value))
            {
                return ItemType.Property;
            }

            if (item.Attribute(_sDataNs + "key") != null ||
                item.Attribute(_sDataNs + "uuid") != null ||
                item.Attribute(_sDataNs + "lookup") != null ||
                item.Attribute(_sDataNs + "descriptor") != null ||
                item.Attribute(_sDataNs + "isDeleted") != null)
            {
                return ItemType.Object;
            }

            if (item.Attribute(_sDataNs + "uri") != null ||
                item.Attribute(_sDataNs + "deleteMissing") != null)
            {
                return ItemType.PayloadCollection;
            }

            if (item.IsEmpty)
            {
                // workaround: Older versions of the SIF generate payload collections as empty namespace-less elements
                return string.IsNullOrEmpty(item.Name.NamespaceName) ? ItemType.PayloadCollection : ItemType.Property;
            }

            var children = item.Elements().ToList();

            if (children.Count == 0)
            {
                return ItemType.Property;
            }

            if (children.Count > 1 && children.Select(child => child.Name.LocalName).Distinct().Count() == 1)
            {
                if (children.All(child => InferItemType(child) == ItemType.Object))
                {
                    return ItemType.PayloadCollection;
                }
                if (children.All(child => InferItemType(child) == ItemType.Property))
                {
                    return ItemType.SimpleCollection;
                }
            }

            return ItemType.Object;
        }

        private enum ItemType
        {
            Property,
            Object,
            PayloadCollection,
            SimpleCollection
        }

        private static T DeserializeObject<T>(XNode node)
        {
#if NET_2_0
            // Deserialization doesn't work with the Mono implementation of XNode.CreateReader
            using (var reader = new StringReader(node.ToString()))
            {
                return (T) new XmlSerializer(typeof (T)).Deserialize(reader);
            }
#else
            using (var reader = node.CreateReader())
            {
                return (T) new XmlSerializer(typeof (T)).Deserialize(reader);
            }
#endif
        }

        private static IList<SDataLink> ReadLinks(XContainer container)
        {
            return container.Elements(_atomNs + "link")
                .Select(element =>
                {
                    MediaType type;
                    return new SDataLink
                        {
                            Uri = ReadAttributeValue<Uri>(element, "href"),
                            Relation = ReadAttributeValue<string>(element, "rel"),
                            Type = MediaTypeNames.TryGetMediaType(ReadAttributeValue<string>(element, "type"), out type) ? type : (MediaType?) null,
                            Title = ReadAttributeValue<string>(element, "title")
                        };
                })
                .ToList();
        }

        private static T ReadElementValue<T>(XContainer container, XName name)
        {
            var element = container.Element(name);
            if (element == null)
            {
                return default(T);
            }

            return ReadValue<T>(element.Value);
        }

        private static T ReadAttributeValue<T>(XElement element, XName name)
        {
            var attr = element.Attribute(name);
            if (attr == null)
            {
                return default(T);
            }

            return ReadValue<T>(attr.Value);
        }

        private static T ReadValue<T>(string value)
        {
            var type = typeof (T);
            if (type == typeof (Uri))
            {
                return (T) (object) new Uri(value);
            }

            type = Nullable.GetUnderlyingType(type) ?? type;
            if (type.GetTypeInfo().IsEnum)
            {
                return (T) EnumEx.Parse(type, value);
            }

            return XmlConvertEx.FromString<T>(value);
        }

        public void WriteTo(object obj, Stream stream, INamingScheme namingScheme = null)
        {
            Guard.ArgumentNotNull(obj, "obj");
            Guard.ArgumentNotNull(stream, "stream");

            if (namingScheme == null)
            {
                namingScheme = NamingScheme.Default;
            }

            var root = WriteAtomObject(obj, namingScheme) ?? WriteAtomObject(ContentHelper.Serialize(obj, namingScheme), namingScheme);
            if (root == null)
            {
                throw new SDataClientException(string.Format("Type '{0}' cannot be written", obj.GetType()));
            }

            var doc = new XDocument();
            root.Add(new XAttribute(XNamespace.Xmlns + Common.SData.Prefix, _sDataNs));
            root.Add(new XAttribute(XNamespace.Xmlns + Common.Sync.Prefix, _syncNs));
            root.Add(new XAttribute(XNamespace.Xmlns + Common.Http.Prefix, _httpNs));
            root.Add(new XAttribute(XNamespace.Xmlns + Common.OpenSearch.Prefix, _openSearchNs));
            root.Add(new XAttribute(XNamespace.Xmlns + Common.Xsi.Prefix, _xsiNs));
            doc.Add(root);

            var writer = XmlWriter.Create(stream);
            doc.WriteTo(writer);
            writer.Flush();
        }

        private static XElement WriteAtomObject(object obj, INamingScheme namingScheme)
        {
            var resources = ContentHelper.AsDictionaries(obj);
            if (resources != null)
            {
                return WriteFeed(resources, namingScheme);
            }

            var resource = ContentHelper.AsDictionary(obj);
            if (resource != null)
            {
                return WriteEntry(resource, namingScheme);
            }

            return null;
        }

        private static XElement WriteFeed(IEnumerable<IDictionary<string, object>> resources, INamingScheme namingScheme)
        {
            var feed = new XElement(_atomNs + "feed");

            var prot = resources as ISDataProtocolObject;
            var info = prot != null ? prot.Info : null;
            if (info != null)
            {
                WriteElementValue(feed, _atomNs + "id", info.Id);
                WriteElementValue(feed, _atomNs + "title", info.Title);
                WriteElementValue(feed, _atomNs + "updated", info.Updated);
                WriteElementValue(feed, _openSearchNs + "totalResults", info.TotalResults);
                WriteElementValue(feed, _openSearchNs + "startIndex", info.StartIndex);
                WriteElementValue(feed, _openSearchNs + "itemsPerPage", info.ItemsPerPage);
                WriteElementValue(feed, _sDataNs + "syncMode", info.SyncMode);

                var diagnoses = info.Diagnoses;
                if (diagnoses != null && diagnoses.Count > 0)
                {
                    foreach (var diagnosis in diagnoses)
                    {
                        feed.Add(SerializeObject(diagnosis));
                    }
                }

                var schema = info.Schema;
                if (!string.IsNullOrEmpty(schema))
                {
                    feed.Add(new XElement(_sDataNs + "schema", schema));
                }

                WriteLinks(feed, info.Links);

                var syncDigest = info.SyncDigest;
                if (syncDigest != null)
                {
                    feed.Add(SerializeObject(syncDigest));
                }
            }

            feed.Add(resources.Select(resource => WriteEntry(resource, namingScheme)));
            return feed;
        }

        private static XElement WriteEntry(IEnumerable<KeyValuePair<string, object>> resource, INamingScheme namingScheme)
        {
            var entry = new XElement(_atomNs + "entry");

            var prot = resource as ISDataProtocolObject;
            var info = prot != null ? prot.Info : null;
            if (info != null)
            {
                WriteElementValue(entry, _atomNs + "id", info.Id);
                WriteElementValue(entry, _atomNs + "title", info.Title);
                WriteElementValue(entry, _atomNs + "updated", info.Updated);
                WriteElementValue(entry, _httpNs + "httpMethod", info.HttpMethod);
                WriteElementValue(entry, _httpNs + "httpStatus", (int?) info.HttpStatus);
                WriteElementValue(entry, _httpNs + "httpMessage", info.HttpMessage);
                WriteElementValue(entry, _httpNs + "location", info.Location);
                WriteElementValue(entry, _httpNs + "etag", info.ETag);
                WriteElementValue(entry, _httpNs + "ifMatch", info.IfMatch);

                var diagnoses = info.Diagnoses;
                if (diagnoses != null && diagnoses.Count > 0)
                {
                    foreach (var diagnosis in diagnoses)
                    {
                        entry.Add(SerializeObject(diagnosis));
                    }
                }

                var schema = info.Schema;
                if (!string.IsNullOrEmpty(schema))
                {
                    entry.Add(new XElement(_sDataNs + "schema", schema));
                }

                WriteLinks(entry, info.Links);

                var syncState = info.SyncState;
                if (syncState != null)
                {
                    entry.Add(SerializeObject(syncState));
                }

                if (info.XmlLocalName != null)
                {
                    var name = XName.Get(info.XmlLocalName, info.XmlNamespace ?? string.Empty);
                    entry.Add(new XElement(_sDataNs + "payload", WriteResource(name, resource, namingScheme)));
                }
            }

            return entry;
        }

        private static XElement WriteResource(XName name, IEnumerable<KeyValuePair<string, object>> resource, INamingScheme namingScheme)
        {
            var payload = new XElement(name);

            var prot = resource as ISDataProtocolObject;
            var info = prot != null ? prot.Info : null;
            if (info != null)
            {
                WriteAttributeValue(payload, _sDataNs + "key", info.Key);
                WriteAttributeValue(payload, _sDataNs + "uri", info.Url != null ? info.Url.AbsoluteUri : null);
                WriteAttributeValue(payload, _sDataNs + "uuid", info.Uuid);
                WriteAttributeValue(payload, _sDataNs + "lookup", info.Lookup);
                WriteAttributeValue(payload, _sDataNs + "descriptor", info.Descriptor);
                WriteAttributeValue(payload, _sDataNs + "isDeleted", info.IsDeleted);
            }

            payload.Add(resource.Select(item => WriteItem(name.Namespace + item.Key, item.Value, namingScheme)));
            return payload;
        }

        private static object WriteItem(XName name, object value, INamingScheme namingScheme)
        {
            var obj = WriteObject(name, value, namingScheme);
            if (obj != null)
            {
                return obj;
            }

            if (ContentHelper.IsObject(value))
            {
                obj = WriteObject(name, ContentHelper.Serialize(value, namingScheme), namingScheme);
                if (obj != null)
                {
                    return obj;
                }
            }

            return new XElement(name, value);
        }

        private static object WriteObject(XName name, object value, INamingScheme namingScheme)
        {
            if (value == null)
            {
                return new XElement(name, new XAttribute(_xsiNs + "nil", true));
            }

            if (Equals(value, string.Empty))
            {
                return new XElement(name);
            }

            var resource = ContentHelper.AsDictionary(value);
            if (resource != null)
            {
                var prot = resource as ISDataProtocolObject;
                var info = prot != null ? prot.Info : null;
                var itemName = info != null && info.XmlNamespace != null
                    ? XName.Get(name.LocalName, prot.Info.XmlNamespace)
                    : name;
                return WriteResource(itemName, resource, namingScheme);
            }

            var resources = ContentHelper.AsDictionaries(value);
            if (resources != null)
            {
                return WriteResourceCollection(name, resources, namingScheme);
            }

            var items = ContentHelper.AsCollection(value);
            if (items != null)
            {
                return WriteSimpleCollection(name, items, namingScheme);
            }

            return null;
        }

        private static object WriteResourceCollection(XName name, IEnumerable<IDictionary<string, object>> resources, INamingScheme namingScheme)
        {
            var element = new XElement(name);
            var prot = resources as ISDataProtocolObject;
            var info = prot != null ? prot.Info : null;
            var localName = info != null ? info.XmlLocalName : null;
            var xmlNs = info != null ? info.XmlNamespace : null;
            var elements = resources.Select(item =>
                                                {
                                                    var itemProt = item as ISDataProtocolObject;
                                                    var itemInfo = itemProt != null ? itemProt.Info : null;
                                                    var itemName = XName.Get(localName ?? (itemInfo != null ? itemInfo.XmlLocalName : null) ?? item.GetType().Name,
                                                                             xmlNs ?? (itemInfo != null ? itemInfo.XmlNamespace : null) ?? name.NamespaceName);
                                                    return WriteResource(itemName, item, namingScheme);
                                                });

            if (info != null)
            {
                if (info.XmlIsFlat)
                {
                    return elements;
                }

                WriteAttributeValue(element, _sDataNs + "uri", info.Url != null ? info.Url.AbsoluteUri : null);
                WriteAttributeValue(element, _sDataNs + "deleteMissing", info.DeleteMissing);
            }

            element.Add(elements);
            return element;
        }

        private static object WriteSimpleCollection(XName name, IEnumerable<object> items, INamingScheme namingScheme)
        {
            var element = new XElement(name);
            var prot = items as ISDataProtocolObject;
            var info = prot != null ? prot.Info : null;
            var localName = info != null ? info.XmlLocalName : null;
            var xmlNs = info != null ? info.XmlNamespace : null;
            element.Add(items.Select(item =>
                                         {
                                             var itemProt = item as ISDataProtocolObject;
                                             var itemInfo = itemProt != null ? itemProt.Info : null;
                                             var itemName = XName.Get(localName ?? (itemInfo != null ? itemInfo.XmlLocalName : null) ?? item.GetType().Name,
                                                                      xmlNs ?? (itemInfo != null ? itemInfo.XmlNamespace : null) ?? name.NamespaceName);
                                             return WriteItem(itemName, item, namingScheme);
                                         }));
            return element;
        }

        private static XElement SerializeObject<T>(T obj)
        {
            var doc = new XDocument();
            using (var writer = doc.CreateWriter())
            {
                new XmlSerializer(typeof (T)).Serialize(writer, obj);
            }
            return doc.Root;
        }

        private static void WriteLinks(XContainer container, IEnumerable<SDataLink> links)
        {
            if (links != null)
            {
                foreach (var link in links)
                {
                    var element = new XElement(_atomNs + "link");
                    WriteAttributeValue(element, "href", link.Uri != null ? link.Uri.AbsoluteUri : null);
                    WriteAttributeValue(element, "rel", link.Relation);
                    WriteAttributeValue(element, "type", link.Type != null ? MediaTypeNames.GetMediaType(link.Type.Value) : null);
                    WriteAttributeValue(element, "title", link.Title);
                    container.Add(element);
                }
            }
        }

        private static void WriteElementValue(XContainer container, XName name, object value)
        {
            if (value != null)
            {
                container.Add(new XElement(name, value));
            }
        }

        private static void WriteAttributeValue(XContainer container, XName name, object value)
        {
            if (value != null)
            {
                container.Add(new XAttribute(name, value));
            }
        }
    }
}