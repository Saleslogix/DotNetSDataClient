using System;
using System.Collections.Generic;
using NUnit.Framework;
using Saleslogix.SData.Client.Framework;
using Saleslogix.SData.Client.Test.Model;

// ReSharper disable InconsistentNaming

namespace Saleslogix.SData.Client.Test.Content
{
    [TestFixture]
    public class JsonContentHandlerTests
    {
        [Test]
        public void Read_DateTime_Test()
        {
            const string json = @"{""$syncState"":{""stamp"":""/Date(1371283200000)/""}}";
            var resource = Helpers.ReadJson<SDataResource>(json);
            Assert.That(resource.SyncState, Is.Not.Null);
            Assert.That(resource.SyncState.Stamp, Is.EqualTo(new DateTime(2013, 6, 15, 8, 0, 0)));
        }

        [Test]
        public void Read_DateTimeOffset_Test()
        {
            const string json = @"{""$updated"":""/Date(1371247200000+1000)/""}";
            var resource = Helpers.ReadJson<SDataResource>(json);
            Assert.That(resource.Updated, Is.EqualTo(new DateTimeOffset(2013, 6, 15, 8, 0, 0, TimeSpan.FromHours(10))));
        }

        [Test]
        public void Read_Guid_Test()
        {
            const string json = @"{""$uuid"":""281018F3-8A96-4CAC-B6C6-957E29F65359""}";
            var resource = Helpers.ReadJson<SDataResource>(json);
            Assert.That(resource.Uuid, Is.EqualTo(new Guid("281018F3-8A96-4CAC-B6C6-957E29F65359")));
        }

        [Test]
        public void Write_DateTime_Test()
        {
            var resources = new SDataResource {SyncState = new SyncState {Stamp = new DateTime(2013, 6, 15, 8, 0, 0)}};
            var obj = Helpers.WriteJson(resources);
            Assert.That(obj["$syncState"], Is.InstanceOf<IDictionary<string, object>>());
            var syncState = (IDictionary<string, object>) obj["$syncState"];
            Assert.That(syncState["stamp"], Is.EqualTo("/Date(1371283200000)/"));
        }

        [Test]
        public void Write_DateTimeOffset_Test()
        {
            var resources = new SDataResource {Updated = new DateTimeOffset(2013, 6, 15, 8, 0, 0, TimeSpan.FromHours(10))};
            var obj = Helpers.WriteJson(resources);
            var updated = obj["$updated"];
            Assert.That(updated, Is.EqualTo("/Date(1371247200000+1000)/"));
        }

        [Test]
        public void Write_Collection_Id_Test()
        {
            var resources = new SDataCollection<SDataResource> {Id = "id"};
            var obj = Helpers.WriteJson(resources);
            var id = obj["$id"];
            Assert.That(id, Is.EqualTo("id"));
        }

        [Test]
        public void Write_Collection_Diagnoses_Test()
        {
            var resources = new SDataCollection<SDataResource>
                                {
                                    Diagnoses =
                                        {
                                            new Diagnosis {SDataCode = DiagnosisCode.BadUrlSyntax}
                                        }
                                };
            var obj = Helpers.WriteJson(resources);
            Assert.That(obj["$diagnoses"], Is.InstanceOf<IList<object>>());
            var diagnoses = (IList<object>) obj["$diagnoses"];
            Assert.That(diagnoses, Has.Count.EqualTo(1));
            Assert.That(diagnoses[0], Is.InstanceOf<IDictionary<string, object>>());
            var diagnosis = (IDictionary<string, object>) diagnoses[0];
            Assert.That(diagnosis["sdataCode"], Is.EqualTo("BadUrlSyntax"));
        }

        [Test]
        public void Write_Collection_Schema_Test()
        {
            var resources = new SDataCollection<SDataResource>
                                {
                                    Schema = @"<schema xmlns=""http://www.w3.org/2001/XMLSchema"">"
                                };
            var obj = Helpers.WriteJson(resources);
            var schema = obj["$schema"];
            Assert.That(schema, Is.EqualTo(@"<schema xmlns=""http://www.w3.org/2001/XMLSchema"">"));
        }

        [Test]
        public void Write_Collection_SyncDigest_Test()
        {
            var resources = new SDataCollection<SDataResource>
                                {
                                    SyncDigest = new Digest {Origin = "origin"}
                                };
            var obj = Helpers.WriteJson(resources);
            Assert.That(obj["$digest"], Is.InstanceOf<IDictionary<string, object>>());
            var digest = (IDictionary<string, object>) obj["$digest"];
            Assert.That(digest["origin"], Is.EqualTo("origin"));
        }

        [Test]
        public void Write_Poco_Array_Test()
        {
            var resources = new[]
                                {
                                    new Contact
                                        {
                                            LastName = "Smith",
                                            Address = new Address {City = "Melbourne"}
                                        }
                                };
            var obj = Helpers.WriteJson(resources);
            Assert.That(obj["$resources"], Is.InstanceOf<IList<object>>());
            var contacts = (IList<object>) obj["$resources"];
            Assert.That(contacts, Has.Count.EqualTo(1));
            Assert.That(contacts[0], Is.InstanceOf<IDictionary<string, object>>());
            var contact = (IDictionary<string, object>) contacts[0];
            Assert.That(contact, Is.Not.Null);
            Assert.That(contact["LastName"], Is.EqualTo("Smith"));
            Assert.That(contact["Address"], Is.InstanceOf<IDictionary<string, object>>());
            var address = (IDictionary<string, object>) contact["Address"];
            Assert.That(address["City"], Is.EqualTo("Melbourne"));
        }

        [Test]
        public void Write_Resource_Id_Test()
        {
            var resources = new SDataResource {Id = "id"};
            var obj = Helpers.WriteJson(resources);
            Assert.That(obj["$id"], Is.EqualTo("id"));
        }

        [Test]
        public void Write_Resource_Diagnoses_Test()
        {
            var resources = new SDataResource
                                {
                                    Diagnoses =
                                        {
                                            new Diagnosis {SDataCode = DiagnosisCode.BadUrlSyntax}
                                        }
                                };
            var obj = Helpers.WriteJson(resources);
            Assert.That(obj["$diagnoses"], Is.InstanceOf<IList<object>>());
            var diagnoses = (IList<object>) obj["$diagnoses"];
            Assert.That(diagnoses, Has.Count.EqualTo(1));
            Assert.That(diagnoses[0], Is.InstanceOf<IDictionary<string, object>>());
            var diagnosis = (IDictionary<string, object>) diagnoses[0];
            Assert.That(diagnosis["sdataCode"], Is.EqualTo("BadUrlSyntax"));
        }

        [Test]
        public void Write_Resource_Schema_Test()
        {
            var resources = new SDataResource
                                {
                                    Schema = @"<schema xmlns=""http://www.w3.org/2001/XMLSchema"">"
                                };
            var obj = Helpers.WriteJson(resources);
            Assert.That(obj["$schema"], Is.EqualTo(@"<schema xmlns=""http://www.w3.org/2001/XMLSchema"">"));
        }

        [Test]
        public void Write_Resource_SyncState_Test()
        {
            var resources = new SDataResource
                                {
                                    SyncState = new SyncState
                                                    {
                                                        Tick = 123
                                                    }
                                };
            var obj = Helpers.WriteJson(resources);
            Assert.That(obj["$syncState"], Is.InstanceOf<IDictionary<string, object>>());
            var syncState = (IDictionary<string, object>) obj["$syncState"];
            Assert.That(syncState["tick"], Is.EqualTo(123));
        }

        [Test]
        public void Write_Poco_Test()
        {
            var resource = new Contact
                               {
                                   LastName = "Smith",
                                   Address = new Address {City = "Melbourne"}
                               };
            var obj = Helpers.WriteJson(resource);
            Assert.That(obj["LastName"], Is.EqualTo("Smith"));
            Assert.That(obj["Address"], Is.InstanceOf<IDictionary<string, object>>());
            var address = (IDictionary<string, object>) obj["Address"];
            Assert.That(address["City"], Is.EqualTo("Melbourne"));
        }

        [Test]
        public void Write_Nested_Dictionary_Test()
        {
            var resource = new SDataResource
                               {
                                   {"LastName", "Smith"},
                                   {"Address", new Dictionary<string, object> {{"City", "Melbourne"}}}
                               };
            var obj = Helpers.WriteJson(resource);
            Assert.That(obj["LastName"], Is.EqualTo("Smith"));
            Assert.That(obj["Address"], Is.InstanceOf<IDictionary<string, object>>());
            var address = (IDictionary<string, object>) obj["Address"];
            Assert.That(address["City"], Is.EqualTo("Melbourne"));
        }

        [Test]
        public void Write_Nested_Poco_Array_Test()
        {
            var resource = new SDataResource("SalesOrder")
                               {
                                   {
                                       "OrderLines", new[]
                                                         {
                                                             new SalesOrderLine {UnitPrice = 123},
                                                             new SalesOrderLine {UnitPrice = 456}
                                                         }
                                   }
                               };
            var obj = Helpers.WriteJson(resource);
            Assert.That(obj["OrderLines"], Is.InstanceOf<ICollection<object>>());
            Assert.That(obj["OrderLines"], Has.Count.EqualTo(2));
        }

        [Test]
        public void Write_Empty_String_Property_Test()
        {
            var resource = new SDataResource {{"LastName", ""}};
            var obj = Helpers.WriteJson(resource);
            Assert.That(obj["LastName"], Is.Empty);
        }
    }
}