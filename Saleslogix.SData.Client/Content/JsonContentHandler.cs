using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using Sage.SData.Client.Framework;
using Sage.SData.Client.Utilities;
using SimpleJson;

namespace Sage.SData.Client.Content
{
    public class JsonContentHandler : IContentHandler
    {
        private static readonly Regex _microsoftDateFormat = new Regex(
            @"\\?/Date\((-?\d+)(-|\+)?([0-9]{4})?\)\\?/",
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

            return ReadObject(obj);
        }

        private static object ReadObject(object obj)
        {
            var jsonObj = ContentHelper.AsDictionary(obj);
            if (jsonObj != null)
            {
                object resourcesObj;
                if (jsonObj.TryGetValue("$resources", out resourcesObj))
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
                                   HttpMethod = ReadProtocolValue<HttpMethod?>(obj, "httpMethod"),
                                   HttpStatus = (HttpStatusCode?) ReadProtocolValue<long?>(obj, "httpStatus"),
                                   HttpMessage = ReadProtocolValue<string>(obj, "httpMessage"),
                                   Location = ReadProtocolValue<string>(obj, "location"),
                                   ETag = ReadProtocolValue<string>(obj, "etag"),
                                   IfMatch = ReadProtocolValue<string>(obj, "ifMatch"),
                                   Url = ReadProtocolValue<Uri>(obj, "url"),
                                   Key = ReadProtocolValue<string>(obj, "key"),
                                   Uuid = ReadProtocolValue<Guid?>(obj, "uuid"),
                                   Lookup = ReadProtocolValue<string>(obj, "lookup"),
                                   Descriptor = ReadProtocolValue<string>(obj, "descriptor"),
                                   Schema = ReadProtocolValue<string>(obj, "schema"),
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

            foreach (var item in obj.Where(item => !item.Key.StartsWith("$")))
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
                                     Schema = ReadProtocolValue<string>(obj, "schema"),
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

        private static SDataCollection<object> ReadSimpleCollection(IEnumerable<object> items)
        {
            return new SDataCollection<object>(items.Select(ReadObject));
        }

        private static T ReadProtocolValue<T>(IDictionary<string, object> obj, string name)
        {
            object value;
            if (!obj.TryGetValue("$" + name, out value))
            {
                return default(T);
            }

            var type = typeof (T);
            type = Nullable.GetUnderlyingType(type) ?? type;
            if (type == typeof (DateTime) || type == typeof (DateTimeOffset))
            {
                var str = value as string;
                DateTimeOffset date;
                if (str != null && TryParseMicrosoftDate(str, out date))
                {
                    value = date;
                }
            }
            else if (type == typeof (Guid))
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
                    value = EnumEx.Parse(type, str);
                }
            }

            return (T) Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
        }

        private static bool TryParseMicrosoftDate(string value, out DateTimeOffset date)
        {
            if (!_microsoftDateFormat.IsMatch(value))
            {
                date = DateTimeOffset.MinValue;
                return false;
            }

            var matches = _microsoftDateFormat.Matches(value);
            var match = matches[0];
            var ms = Convert.ToInt64(match.Groups[1].Value);
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var dt = epoch.AddMilliseconds(ms);

            if (match.Groups.Count < 3 || string.IsNullOrEmpty(match.Groups[3].Value))
            {
                date = dt;
                return true;
            }

            var mod = DateTime.ParseExact(match.Groups[3].Value, "HHmm", CultureInfo.InvariantCulture);
            var offset = match.Groups[2].Value == "+" ? mod.TimeOfDay : -mod.TimeOfDay;
            date = new DateTimeOffset(dt.ToLocalTime(), offset);
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

            if (obj != null && !(obj is string) && !obj.GetType().GetTypeInfo().IsValueType)
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

            var prot = resource as ISDataProtocolAware;
            if (prot != null)
            {
                WriteProtocolValue(obj, "id", prot.Info.Id);
                WriteProtocolValue(obj, "title", prot.Info.Title);
                WriteProtocolValue(obj, "updated", prot.Info.Updated);
                WriteProtocolValue(obj, "location", prot.Info.Location);
                WriteProtocolValue(obj, "etag", prot.Info.ETag);
                WriteProtocolValue(obj, "ifMatch", prot.Info.IfMatch);
                WriteProtocolValue(obj, "httpMethod", prot.Info.HttpMethod);
                WriteProtocolValue(obj, "httpStatus", prot.Info.HttpStatus);
                WriteProtocolValue(obj, "httpMessage", prot.Info.HttpMessage);
                WriteProtocolValue(obj, "key", prot.Info.Key);
                WriteProtocolValue(obj, "url", prot.Info.Url != null ? prot.Info.Url.AbsoluteUri : null);
                WriteProtocolValue(obj, "uuid", prot.Info.Uuid);
                WriteProtocolValue(obj, "lookup", prot.Info.Lookup);
                WriteProtocolValue(obj, "descriptor", prot.Info.Descriptor);
                WriteProtocolValue(obj, "schema", prot.Info.Schema);
                WriteProtocolValue(obj, "isDeleted", prot.Info.IsDeleted);

                var diagnoses = prot.Info.Diagnoses;
                if (diagnoses != null && diagnoses.Count > 0)
                {
                    obj["$diagnoses"] = ContentHelper.Serialize(diagnoses, namingScheme);
                }

                var syncState = prot.Info.SyncState;
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

            var prot = collection as ISDataProtocolAware;
            if (prot != null)
            {
                WriteProtocolValue(obj, "id", prot.Info.Id);
                WriteProtocolValue(obj, "title", prot.Info.Title);
                WriteProtocolValue(obj, "updated", prot.Info.Updated);
                WriteProtocolValue(obj, "totalResults", prot.Info.TotalResults);
                WriteProtocolValue(obj, "startIndex", prot.Info.StartIndex);
                WriteProtocolValue(obj, "itemsPerPage", prot.Info.ItemsPerPage);
                WriteProtocolValue(obj, "url", prot.Info.Url != null ? prot.Info.Url.AbsoluteUri : null);
                WriteProtocolValue(obj, "deleteMissing", prot.Info.DeleteMissing);
                WriteProtocolValue(obj, "schema", prot.Info.Schema);
                WriteProtocolValue(obj, "syncMode", prot.Info.SyncMode);

                var diagnoses = prot.Info.Diagnoses;
                if (diagnoses != null && diagnoses.Count > 0)
                {
                    obj["$diagnoses"] = ContentHelper.Serialize(diagnoses, namingScheme);
                }

                var syncDigest = prot.Info.SyncDigest;
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

                return base.TrySerializeKnownTypes(input, out output);
            }
        }

        #endregion
    }
}