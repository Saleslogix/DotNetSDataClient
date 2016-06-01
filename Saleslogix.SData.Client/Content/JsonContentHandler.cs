// Copyright (c) 1997-2014, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using Saleslogix.SData.Client.Framework;
using Saleslogix.SData.Client.Utilities;
using SimpleJson;

namespace Saleslogix.SData.Client.Content
{
    public class JsonContentHandler : IContentHandler
    {
        private static readonly Regex _microsoftDateFormat = new Regex(
            @"^\\?/Date\((-?\d+)(-|\+)?([0-9]{4})?\)\\?/$",
            RegexOptions.IgnoreCase |
            RegexOptions.CultureInvariant |
            RegexOptions.IgnorePatternWhitespace
#if !PCL && !NETFX_CORE && !SILVERLIGHT
            | RegexOptions.Compiled
#endif
            );

        private readonly SerializerStrategy _serializerStrategy = new SerializerStrategy();

        public object ReadFrom(Stream stream)
        {
            Guard.ArgumentNotNull(stream, "stream");

            object obj;
            using (var reader = new StreamReader(stream))
            {
                obj = SimpleJson.SimpleJson.DeserializeObject(reader.ReadToEnd());
            }

            RecursivelyReplaceMicrosoftDates(ref obj);
            return ReadObject(obj);
        }

        private static object ReadObject(object obj)
        {
            var jsonObj = ContentHelper.AsDictionary(obj);
            if (jsonObj != null)
            {
                object resourcesObj;
                if (jsonObj.TryGetValue("$resources", out resourcesObj) ||
                    ((jsonObj.ContainsKey("$url") || jsonObj.ContainsKey("$deleteMissing")) &&
                     !jsonObj.ContainsKey("$key") && !jsonObj.ContainsKey("$uuid") && !jsonObj.ContainsKey("$lookup") && !jsonObj.ContainsKey("$descriptor") && !jsonObj.ContainsKey("$isDeleted")))
                {
                    var jsonResources = ContentHelper.AsDictionaries(resourcesObj);
                    return ReadResourceCollection(jsonObj, jsonResources ?? new List<IDictionary<string, object>>());
                }

                return ReadResource(jsonObj);
            }

            var jsonItems = ContentHelper.AsCollection(obj);
            if (jsonItems != null)
            {
                return ReadSimpleCollection(jsonItems);
            }

            return obj;
        }

        private static SDataResource ReadResource(IDictionary<string, object> obj)
        {
            var resource = new SDataResource
                {
                    Id = ReadProtocolValue<string>(obj, "id"),
                    Title = ReadProtocolValue<string>(obj, "title"),
                    Updated = ReadProtocolValue<DateTimeOffset?>(obj, "updated"),
                    HttpMethod = ReadProtocolValue<HttpMethod?>(obj, "httpMethod", true),
                    HttpStatus = ReadProtocolValue<HttpStatusCode?>(obj, "httpStatus"),
                    HttpMessage = ReadProtocolValue<string>(obj, "httpMessage"),
                    Location = ReadProtocolValue<string>(obj, "location"),
                    ETag = ReadProtocolValue<string>(obj, "etag"),
                    IfMatch = ReadProtocolValue<string>(obj, "ifMatch"),
                    Url = ReadProtocolValue<Uri>(obj, "url"),
                    Key = ReadProtocolValue<string>(obj, "key"),
                    Uuid = ReadProtocolValue<Guid?>(obj, "uuid"),
                    Lookup = ReadProtocolValue<string>(obj, "lookup"),
                    Descriptor = ReadProtocolValue<string>(obj, "descriptor"),
                    Links = ReadLinks(obj),
                    IsDeleted = ReadProtocolValue<bool?>(obj, "isDeleted")
                };

            object value;
            if (obj.TryGetValue("$diagnoses", out value))
            {
                resource.Diagnoses = ContentHelper.Deserialize<Diagnoses>(value);
            }
            if (obj.TryGetValue("$syncState", out value))
            {
                resource.SyncState = ContentHelper.Deserialize<SyncState>(value);
            }

            foreach (var item in obj.Where(item => !item.Key.StartsWith("$", StringComparison.Ordinal)))
            {
                resource[item.Key] = ReadObject(item.Value);
            }

            return resource;
        }

        private static SDataCollection<SDataResource> ReadResourceCollection(IDictionary<string, object> obj, IEnumerable<IDictionary<string, object>> items)
        {
            var collection = new SDataCollection<SDataResource>(items.Select(ReadResource))
                {
                    Id = ReadProtocolValue<string>(obj, "id"),
                    Title = ReadProtocolValue<string>(obj, "title"),
                    Updated = ReadProtocolValue<DateTimeOffset?>(obj, "updated"),
                    TotalResults = ReadProtocolValue<int?>(obj, "totalResults"),
                    StartIndex = ReadProtocolValue<int?>(obj, "startIndex"),
                    ItemsPerPage = ReadProtocolValue<int?>(obj, "itemsPerPage"),
                    Url = ReadProtocolValue<Uri>(obj, "url"),
                    DeleteMissing = ReadProtocolValue<bool?>(obj, "deleteMissing"),
                    Links = ReadLinks(obj),
                    SyncMode = ReadProtocolValue<SyncMode?>(obj, "syncMode")
                };

            object diagnoses;
            if (obj.TryGetValue("$diagnoses", out diagnoses))
            {
                collection.Diagnoses = ContentHelper.Deserialize<Diagnoses>(diagnoses);
            }

            object digest;
            if (obj.TryGetValue("$digest", out digest))
            {
                collection.SyncDigest = ContentHelper.Deserialize<Digest>(digest);
            }

            return collection;
        }

        private static object ReadSimpleCollection(IEnumerable<object> items)
        {
            var output = ContentHelper.ToTypedCollection(items.Select(ReadObject));
            ((ISDataProtocolObject) output).Info.JsonIsSimpleArray = true;
            return output;
        }

        private static IList<SDataLink> ReadLinks(IEnumerable<KeyValuePair<string, object>> obj)
        {
            var links = new List<SDataLink>();
            foreach (var pair in obj)
            {
                var str = pair.Value as string;
                if (pair.Key.StartsWith("$", StringComparison.Ordinal) && str != null)
                {
                    var name = pair.Key.Substring(1);
                    Uri uri;
                    if ((name == "schema" || !Enum.GetNames(typeof (SDataProtocolProperty)).Any(item => string.Equals(item, name, StringComparison.OrdinalIgnoreCase))) &&
                        Uri.TryCreate(str, UriKind.Absolute, out uri))
                    {
                        links.Add(new SDataLink {Uri = uri, Relation = name});
                    }
                }
            }
            return links;
        }

        private static T ReadProtocolValue<T>(IDictionary<string, object> obj, string name, bool ignoreCase = false)
        {
            object value;
            if (!obj.TryGetValue("$" + name, out value))
            {
                return default(T);
            }

            var type = typeof (T);
            type = Nullable.GetUnderlyingType(type) ?? type;
            if (type == typeof (Guid))
            {
                var str = value as string;
                if (str != null)
                {
                    value = new Guid(str);
                }
            }
            else if (type == typeof (Uri))
            {
                var str = value as string;
                if (str != null)
                {
                    value = new Uri(str);
                }
            }
            else if (type.GetTypeInfo().IsEnum)
            {
                var str = value as string;
                if (str != null)
                {
                    value = EnumEx.Parse(type, str, ignoreCase);
                }
                else
                {
                    value = Enum.ToObject(type, value);
                }
            }

            return (T) Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
        }

        private static bool RecursivelyReplaceMicrosoftDates(ref object obj)
        {
            var jsonObj = obj as JsonObject;
            if (jsonObj != null)
            {
                foreach (var item in jsonObj.ToList())
                {
                    var value = item.Value;
                    if (RecursivelyReplaceMicrosoftDates(ref value))
                    {
                        jsonObj[item.Key] = value;
                    }
                }
                return false;
            }

            var jsonItems = obj as JsonArray;
            if (jsonItems != null)
            {
                for (var i = 0; i < jsonItems.Count; i++)
                {
                    var value = jsonItems[i];
                    if (RecursivelyReplaceMicrosoftDates(ref value))
                    {
                        jsonItems[i] = value;
                    }
                }
                return false;
            }

            var str = obj as string;
            if (str != null)
            {
                DateTimeOffset date;
                if (TryParseMicrosoftDate(str, out date))
                {
                    obj = date;
                    return true;
                }
            }

            return false;
        }

        private static bool TryParseMicrosoftDate(string value, out DateTimeOffset date)
        {
            var match = _microsoftDateFormat.Match(value);
            if (!match.Success)
            {
                date = DateTimeOffset.MinValue;
                return false;
            }

            var ms = long.Parse(match.Groups[1].Value);
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0);
            var dt = epoch.AddMilliseconds(ms);

            if (match.Groups.Count < 3 || string.IsNullOrEmpty(match.Groups[3].Value))
            {
                date = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                return true;
            }

            var mod = DateTime.ParseExact(match.Groups[3].Value, "HHmm", CultureInfo.InvariantCulture);
            var offset = match.Groups[2].Value == "+" ? mod.TimeOfDay : -mod.TimeOfDay;
            date = new DateTimeOffset(dt, offset);
            return true;
        }

        public void WriteTo(object obj, Stream stream, INamingScheme namingScheme = null)
        {
            Guard.ArgumentNotNull(obj, "obj");
            Guard.ArgumentNotNull(stream, "stream");

            if (namingScheme == null)
            {
                namingScheme = NamingScheme.Default;
            }

            var root = WriteItem(obj, true, namingScheme) ?? WriteItem(ContentHelper.Serialize(obj, namingScheme), true, namingScheme);
            var json = SimpleJson.SimpleJson.SerializeObject(root, _serializerStrategy);
            var writer = new StreamWriter(stream);
            writer.Write(json);
            writer.Flush();
        }

        private static object WriteItem(object obj, bool isRoot, INamingScheme namingScheme)
        {
            var jsonObj = WriteObject(obj, isRoot, namingScheme);
            if (jsonObj != null)
            {
                return jsonObj;
            }

            if (ContentHelper.IsObject(obj))
            {
                jsonObj = WriteObject(ContentHelper.Serialize(obj, namingScheme), isRoot, namingScheme);
                if (jsonObj != null)
                {
                    return jsonObj;
                }
            }

            return obj;
        }

        private static object WriteObject(object obj, bool isRoot, INamingScheme namingScheme)
        {
            var resource = ContentHelper.AsDictionary(obj);
            if (resource != null)
            {
                return WriteResource(resource, namingScheme);
            }

            var resources = ContentHelper.AsDictionaries(obj);
            if (resources != null)
            {
                var prot = resources as ISDataProtocolObject;
                if (prot != null && prot.Info != null && prot.Info.JsonIsSimpleArray)
                {
#if NET_2_0 || NET_3_5
                    return WriteSimpleCollection(resources.Cast<object>(), namingScheme);
#else
                    return WriteSimpleCollection(resources, namingScheme);
#endif
                }
                return WriteResourceCollection(resources, namingScheme);
            }

            if (!isRoot)
            {
                var items = ContentHelper.AsCollection(obj);
                if (items != null)
                {
                    return WriteSimpleCollection(items, namingScheme);
                }
            }

            return null;
        }

        private static IDictionary<string, object> WriteResource(IEnumerable<KeyValuePair<string, object>> resource, INamingScheme namingScheme)
        {
            var obj = new Dictionary<string, object>();

            var prot = resource as ISDataProtocolObject;
            var info = prot != null ? prot.Info : null;
            if (info != null)
            {
                WriteProtocolValue(obj, "id", info.Id);
                WriteProtocolValue(obj, "title", info.Title);
                WriteProtocolValue(obj, "updated", info.Updated);
                WriteProtocolValue(obj, "location", info.Location);
                WriteProtocolValue(obj, "etag", info.ETag);
                WriteProtocolValue(obj, "ifMatch", info.IfMatch);
                WriteProtocolValue(obj, "httpMethod", info.HttpMethod != null ? EnumEx.Format(info.HttpMethod).ToUpperInvariant() : null);
                WriteProtocolValue(obj, "httpStatus", (int?) info.HttpStatus);
                WriteProtocolValue(obj, "httpMessage", info.HttpMessage);
                WriteProtocolValue(obj, "key", info.Key);
                WriteProtocolValue(obj, "url", info.Url != null ? info.Url.AbsoluteUri : null);
                WriteProtocolValue(obj, "uuid", info.Uuid);
                WriteProtocolValue(obj, "lookup", info.Lookup);
                WriteProtocolValue(obj, "descriptor", info.Descriptor);
                WriteLinks(obj, info.Links);
                WriteProtocolValue(obj, "isDeleted", info.IsDeleted);

                var diagnoses = info.Diagnoses;
                if (diagnoses != null && diagnoses.Count > 0)
                {
                    obj["$diagnoses"] = ContentHelper.Serialize(diagnoses, namingScheme);
                }

                var syncState = info.SyncState;
                if (syncState != null)
                {
                    obj["$syncState"] = ContentHelper.Serialize(syncState, namingScheme);
                }
            }

            foreach (var item in resource)
            {
                obj[item.Key] = WriteItem(item.Value, false, namingScheme);
            }

            return obj;
        }

        private static IDictionary<string, object> WriteResourceCollection(IEnumerable<IDictionary<string, object>> collection, INamingScheme namingScheme)
        {
            var obj = new Dictionary<string, object>();

            var prot = collection as ISDataProtocolObject;
            var info = prot != null ? prot.Info : null;
            if (info != null)
            {
                WriteProtocolValue(obj, "id", info.Id);
                WriteProtocolValue(obj, "title", info.Title);
                WriteProtocolValue(obj, "updated", info.Updated);
                WriteProtocolValue(obj, "totalResults", info.TotalResults);
                WriteProtocolValue(obj, "startIndex", info.StartIndex);
                WriteProtocolValue(obj, "itemsPerPage", info.ItemsPerPage);
                WriteProtocolValue(obj, "url", info.Url != null ? info.Url.AbsoluteUri : null);
                WriteProtocolValue(obj, "deleteMissing", info.DeleteMissing);
                WriteLinks(obj, info.Links);
                WriteProtocolValue(obj, "syncMode", info.SyncMode);

                var diagnoses = info.Diagnoses;
                if (diagnoses != null && diagnoses.Count > 0)
                {
                    obj["$diagnoses"] = ContentHelper.Serialize(diagnoses, namingScheme);
                }

                var syncDigest = info.SyncDigest;
                if (syncDigest != null)
                {
                    obj["$digest"] = ContentHelper.Serialize(syncDigest, namingScheme);
                }
            }

            obj["$resources"] = collection.Select(item => (object) WriteResource(item, namingScheme)).ToList();
            return obj;
        }

        private static ICollection<object> WriteSimpleCollection(IEnumerable<object> items, INamingScheme namingScheme)
        {
            return items.Select(item => WriteItem(item, false, namingScheme)).ToArray();
        }

        private static void WriteLinks(IDictionary<string, object> obj, IEnumerable<SDataLink> links)
        {
            if (links != null)
            {
                foreach (var link in links)
                {
                    obj["$" + link.Relation] = link.Uri != null ? link.Uri.AbsoluteUri : null;
                }
            }
        }

        private static void WriteProtocolValue(IDictionary<string, object> obj, string name, object value)
        {
            if (value != null)
            {
                obj["$" + name] = value;
            }
        }

        private static string FormatMicrosoftDate(DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var ms = (long) (date - epoch).TotalMilliseconds;
            return string.Format(@"/Date({0})/", ms);
        }

        private static string FormatMicrosoftDate(DateTimeOffset date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var ms = (long) (date - epoch).TotalMilliseconds;
            var sign = date.Offset >= TimeSpan.Zero ? "+" : "-";
            return string.Format(@"/Date({0}{1}{2:00}{3:00})/", ms, sign, date.Offset.Hours, date.Offset.Minutes);
        }

        #region Nested type: SerializerStrategy

        private class SerializerStrategy : PocoJsonSerializerStrategy
        {
            protected override object SerializeEnum(Enum value)
            {
                return value.ToString();
            }

            protected override bool TrySerializeKnownTypes(object input, out object output)
            {
                if (input is DateTime)
                {
                    output = FormatMicrosoftDate((DateTime) input);
                    return true;
                }
                if (input is DateTimeOffset)
                {
                    output = FormatMicrosoftDate((DateTimeOffset) input);
                    return true;
                }
                if (input is char || input is TimeSpan || input is Version)
                {
                    output = input.ToString();
                    return true;
                }

                return base.TrySerializeKnownTypes(input, out output);
            }
        }

        #endregion
    }
}