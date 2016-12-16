using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using NUnit.Framework;
using Saleslogix.SData.Client.Content;
using Saleslogix.SData.Client.Framework;

#if !NET_2_0
using System.Runtime.Serialization;
#endif

// ReSharper disable InconsistentNaming

namespace Saleslogix.SData.Client.Test.Content
{
    [TestFixture]
    public class ContentHelperTests
    {
        [Test]
        public void Serialize_SDataProtocolObject_Test()
        {
            var value = new SDataProtocolObject_Object {Info = new SDataProtocolInfo {XmlLocalName = "contact"}};
            var result = ContentHelper.Serialize(value);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.SameAs(value));
            Assert.That(result, Is.InstanceOf<ISDataProtocolObject>());
            Assert.That(((ISDataProtocolObject) result).Info.XmlLocalName, Is.EqualTo("contact"));
        }

        [Test]
        public void Deserialize_SDataProtocolObject_Test()
        {
            var value = new SDataResource {XmlLocalName = "contact"};
            var result = ContentHelper.Deserialize<SDataProtocolObject_Object>(value);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.SameAs(value));
            Assert.That(result, Is.InstanceOf<ISDataProtocolObject>());
            Assert.That(result.Info.XmlLocalName, Is.EqualTo("contact"));
        }

        private class SDataProtocolObject_Object : ISDataProtocolObject
        {
            public SDataProtocolInfo Info { get; set; }
        }

        [Test]
        public void Serialize_SDataProtocolObject_Collection_Test()
        {
            var value = new SDataProtocolObject_Collection {Info = new SDataProtocolInfo {XmlLocalName = "contact"}};
            var result = ContentHelper.Serialize(value);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.SameAs(value));
            Assert.That(result, Is.InstanceOf<ISDataProtocolObject>());
            Assert.That(((ISDataProtocolObject) result).Info.XmlLocalName, Is.EqualTo("contact"));
        }

        [Test]
        public void Deserialize_SDataProtocolObject_Collection_Test()
        {
            var value = new SDataCollection<object> {XmlLocalName = "contact"};
            var result = ContentHelper.Deserialize<SDataProtocolObject_Collection>(value);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.SameAs(value));
            Assert.That(result, Is.InstanceOf<ISDataProtocolObject>());
            Assert.That(result.Info.XmlLocalName, Is.EqualTo("contact"));
        }

        private class SDataProtocolObject_Collection : List<object>, ISDataProtocolObject
        {
            public SDataProtocolInfo Info { get; set; }
        }

#if !NET_2_0
        [Test]
        public void Serialize_DataContractAttribute_Test()
        {
            var value = new DataContractAttribute_Object();
            var result = ContentHelper.Serialize(value);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ISDataProtocolObject>());
            Assert.That(((ISDataProtocolObject) result).Info.XmlLocalName, Is.EqualTo("contact"));
        }

        [DataContract(Name = "contact")]
        private class DataContractAttribute_Object
        {
        }
#endif

        [Test]
        public void Serialize_Enumerable_Test()
        {
            var value = new[] {"Smith"}.Select(name => new {name});
            var result = ContentHelper.Serialize(value);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<IEnumerable>());
            Assert.That(result, Has.Count.EqualTo(1));
            var item = ((IEnumerable) result).Cast<object>().First();
            Assert.That(item, Is.InstanceOf<IDictionary<string, object>>());
            Assert.That(((IDictionary<string, object>) item)["name"], Is.EqualTo("Smith"));
        }

        [Test]
        public void Deserialize_Enumerable_Test()
        {
            var value = new[] {"Smith"}.Select(name => new Dictionary<string, object> {{"name", name}});
            var result = ContentHelper.Deserialize<IList<IDictionary<string, object>>>(value);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.SameAs(value));
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0]["name"], Is.EqualTo("Smith"));
        }

        [Test]
        public void Deserialize_ProtocolObject_Enumerable_Test()
        {
            var value = new SDataCollection<string> {"Smith"};
            ((ISDataProtocolObject) value).Info = new SDataProtocolInfo {Url = new Uri("http://www.example.com")};
            var result = ContentHelper.Deserialize<IList<object>>(value);
            Assert.That(result, Is.InstanceOf<ISDataProtocolObject>());
            Assert.That(result, Is.Not.SameAs(value));
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0], Is.EqualTo("Smith"));
            Assert.That(((ISDataProtocolObject) result).Info.Url, Is.EqualTo(new Uri("http://www.example.com")));
        }

        [Test]
        public void Serialize_SDataProtocolProperty_Test()
        {
            var value = new SDataProtocolProperty_Object
                            {
                                Id = "id",
                                Title = "title",
                                Updated = new DateTimeOffset(2001, 1, 1, 1, 1, 1, TimeSpan.FromHours(1)),
                                Url = new Uri("http://localhost"),
                                Diagnoses = new Diagnoses {new Diagnosis {SDataCode = DiagnosisCode.BadUrlSyntax}},
                                Schema = "schema",
                                Key = "key",
                                Uuid = new Guid("6787D975-EC01-4FCA-AA4E-2402968BC911"),
                                Lookup = "lookup",
                                Descriptor = "descriptor",
                                HttpMethod = HttpMethod.Post,
                                HttpStatus = HttpStatusCode.BadRequest,
                                HttpMessage = "message",
                                Location = "location",
                                ETag = "etag",
                                IfMatch = "ifmatch",
                                IsDeleted = true,
                                SyncState = new SyncState {EndPoint = "endpoint"},
                                TotalResults = 1,
                                StartIndex = 2,
                                ItemsPerPage = 3,
                                DeleteMissing = true,
                                SyncMode = SyncMode.CatchUp,
                                SyncDigest = new Digest {Origin = "origin"}
                            };

            var result = ContentHelper.Serialize(value);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ISDataProtocolObject>());
            var info = ((ISDataProtocolObject) result).Info;
            Assert.That(info.Id, Is.EqualTo("id"));
            Assert.That(info.Title, Is.EqualTo("title"));
            Assert.That(info.Updated, Is.EqualTo(new DateTimeOffset(2001, 1, 1, 1, 1, 1, TimeSpan.FromHours(1))));
            Assert.That(info.Url, Is.EqualTo(new Uri("http://localhost")));
            Assert.That(info.Diagnoses[0].SDataCode, Is.EqualTo(DiagnosisCode.BadUrlSyntax));
            Assert.That(info.Schema, Is.EqualTo("schema"));
            Assert.That(info.Key, Is.EqualTo("key"));
            Assert.That(info.Uuid, Is.EqualTo(new Guid("6787D975-EC01-4FCA-AA4E-2402968BC911")));
            Assert.That(info.Lookup, Is.EqualTo("lookup"));
            Assert.That(info.Descriptor, Is.EqualTo("descriptor"));
            Assert.That(info.HttpMethod, Is.EqualTo(HttpMethod.Post));
            Assert.That(info.HttpStatus, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(info.HttpMessage, Is.EqualTo("message"));
            Assert.That(info.Location, Is.EqualTo("location"));
            Assert.That(info.ETag, Is.EqualTo("etag"));
            Assert.That(info.IfMatch, Is.EqualTo("ifmatch"));
            Assert.That(info.IsDeleted, Is.True);
            Assert.That(info.SyncState.EndPoint, Is.EqualTo("endpoint"));
            Assert.That(info.TotalResults, Is.EqualTo(1));
            Assert.That(info.StartIndex, Is.EqualTo(2));
            Assert.That(info.ItemsPerPage, Is.EqualTo(3));
            Assert.That(info.DeleteMissing, Is.True);
            Assert.That(info.SyncMode, Is.EqualTo(SyncMode.CatchUp));
            Assert.That(info.SyncDigest.Origin, Is.EqualTo("origin"));
        }

        [Test]
        public void Deserialize_SDataProtocolProperty_Test()
        {
            var value = new SDataResource
                            {
                                Id = "id",
                                Title = "title",
                                Updated = new DateTimeOffset(2001, 1, 1, 1, 1, 1, TimeSpan.FromHours(1)),
                                Url = new Uri("http://localhost"),
                                Diagnoses = new Diagnoses {new Diagnosis {SDataCode = DiagnosisCode.BadUrlSyntax}},
                                Schema = "schema",
                                Key = "key",
                                Uuid = new Guid("6787D975-EC01-4FCA-AA4E-2402968BC911"),
                                Lookup = "lookup",
                                Descriptor = "descriptor",
                                HttpMethod = HttpMethod.Post,
                                HttpStatus = HttpStatusCode.BadRequest,
                                HttpMessage = "message",
                                Location = "location",
                                ETag = "etag",
                                IfMatch = "ifmatch",
                                IsDeleted = true,
                                SyncState = new SyncState {EndPoint = "endpoint"},
                                XmlLocalName = "xmllocalname",
                                XmlNamespace = "xmlnamespace"
                            };
            var info = ((ISDataProtocolObject) value).Info;
            info.TotalResults = 1;
            info.StartIndex = 2;
            info.ItemsPerPage = 3;
            info.DeleteMissing = true;
            info.SyncMode = SyncMode.CatchUp;
            info.SyncDigest = new Digest {Origin = "origin"};
            var result = ContentHelper.Deserialize<SDataProtocolProperty_Object>(value);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.SameAs(value));
            Assert.That(result.Id, Is.EqualTo("id"));
            Assert.That(result.Title, Is.EqualTo("title"));
            Assert.That(result.Updated, Is.EqualTo(new DateTimeOffset(2001, 1, 1, 1, 1, 1, TimeSpan.FromHours(1))));
            Assert.That(result.Url, Is.EqualTo(new Uri("http://localhost")));
            Assert.That(result.Diagnoses[0].SDataCode, Is.EqualTo(DiagnosisCode.BadUrlSyntax));
            Assert.That(result.Schema, Is.EqualTo("schema"));
            Assert.That(result.Key, Is.EqualTo("key"));
            Assert.That(result.Uuid, Is.EqualTo(new Guid("6787D975-EC01-4FCA-AA4E-2402968BC911")));
            Assert.That(result.Lookup, Is.EqualTo("lookup"));
            Assert.That(result.Descriptor, Is.EqualTo("descriptor"));
            Assert.That(result.HttpMethod, Is.EqualTo(HttpMethod.Post));
            Assert.That(result.HttpStatus, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(result.HttpMessage, Is.EqualTo("message"));
            Assert.That(result.Location, Is.EqualTo("location"));
            Assert.That(result.ETag, Is.EqualTo("etag"));
            Assert.That(result.IfMatch, Is.EqualTo("ifmatch"));
            Assert.That(result.IsDeleted, Is.True);
            Assert.That(result.SyncState.EndPoint, Is.EqualTo("endpoint"));
            Assert.That(result.TotalResults, Is.EqualTo(1));
            Assert.That(result.StartIndex, Is.EqualTo(2));
            Assert.That(result.ItemsPerPage, Is.EqualTo(3));
            Assert.That(result.DeleteMissing, Is.True);
            Assert.That(result.SyncMode, Is.EqualTo(SyncMode.CatchUp));
            Assert.That(result.SyncDigest.Origin, Is.EqualTo("origin"));
        }

        private class SDataProtocolProperty_Object
        {
            [SDataProtocolProperty(SDataProtocolProperty.Id)]
            public string Id { get; set; }

            [SDataProtocolProperty(SDataProtocolProperty.Title)]
            public string Title { get; set; }

            [SDataProtocolProperty(SDataProtocolProperty.Updated)]
            public DateTimeOffset? Updated { get; set; }

            [SDataProtocolProperty(SDataProtocolProperty.Url)]
            public Uri Url { get; set; }

            [SDataProtocolProperty(SDataProtocolProperty.Diagnoses)]
            public Diagnoses Diagnoses { get; set; }

            [SDataProtocolProperty(SDataProtocolProperty.Schema)]
            public string Schema { get; set; }

            [SDataProtocolProperty(SDataProtocolProperty.Key)]
            public string Key { get; set; }

            [SDataProtocolProperty(SDataProtocolProperty.Uuid)]
            public Guid? Uuid { get; set; }

            [SDataProtocolProperty(SDataProtocolProperty.Lookup)]
            public string Lookup { get; set; }

            [SDataProtocolProperty(SDataProtocolProperty.Descriptor)]
            public string Descriptor { get; set; }

            [SDataProtocolProperty(SDataProtocolProperty.HttpMethod)]
            public HttpMethod? HttpMethod { get; set; }

            [SDataProtocolProperty(SDataProtocolProperty.HttpStatus)]
            public HttpStatusCode? HttpStatus { get; set; }

            [SDataProtocolProperty(SDataProtocolProperty.HttpMessage)]
            public string HttpMessage { get; set; }

            [SDataProtocolProperty(SDataProtocolProperty.Location)]
            public string Location { get; set; }

            [SDataProtocolProperty(SDataProtocolProperty.ETag)]
            public string ETag { get; set; }

            [SDataProtocolProperty(SDataProtocolProperty.IfMatch)]
            public string IfMatch { get; set; }

            [SDataProtocolProperty(SDataProtocolProperty.IsDeleted)]
            public bool? IsDeleted { get; set; }

            [SDataProtocolProperty(SDataProtocolProperty.SyncState)]
            public SyncState SyncState { get; set; }

            [SDataProtocolProperty(SDataProtocolProperty.TotalResults)]
            public int? TotalResults { get; set; }

            [SDataProtocolProperty(SDataProtocolProperty.StartIndex)]
            public int? StartIndex { get; set; }

            [SDataProtocolProperty(SDataProtocolProperty.ItemsPerPage)]
            public int? ItemsPerPage { get; set; }

            [SDataProtocolProperty(SDataProtocolProperty.DeleteMissing)]
            public bool? DeleteMissing { get; set; }

            [SDataProtocolProperty(SDataProtocolProperty.SyncMode)]
            public SyncMode? SyncMode { get; set; }

            [SDataProtocolProperty(SDataProtocolProperty.SyncDigest)]
            public Digest SyncDigest { get; set; }
        }

        [Test]
        public void Serialize_Field_Test()
        {
            var value = new Field_Object {Name = "Smith"};
            var result = ContentHelper.Serialize(value);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<IDictionary<string, object>>());
            Assert.That(((IDictionary<string, object>) result)["Name"], Is.EqualTo("Smith"));
        }

        [Test]
        public void Deserialize_Field_Test()
        {
            var value = new Dictionary<string, object> {{"Name", "Smith"}};
            var result = ContentHelper.Deserialize<Field_Object>(value);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.SameAs(value));
            Assert.That(result.Name, Is.EqualTo("Smith"));
        }

        private class Field_Object
        {
            public string Name;
        }

        [Test]
        public void Deserialize_NumberTypeMismatch_FromInteger_Test()
        {
            var value = new Dictionary<string, object>
                {
                    {"Byte", 123},
                    {"Int16", 123},
                    {"Int32", 123},
                    {"Float", 123},
                    {"Decimal", 123}
                };
            var result = ContentHelper.Deserialize<NumberTypeMismatch_Object>(value);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Byte, Is.EqualTo(123));
            Assert.That(result.Int16, Is.EqualTo(123));
            Assert.That(result.Int32, Is.EqualTo(123));
            Assert.That(result.Float, Is.EqualTo(123));
            Assert.That(result.Decimal, Is.EqualTo(123));
        }

        [Test]
        public void Deserialize_NumberTypeMismatch_FromFloat_Test()
        {
            var value = new Dictionary<string, object>
                {
                    {"Byte", 123.456},
                    {"Int16", 123.456},
                    {"Int32", 123.456},
                    {"Float", 123.456},
                    {"Decimal", 123.456}
                };
            var result = ContentHelper.Deserialize<NumberTypeMismatch_Object>(value);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Byte, Is.EqualTo(123));
            Assert.That(result.Int16, Is.EqualTo(123));
            Assert.That(result.Int32, Is.EqualTo(123));
            Assert.That(result.Float, Is.EqualTo(123.456).Within(1e-4));
            Assert.That(result.Decimal, Is.EqualTo(123.456).Within(1e-4));
        }

        private class NumberTypeMismatch_Object
        {
            public byte Byte { get; set; }
            public short Int16 { get; set; }
            public int Int32 { get; set; }
            public float Float { get; set; }
            public decimal Decimal { get; set; }
        }

        [Test]
        public void GetProtocolValue_Untyped_Test()
        {
            var resource = new SDataResource {ETag = "abc123"};
            var eTag = ContentHelper.GetProtocolValue<string>(resource, SDataProtocolProperty.ETag);
            Assert.That(eTag, Is.EqualTo("abc123"));
        }

        [Test]
        public void GetProtocolValue_Typed_Test()
        {
            var resource = new ProtocolValue_Typed_Object {ETag = "abc123"};
            var eTag = ContentHelper.GetProtocolValue<string>(resource, SDataProtocolProperty.ETag);
            Assert.That(eTag, Is.EqualTo("abc123"));
        }

        [Test]
        public void SetProtocolValue_Untyped_Test()
        {
            var resource = new SDataResource();
            ContentHelper.SetProtocolValue(resource, SDataProtocolProperty.ETag, "abc123");
            Assert.That(resource.ETag, Is.EqualTo("abc123"));
        }

        [Test]
        public void SetProtocolValue_Typed_Test()
        {
            var resource = new ProtocolValue_Typed_Object();
            ContentHelper.SetProtocolValue(resource, SDataProtocolProperty.ETag, "abc123");
            Assert.That(resource.ETag, Is.EqualTo("abc123"));
        }

        private class ProtocolValue_Typed_Object
        {
            [SDataProtocolProperty(SDataProtocolProperty.ETag)]
            public string ETag { get; set; }
        }

        [Test]
        public void Deserialize_Anonymous_Type_Test()
        {
            var value = new SDataResource
                {
                    {"AccountName", "Bloggs Inc."},
                    {"Incorporated", new DateTime(2001, 2, 3, 4, 5, 6)},
                    {"Address", new SDataResource {{"City", "Melbourne"}}},
                    {
                        "Contacts", new SDataCollection<SDataResource>
                            {
                                new SDataResource
                                    {
                                        {"FullName", "Joe Bloggs"},
                                        {"Age", 44}
                                    }
                            }
                    }
                };
            var prototype = new
                {
                    AccountName = default(string),
                    Incorporated = default(DateTime),
                    Address = new {City = default(string)},
                    Contacts = new[]
                        {
                            new
                                {
                                    FullName = default(string),
                                    Age = default(int)
                                }
                        }
                };
            var result = DeserializeAnonymous(value, prototype);
            Assert.That(result.AccountName, Is.EqualTo("Bloggs Inc."));
            Assert.That(result.Incorporated, Is.EqualTo(new DateTime(2001, 2, 3, 4, 5, 6)));
            Assert.That(result.Address, Is.Not.Null);
            Assert.That(result.Address.City, Is.EqualTo("Melbourne"));
            Assert.That(result.Contacts, Is.Not.Empty);
            Assert.That(result.Contacts.Length, Is.EqualTo(1));
            Assert.That(result.Contacts[0].FullName, Is.EqualTo("Joe Bloggs"));
            Assert.That(result.Contacts[0].Age, Is.EqualTo(44));
        }

        private static T DeserializeAnonymous<T>(object value, T prototype)
        {
            return ContentHelper.Deserialize<T>(value);
        }

        [Test]
        public void Serialize_Resource_Nested_In_Anonymous_Type_Test()
        {
            var value = new {nested = new SDataResource {Key = "abc123"}};
            var result = ContentHelper.Serialize(value) as IDictionary<string, object>;
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ContainsKey("nested"), Is.True);
            var nested = result["nested"] as SDataResource;
            Assert.That(nested, Is.Not.Null);
            Assert.That(nested.Key, Is.EqualTo("abc123"));
        }

        [Test]
        public void Serialize_Anonymous_Type_Nested_In_Resource_Test()
        {
            var value = new SDataResource("root") {{"nested", new {id = "abc123"}}};
            var result = ContentHelper.Serialize(value) as IDictionary<string, object>;
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ContainsKey("nested"), Is.True);
            var nested = result["nested"] as IDictionary<string, object>;
            Assert.That(nested, Is.Not.Null);
            Assert.That(nested.ContainsKey("id"), Is.True);
            Assert.That(nested["id"], Is.EqualTo("abc123"));
        }

        [Test]
        public void Deserialize_Empty_Collection_Inference_Workaround_Test()
        {
            var value = new SDataResource {{"Items", new SDataResource()}};
            var result = ContentHelper.Deserialize<Deserialize_Empty_Collection_Inference_Workaround_Object>(value);
            Assert.That(result.Items, Is.Not.Null);
        }

        private class Deserialize_Empty_Collection_Inference_Workaround_Object
        {
            public IList<object> Items { get; set; }
        }

        [Test]
        public void Deserialize_Simple_Types_From_String_Test()
        {
            var value = new SDataResource
                {
                    {"SByteProp", "1"},
                    {"ShortProp", "1"},
                    {"IntProp", "1"},
                    {"LongProp", "1"},
                    {"ByteProp", "1"},
                    {"UShortProp", "1"},
                    {"UIntProp", "1"},
                    {"ULongProp", "1"},
                    {"FloatProp", "1.1"},
                    {"DoubleProp", "1.1"},
                    {"BoolProp", "true"},
                    {"CharProp", "1"},
                    {"DecimalProp", "1.1"},
                    {"DateTimeProp", "2001-01-01T01:01:01Z"},
                    {"EnumProp", "1"},
                    {"StringProp", "1"},
                    {"DateTimeOffsetProp", "2001-01-01T01:01:01Z"},
                    {"GuidProp", "11111111-1111-1111-1111-111111111111"},
                    {"TimeSpanProp", "01:01:01"},
                    {"VersionProp", "1.1"},
                    {"UriProp", "http://1.com"},
                    {"ByteArrayProp", "1"}
                };
            var result = ContentHelper.Deserialize<Simple_Types_Object>(value);
            Assert.That(result.SByteProp, Is.EqualTo(1));
            Assert.That(result.ShortProp, Is.EqualTo(1));
            Assert.That(result.IntProp, Is.EqualTo(1));
            Assert.That(result.LongProp, Is.EqualTo(1));
            Assert.That(result.ByteProp, Is.EqualTo(1));
            Assert.That(result.UShortProp, Is.EqualTo(1));
            Assert.That(result.UIntProp, Is.EqualTo(1));
            Assert.That(result.ULongProp, Is.EqualTo(1));
            Assert.That(result.FloatProp, Is.EqualTo(1.1F));
            Assert.That(result.DoubleProp, Is.EqualTo(1.1));
            Assert.That(result.BoolProp, Is.True);
            Assert.That(result.CharProp, Is.EqualTo('1'));
            Assert.That(result.DecimalProp, Is.EqualTo(1.1));
            Assert.That(result.DateTimeProp, Is.EqualTo(new DateTime(2001, 1, 1, 1, 1, 1)));
            Assert.That(result.EnumProp, Is.EqualTo(DayOfWeek.Monday));
            Assert.That(result.StringProp, Is.EqualTo("1"));
            Assert.That(result.DateTimeOffsetProp, Is.EqualTo(new DateTimeOffset(2001, 1, 1, 1, 1, 1, TimeSpan.Zero)));
            Assert.That(result.GuidProp, Is.EqualTo(new Guid("11111111-1111-1111-1111-111111111111")));
            Assert.That(result.TimeSpanProp, Is.EqualTo(new TimeSpan(1, 1, 1)));
            Assert.That(result.VersionProp, Is.EqualTo(new Version(1, 1)));
            Assert.That(result.UriProp, Is.EqualTo(new Uri("http://1.com")));
        }

        [Test]
        public void Deserialize_Simple_Types_From_Null_Test()
        {
            var value = new SDataResource
                {
                    {"SByteProp", null},
                    {"ShortProp", null},
                    {"IntProp", null},
                    {"LongProp", null},
                    {"ByteProp", null},
                    {"UShortProp", null},
                    {"UIntProp", null},
                    {"ULongProp", null},
                    {"FloatProp", null},
                    {"DoubleProp", null},
                    {"BoolProp", null},
                    {"CharProp", null},
                    {"DecimalProp", null},
                    {"DateTimeProp", null},
                    {"EnumProp", null},
                    {"StringProp", null},
                    {"DateTimeOffsetProp", null},
                    {"GuidProp", null},
                    {"TimeSpanProp", null},
                    {"VersionProp", null},
                    {"UriProp", null},
                    {"ByteArrayProp", null}
                };
            var result = ContentHelper.Deserialize<Simple_Types_Object>(value);
            Assert.That(result.SByteProp, Is.EqualTo(0));
            Assert.That(result.ShortProp, Is.EqualTo(0));
            Assert.That(result.IntProp, Is.EqualTo(0));
            Assert.That(result.LongProp, Is.EqualTo(0));
            Assert.That(result.ByteProp, Is.EqualTo(0));
            Assert.That(result.UShortProp, Is.EqualTo(0));
            Assert.That(result.UIntProp, Is.EqualTo(0));
            Assert.That(result.ULongProp, Is.EqualTo(0));
            Assert.That(result.FloatProp, Is.EqualTo(0));
            Assert.That(result.DoubleProp, Is.EqualTo(0));
            Assert.That(result.BoolProp, Is.False);
            Assert.That(result.CharProp, Is.EqualTo('\0'));
            Assert.That(result.DecimalProp, Is.EqualTo(0));
            Assert.That(result.DateTimeProp, Is.EqualTo(DateTime.MinValue));
            Assert.That(result.EnumProp, Is.EqualTo(DayOfWeek.Sunday));
            Assert.That(result.StringProp, Is.EqualTo(null));
            Assert.That(result.DateTimeOffsetProp, Is.EqualTo(DateTimeOffset.MinValue));
            Assert.That(result.GuidProp, Is.EqualTo(Guid.Empty));
            Assert.That(result.TimeSpanProp, Is.EqualTo(TimeSpan.Zero));
            Assert.That(result.VersionProp, Is.EqualTo(null));
            Assert.That(result.UriProp, Is.EqualTo(null));
        }

        [Test]
        public void Serialize_Simple_Types_Test()
        {
            var value = new Simple_Types_Object
                {
                    SByteProp = 1,
                    ShortProp = 1,
                    IntProp = 1,
                    LongProp = 1,
                    ByteProp = 1,
                    UShortProp = 1,
                    UIntProp = 1,
                    ULongProp = 1,
                    FloatProp = 1.1F,
                    DoubleProp = 1.1,
                    BoolProp = true,
                    CharProp = '1',
                    DecimalProp = 1.1M,
                    DateTimeProp = new DateTime(2001, 1, 1, 1, 1, 1),
                    EnumProp = DayOfWeek.Monday,
                    StringProp = "1",
                    DateTimeOffsetProp = new DateTimeOffset(2001, 1, 1, 1, 1, 1, TimeSpan.Zero),
                    GuidProp = new Guid("11111111-1111-1111-1111-111111111111"),
                    TimeSpanProp = new TimeSpan(1, 1, 1),
                    VersionProp = new Version(1, 1),
                    UriProp = new Uri("http://1.com")
                };
            var result = (IDictionary<string, object>) ContentHelper.Serialize(value);
            Assert.That(result["SByteProp"], Is.EqualTo(1));
            Assert.That(result["ShortProp"], Is.EqualTo(1));
            Assert.That(result["IntProp"], Is.EqualTo(1));
            Assert.That(result["LongProp"], Is.EqualTo(1));
            Assert.That(result["ByteProp"], Is.EqualTo(1));
            Assert.That(result["UShortProp"], Is.EqualTo(1));
            Assert.That(result["UIntProp"], Is.EqualTo(1));
            Assert.That(result["ULongProp"], Is.EqualTo(1));
            Assert.That(result["FloatProp"], Is.EqualTo(1.1F));
            Assert.That(result["DoubleProp"], Is.EqualTo(1.1));
            Assert.That(result["BoolProp"], Is.True);
            Assert.That(result["CharProp"], Is.EqualTo('1'));
            Assert.That(result["DecimalProp"], Is.EqualTo(1.1));
            Assert.That(result["DateTimeProp"], Is.EqualTo(new DateTime(2001, 1, 1, 1, 1, 1)));
            Assert.That(result["EnumProp"], Is.EqualTo(DayOfWeek.Monday));
            Assert.That(result["StringProp"], Is.EqualTo("1"));
            Assert.That(result["DateTimeOffsetProp"], Is.EqualTo(new DateTimeOffset(2001, 1, 1, 1, 1, 1, TimeSpan.Zero)));
            Assert.That(result["GuidProp"], Is.EqualTo(new Guid("11111111-1111-1111-1111-111111111111")));
            Assert.That(result["TimeSpanProp"], Is.EqualTo(new TimeSpan(1, 1, 1)));
            Assert.That(result["VersionProp"], Is.EqualTo(new Version(1, 1)));
            Assert.That(result["UriProp"], Is.EqualTo(new Uri("http://1.com")));
        }

        private class Simple_Types_Object
        {
            public sbyte SByteProp { get; set; }
            public short ShortProp { get; set; }
            public int IntProp { get; set; }
            public long LongProp { get; set; }
            public byte ByteProp { get; set; }
            public ushort UShortProp { get; set; }
            public uint UIntProp { get; set; }
            public ulong ULongProp { get; set; }
            public float FloatProp { get; set; }
            public double DoubleProp { get; set; }
            public decimal DecimalProp { get; set; }
            public DateTime DateTimeProp { get; set; }
            public DayOfWeek EnumProp { get; set; }
            public bool BoolProp { get; set; }
            public char CharProp { get; set; }
            public string StringProp { get; set; }
            public DateTimeOffset DateTimeOffsetProp { get; set; }
            public Guid GuidProp { get; set; }
            public TimeSpan TimeSpanProp { get; set; }
            public Version VersionProp { get; set; }
            public Uri UriProp { get; set; }
        }

        [Test]
        public void Deserialize_Object_With_ReadOnly_Property_Test()
        {
            var value = new SDataResource();
            var result = ContentHelper.Deserialize<Deserialize_Object_With_ReadOnly_Property_Object>(value);
            Assert.That(result, Is.Not.Null);
        }

        private class Deserialize_Object_With_ReadOnly_Property_Object
        {
            public string Name
            {
                get { throw new NotSupportedException(); }
            }
        }

        [Test]
        public void Serialize_Object_With_WriteOnly_Property_Test()
        {
            var value = new Serialize_Object_With_WriteOnly_Property_Object();
            var result = ContentHelper.Serialize(value);
            Assert.That(result, Is.Not.Null);
        }

        private class Serialize_Object_With_WriteOnly_Property_Object
        {
            public string Name
            {
                set { throw new NotSupportedException(); }
            }
        }

        [Test]
        public void Serialize_Object_With_Indexer_Test()
        {
            var value = new Serialize_Object_With_Indexer_Object();
            var result = ContentHelper.Serialize(value);
            Assert.That(result, Is.Not.Null);
        }

        private class Serialize_Object_With_Indexer_Object
        {
            public object this[string name]
            {
                get { throw new NotSupportedException(); }
                set { throw new NotSupportedException(); }
            }
        }

        [Test]
        public void Serialize_Empty_Collection_String_Test()
        {
            var value = new {items = new List<string>()};
            var result = (IDictionary<string, object>) ContentHelper.Serialize(value);
            Assert.That(result, Is.Not.Null);
            Assert.That(result["items"], Is.InstanceOf<SDataCollection<string>>());
        }

        [Test]
        public void Serialize_Empty_Collection_Object_Test()
        {
            var value = new {items = new List<object>()};
            var result = (IDictionary<string, object>) ContentHelper.Serialize(value);
            Assert.That(result, Is.Not.Null);
            Assert.That(result["items"], Is.InstanceOf<SDataCollection<object>>());
        }

        [Test]
        public void Serialize_Empty_Collection_Resource_Test()
        {
            var value = new {items = new List<Serialize_Empty_Collection_Resource_Object>()};
            var result = (IDictionary<string, object>) ContentHelper.Serialize(value);
            Assert.That(result, Is.Not.Null);
            Assert.That(result["items"], Is.InstanceOf<SDataCollection<SDataResource>>());
        }

        private class Serialize_Empty_Collection_Resource_Object
        {
        }
    }
}