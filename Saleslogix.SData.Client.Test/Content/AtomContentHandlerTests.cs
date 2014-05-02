using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using NUnit.Framework;
using Saleslogix.SData.Client.Framework;
using Saleslogix.SData.Client.Test.Model;

// ReSharper disable InconsistentNaming

namespace Saleslogix.SData.Client.Test.Content
{
    [TestFixture]
    public class AtomContentHandlerTests
    {
        [Test]
        public void Read_DateTime_Test()
        {
            const string xml = @"
                    <atom:entry xmlns:atom=""http://www.w3.org/2005/Atom"" xmlns:sync=""http://schemas.sage.com/sdata/sync/2008/1"">
                      <sync:syncState>
                        <sync:stamp>2013-06-15T08:00:00</sync:stamp>
                      </sync:syncState>
                    </atom:entry>";
            var resource = Helpers.ReadAtom<SDataResource>(xml);
            Assert.That(resource.SyncState, Is.Not.Null);
            Assert.That(resource.SyncState.Stamp, Is.EqualTo(new DateTime(2013, 6, 15, 8, 0, 0)));
        }

        [Test]
        public void Read_DateTimeOffset_Test()
        {
            const string xml = @"
                    <atom:entry xmlns:atom=""http://www.w3.org/2005/Atom"">
                      <atom:updated>2013-06-15T08:00:00+10:00</atom:updated>
                    </atom:entry>";
            var resource = Helpers.ReadAtom<SDataResource>(xml);
            Assert.That(resource.Updated, Is.EqualTo(new DateTimeOffset(2013, 6, 15, 8, 0, 0, TimeSpan.FromHours(10))));
        }

        [Test]
        public void Read_Guid_Test()
        {
            const string xml = @"
                    <atom:entry xmlns:atom=""http://www.w3.org/2005/Atom"">
                      <sdata:payload xmlns:sdata=""http://schemas.sage.com/sdata/2008/1"">
                        <salesOrder sdata:uuid=""281018F3-8A96-4CAC-B6C6-957E29F65359"" />
                      </sdata:payload>
                    </atom:entry>";
            var resource = Helpers.ReadAtom<SDataResource>(xml);
            Assert.That(resource.Uuid, Is.EqualTo(new Guid("281018F3-8A96-4CAC-B6C6-957E29F65359")));
        }

        [Test]
        public void Read_Links_Test()
        {
            const string xml = @"
                    <atom:entry xmlns:atom=""http://www.w3.org/2005/Atom"">
                      <atom:link href=""http://dummy/$schema"" rel=""schemas.sage.com/sdata/link-relations/schema"" type=""application/xml"" title=""Schema""/>
                      <atom:link href=""http://dummy/$template"" rel=""schemas.sage.com/sdata/link-relations/template"" type=""application/atom+xml; type=entry"" title=""Template""/>
                      <atom:link href=""http://dummy/$service"" rel=""schemas.sage.com/sdata/link-relations/service"" type=""application/atom+xml; type=feed"" title=""Service""/>
                    </atom:entry>";
            var resource = Helpers.ReadAtom<SDataResource>(xml);
            Assert.That(resource.Links, Is.Not.Null);
            Assert.That(resource.Links.Count, Is.EqualTo(3));
            var link = resource.Links[0];
            Assert.That(link.Uri, Is.EqualTo(new Uri("http://dummy/$schema")));
            Assert.That(link.Relation, Is.EqualTo("schemas.sage.com/sdata/link-relations/schema"));
            Assert.That(link.Type, Is.EqualTo(MediaType.Xml));
            Assert.That(link.Title, Is.EqualTo("Schema"));
            link = resource.Links[1];
            Assert.That(link.Uri, Is.EqualTo(new Uri("http://dummy/$template")));
            Assert.That(link.Relation, Is.EqualTo("schemas.sage.com/sdata/link-relations/template"));
            Assert.That(link.Type, Is.EqualTo(MediaType.AtomEntry));
            Assert.That(link.Title, Is.EqualTo("Template"));
            link = resource.Links[2];
            Assert.That(link.Uri, Is.EqualTo(new Uri("http://dummy/$service")));
            Assert.That(link.Relation, Is.EqualTo("schemas.sage.com/sdata/link-relations/service"));
            Assert.That(link.Type, Is.EqualTo(MediaType.Atom));
            Assert.That(link.Title, Is.EqualTo("Service"));
        }

        [Test]
        public void Write_DateTime_Test()
        {
            var resource = new SDataResource {SyncState = new SyncState {Stamp = new DateTime(2013, 6, 15, 8, 0, 0)}};
            var nav = Helpers.WriteAtom(resource);
            var mgr = new XmlNamespaceManager(nav.NameTable);
            mgr.AddNamespace("atom", "http://www.w3.org/2005/Atom");
            mgr.AddNamespace("sync", "http://schemas.sage.com/sdata/sync/2008/1");
            var node = nav.SelectSingleNode("atom:entry/sync:syncState/sync:stamp", mgr);
            Assert.That(node, Is.Not.Null);
            Assert.That(node.Value, Is.EqualTo("2013-06-15T08:00:00"));
        }

        [Test]
        public void Write_DateTimeOffset_Test()
        {
            var resource = new SDataResource {Updated = new DateTimeOffset(2013, 6, 15, 8, 0, 0, TimeSpan.FromHours(10))};
            var nav = Helpers.WriteAtom(resource);
            var mgr = new XmlNamespaceManager(nav.NameTable);
            mgr.AddNamespace("atom", "http://www.w3.org/2005/Atom");
            var node = nav.SelectSingleNode("atom:entry/atom:updated", mgr);
            Assert.That(node, Is.Not.Null);
            Assert.That(node.Value, Is.EqualTo("2013-06-15T08:00:00+10:00"));
        }

        [Test]
        public void Write_Feed_Id_Test()
        {
            var resources = new SDataCollection<SDataResource> {Id = "id"};
            var nav = Helpers.WriteAtom(resources);
            var mgr = new XmlNamespaceManager(nav.NameTable);
            mgr.AddNamespace("atom", "http://www.w3.org/2005/Atom");
            var node = nav.SelectSingleNode("atom:feed/atom:id", mgr);
            Assert.That(node, Is.Not.Null);
            Assert.That(node.Value, Is.EqualTo("id"));
        }

        [Test]
        public void Write_Feed_Diagnoses_Test()
        {
            var resources = new SDataCollection<SDataResource>
                                {
                                    Diagnoses =
                                        {
                                            new Diagnosis {SDataCode = DiagnosisCode.BadUrlSyntax}
                                        }
                                };
            var nav = Helpers.WriteAtom(resources);
            var mgr = new XmlNamespaceManager(nav.NameTable);
            mgr.AddNamespace("atom", "http://www.w3.org/2005/Atom");
            mgr.AddNamespace("sdata", "http://schemas.sage.com/sdata/2008/1");
            var node = nav.SelectSingleNode("atom:feed/sdata:diagnosis/sdata:sdataCode", mgr);
            Assert.That(node, Is.Not.Null);
            Assert.That(node.Value, Is.EqualTo("BadUrlSyntax"));
        }

        [Test]
        public void Write_Feed_Schema_Test()
        {
            var resources = new SDataCollection<SDataResource>
                                {
                                    Schema = @"<schema xmlns=""http://www.w3.org/2001/XMLSchema"">"
                                };
            var nav = Helpers.WriteAtom(resources);
            var mgr = new XmlNamespaceManager(nav.NameTable);
            mgr.AddNamespace("atom", "http://www.w3.org/2005/Atom");
            mgr.AddNamespace("sdata", "http://schemas.sage.com/sdata/2008/1");
            var node = nav.SelectSingleNode("atom:feed/sdata:schema", mgr);
            Assert.That(node, Is.Not.Null);
            Assert.That(node.Value, Is.EqualTo(@"<schema xmlns=""http://www.w3.org/2001/XMLSchema"">"));
        }

        [Test]
        public void Write_Feed_SyncDigest_Test()
        {
            var resources = new SDataCollection<SDataResource>
                                {
                                    SyncDigest = new Digest {Origin = "origin"}
                                };
            var nav = Helpers.WriteAtom(resources);
            var mgr = new XmlNamespaceManager(nav.NameTable);
            mgr.AddNamespace("atom", "http://www.w3.org/2005/Atom");
            mgr.AddNamespace("sync", "http://schemas.sage.com/sdata/sync/2008/1");
            var node = nav.SelectSingleNode("atom:feed/sync:digest/sync:origin", mgr);
            Assert.That(node, Is.Not.Null);
            Assert.That(node.Value, Is.EqualTo("origin"));
        }

        [Test]
        public void Write_Poco_Collection_Test()
        {
            var resources = new[]
                                {
                                    new Contact
                                        {
                                            LastName = "Smith",
                                            Address = new Address {City = "Melbourne"}
                                        }
                                };
            var nav = Helpers.WriteAtom(resources);
            var mgr = new XmlNamespaceManager(nav.NameTable);
            mgr.AddNamespace("atom", "http://www.w3.org/2005/Atom");
            mgr.AddNamespace("sdata", "http://schemas.sage.com/sdata/2008/1");
            var node = nav.SelectSingleNode("atom:feed/atom:entry/sdata:payload/Contact/LastName", mgr);
            Assert.That(node, Is.Not.Null);
            Assert.That(node.Value, Is.EqualTo("Smith"));
            node = nav.SelectSingleNode("atom:feed/atom:entry/sdata:payload/Contact/Address/City", mgr);
            Assert.That(node, Is.Not.Null);
            Assert.That(node.Value, Is.EqualTo("Melbourne"));
        }

        [Test]
        public void Write_Entry_Id_Test()
        {
            var resource = new SDataResource {Id = "id"};
            var nav = Helpers.WriteAtom(resource);
            var mgr = new XmlNamespaceManager(nav.NameTable);
            mgr.AddNamespace("atom", "http://www.w3.org/2005/Atom");
            var node = nav.SelectSingleNode("atom:entry/atom:id", mgr);
            Assert.That(node, Is.Not.Null);
            Assert.That(node.Value, Is.EqualTo("id"));
        }

        [Test]
        public void Write_Entry_Diagnoses_Test()
        {
            var resource = new SDataResource
                               {
                                   Diagnoses =
                                       {
                                           new Diagnosis {SDataCode = DiagnosisCode.BadUrlSyntax}
                                       }
                               };
            var nav = Helpers.WriteAtom(resource);
            var mgr = new XmlNamespaceManager(nav.NameTable);
            mgr.AddNamespace("atom", "http://www.w3.org/2005/Atom");
            mgr.AddNamespace("sdata", "http://schemas.sage.com/sdata/2008/1");
            var node = nav.SelectSingleNode("atom:entry/sdata:diagnosis/sdata:sdataCode", mgr);
            Assert.That(node, Is.Not.Null);
            Assert.That(node.Value, Is.EqualTo("BadUrlSyntax"));
        }

        [Test]
        public void Write_Entry_Schema_Test()
        {
            var resource = new SDataResource
                               {
                                   Schema = @"<schema xmlns=""http://www.w3.org/2001/XMLSchema"">"
                               };
            var nav = Helpers.WriteAtom(resource);
            var mgr = new XmlNamespaceManager(nav.NameTable);
            mgr.AddNamespace("atom", "http://www.w3.org/2005/Atom");
            mgr.AddNamespace("sdata", "http://schemas.sage.com/sdata/2008/1");
            var node = nav.SelectSingleNode("atom:entry/sdata:schema", mgr);
            Assert.That(node, Is.Not.Null);
            Assert.That(node.Value, Is.EqualTo(@"<schema xmlns=""http://www.w3.org/2001/XMLSchema"">"));
        }

        [Test]
        public void Write_Entry_SyncState_Test()
        {
            var resource = new SDataResource
                               {
                                   SyncState = new SyncState {Tick = 123}
                               };
            var nav = Helpers.WriteAtom(resource);
            var mgr = new XmlNamespaceManager(nav.NameTable);
            mgr.AddNamespace("atom", "http://www.w3.org/2005/Atom");
            mgr.AddNamespace("sync", "http://schemas.sage.com/sdata/sync/2008/1");
            var node = nav.SelectSingleNode("atom:entry/sync:syncState/sync:tick", mgr);
            Assert.That(node, Is.Not.Null);
            Assert.That(node.Value, Is.EqualTo("123"));
        }

        [Test]
        public void Write_Poco_Test()
        {
            var resource = new Contact
                               {
                                   LastName = "Smith",
                                   Address = new Address {City = "Melbourne"}
                               };
            var nav = Helpers.WriteAtom(resource);
            var mgr = new XmlNamespaceManager(nav.NameTable);
            mgr.AddNamespace("atom", "http://www.w3.org/2005/Atom");
            mgr.AddNamespace("sdata", "http://schemas.sage.com/sdata/2008/1");
            var node = nav.SelectSingleNode("atom:entry/sdata:payload/Contact/LastName", mgr);
            Assert.That(node, Is.Not.Null);
            Assert.That(node.Value, Is.EqualTo("Smith"));
            node = nav.SelectSingleNode("atom:entry/sdata:payload/Contact/Address/City", mgr);
            Assert.That(node, Is.Not.Null);
            Assert.That(node.Value, Is.EqualTo("Melbourne"));
        }

        [Test]
        public void Write_Nested_Dictionary_Test()
        {
            var resource = new SDataResource("Contact")
                               {
                                   {"LastName", "Smith"},
                                   {"Address", new Dictionary<string, object> {{"City", "Melbourne"}}}
                               };
            var nav = Helpers.WriteAtom(resource);
            var mgr = new XmlNamespaceManager(nav.NameTable);
            mgr.AddNamespace("atom", "http://www.w3.org/2005/Atom");
            mgr.AddNamespace("sdata", "http://schemas.sage.com/sdata/2008/1");
            var node = nav.SelectSingleNode("atom:entry/sdata:payload/Contact/LastName", mgr);
            Assert.That(node, Is.Not.Null);
            Assert.That(node.Value, Is.EqualTo("Smith"));
            node = nav.SelectSingleNode("atom:entry/sdata:payload/Contact/Address/City", mgr);
            Assert.That(node, Is.Not.Null);
            Assert.That(node.Value, Is.EqualTo("Melbourne"));
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
            var nav = Helpers.WriteAtom(resource);
            var mgr = new XmlNamespaceManager(nav.NameTable);
            mgr.AddNamespace("atom", "http://www.w3.org/2005/Atom");
            mgr.AddNamespace("sdata", "http://schemas.sage.com/sdata/2008/1");
            var nodes = nav.Select("atom:entry/sdata:payload/SalesOrder/OrderLines/SalesOrderLine", mgr);
            Assert.That(nodes, Is.Not.Null);
            Assert.That(nodes.Count, Is.EqualTo(2));
        }

        [Test]
        public void Write_Flat_Collection_Test()
        {
            var lines = new SDataCollection<SDataResource>("SalesOrderLine")
                            {
                                new SDataResource(),
                                new SDataResource()
                            };
            lines.XmlIsFlat = true;
            var resource = new SDataResource("SalesOrder") {{"SalesOrderLines", lines}};
            var nav = Helpers.WriteAtom(resource);
            var mgr = new XmlNamespaceManager(nav.NameTable);
            mgr.AddNamespace("atom", "http://www.w3.org/2005/Atom");
            mgr.AddNamespace("sdata", "http://schemas.sage.com/sdata/2008/1");
            var nodes = nav.Select("atom:entry/sdata:payload/SalesOrder/SalesOrderLine", mgr);
            Assert.That(nodes, Is.Not.Null);
            Assert.That(nodes.Count, Is.EqualTo(2));
        }

        [Test]
        public void Write_Empty_String_Property_Test()
        {
            var resource = new SDataResource("Contact") {{"LastName", ""}};
            var nav = Helpers.WriteAtom(resource);
            var mgr = new XmlNamespaceManager(nav.NameTable);
            mgr.AddNamespace("atom", "http://www.w3.org/2005/Atom");
            mgr.AddNamespace("sdata", "http://schemas.sage.com/sdata/2008/1");
            var node = nav.SelectSingleNode("atom:entry/sdata:payload/Contact/LastName", mgr);
            Assert.That(node, Is.Not.Null);
            Assert.That(node.IsEmptyElement, Is.True);
        }

        [Test]
        public void Write_Poco_Namespace_Test()
        {
            var resource = new Write_Poco_Namespace_Object {Name = "Abbott"};
            var nav = Helpers.WriteAtom(resource);
            var mgr = new XmlNamespaceManager(nav.NameTable);
            mgr.AddNamespace("atom", "http://www.w3.org/2005/Atom");
            mgr.AddNamespace("sdata", "http://schemas.sage.com/sdata/2008/1");
            mgr.AddNamespace("a", "Account_NS");
            var node = nav.SelectSingleNode("atom:entry/sdata:payload/a:Account_/a:Name", mgr);
            Assert.That(node, Is.Not.Null);
            Assert.That(node.Value, Is.EqualTo("Abbott"));
        }

        [XmlType("Account_", Namespace = "Account_NS")]
        private class Write_Poco_Namespace_Object
        {
            public string Name { get; set; }
        }

        [Test]
        public void Write_Nested_Poco_Namespace_Test()
        {
            var resource = new Write_Nested_Poco_Namespace_Object
                               {
                                   Address = new Address {City = "Melbourne"}
                               };
            var nav = Helpers.WriteAtom(resource);
            var mgr = new XmlNamespaceManager(nav.NameTable);
            mgr.AddNamespace("atom", "http://www.w3.org/2005/Atom");
            mgr.AddNamespace("sdata", "http://schemas.sage.com/sdata/2008/1");
            mgr.AddNamespace("a", "Address_NS");
            var node = nav.SelectSingleNode("atom:entry/sdata:payload/Account_/a:Address_/a:City", mgr);
            Assert.That(node, Is.Not.Null);
            Assert.That(node.Value, Is.EqualTo("Melbourne"));
        }

        [XmlType("Account_")]
        private class Write_Nested_Poco_Namespace_Object
        {
            [XmlElement("Address_", Namespace = "Address_NS")]
            public Address Address { get; set; }
        }

        [Test]
        public void Write_Nested_Poco_Collection_Namespace_Test()
        {
            var resource = new Write_Nested_Poco_Collection_Namespace_Object
                               {
                                   Addresses = new[] {new Address {City = "Melbourne"}}
                               };
            var nav = Helpers.WriteAtom(resource);
            var mgr = new XmlNamespaceManager(nav.NameTable);
            mgr.AddNamespace("atom", "http://www.w3.org/2005/Atom");
            mgr.AddNamespace("sdata", "http://schemas.sage.com/sdata/2008/1");
            mgr.AddNamespace("a", "Address_NS");
            var node = nav.SelectSingleNode("atom:entry/sdata:payload/Account_/Addresses_/a:Address_/a:City", mgr);
            Assert.That(node, Is.Not.Null);
            Assert.That(node.Value, Is.EqualTo("Melbourne"));
        }

        [XmlType("Account_")]
        private class Write_Nested_Poco_Collection_Namespace_Object
        {
            [XmlArray("Addresses_")]
            [XmlArrayItem("Address_", Namespace = "Address_NS")]
            public IList<Address> Addresses { get; set; }
        }

        [Test]
        public void Write_Nested_Flat_Poco_Collection_Namespace_Test()
        {
            var resource = new Write_Nested_Flat_Poco_Collection_Namespace_Object
                               {
                                   Addresses = new[] {new Address {City = "Melbourne"}}
                               };
            var nav = Helpers.WriteAtom(resource);
            var mgr = new XmlNamespaceManager(nav.NameTable);
            mgr.AddNamespace("atom", "http://www.w3.org/2005/Atom");
            mgr.AddNamespace("sdata", "http://schemas.sage.com/sdata/2008/1");
            mgr.AddNamespace("a", "Address_NS");
            var node = nav.SelectSingleNode("atom:entry/sdata:payload/Account_/a:Address_/a:City", mgr);
            Assert.That(node, Is.Not.Null);
            Assert.That(node.Value, Is.EqualTo("Melbourne"));
        }

        [XmlType("Account_")]
        private class Write_Nested_Flat_Poco_Collection_Namespace_Object
        {
            [XmlElement("Address_", Namespace = "Address_NS")]
            public IList<Address> Addresses { get; set; }
        }

        [Test]
        public void Write_Nested_Poco_Name_Test()
        {
            var resource = new Write_Nested_Poco_Name_Object
                {
                    PostalAddress = new Address {City = "Melbourne"}
                };
            var nav = Helpers.WriteAtom(resource);
            var mgr = new XmlNamespaceManager(nav.NameTable);
            mgr.AddNamespace("atom", "http://www.w3.org/2005/Atom");
            mgr.AddNamespace("sdata", "http://schemas.sage.com/sdata/2008/1");
            var node = nav.SelectSingleNode("atom:entry/sdata:payload/Account/PostalAddress/City", mgr);
            Assert.That(node, Is.Not.Null);
            Assert.That(node.Value, Is.EqualTo("Melbourne"));
        }

        [XmlType("Account")]
        private class Write_Nested_Poco_Name_Object
        {
            public Address PostalAddress { get; set; }
        }

        [Test]
        public void Write_Links_Test()
        {
            var resource = new SDataResource
                {
                    Links = new List<SDataLink>
                        {
                            new SDataLink {Uri = new Uri("http://dummy/$schema"), Relation = "schemas.sage.com/sdata/link-relations/schema", Type = MediaType.Xml, Title = "Schema"},
                            new SDataLink {Uri = new Uri("http://dummy/$template"), Relation = "schemas.sage.com/sdata/link-relations/template", Type = MediaType.AtomEntry, Title = "Template"},
                            new SDataLink {Uri = new Uri("http://dummy/$service"), Relation = "schemas.sage.com/sdata/link-relations/service", Type = MediaType.Atom, Title = "Service"}
                        }
                };
            var nav = Helpers.WriteAtom(resource);
            var mgr = new XmlNamespaceManager(nav.NameTable);
            mgr.AddNamespace("atom", "http://www.w3.org/2005/Atom");
            var nodes = nav.Select("atom:entry/atom:link", mgr);
            Assert.That(nodes, Is.Not.Null);
            Assert.That(nodes.Count, Is.EqualTo(3));
            nodes.MoveNext();
            Assert.That(nodes.Current.SelectSingleNode("@href").Value, Is.EqualTo("http://dummy/$schema"));
            Assert.That(nodes.Current.SelectSingleNode("@rel").Value, Is.EqualTo("schemas.sage.com/sdata/link-relations/schema"));
            Assert.That(nodes.Current.SelectSingleNode("@type").Value, Is.EqualTo("application/xml"));
            Assert.That(nodes.Current.SelectSingleNode("@title").Value, Is.EqualTo("Schema"));
            nodes.MoveNext();
            Assert.That(nodes.Current.SelectSingleNode("@href").Value, Is.EqualTo("http://dummy/$template"));
            Assert.That(nodes.Current.SelectSingleNode("@rel").Value, Is.EqualTo("schemas.sage.com/sdata/link-relations/template"));
            Assert.That(nodes.Current.SelectSingleNode("@type").Value, Is.EqualTo("application/atom+xml;type=entry"));
            Assert.That(nodes.Current.SelectSingleNode("@title").Value, Is.EqualTo("Template"));
            nodes.MoveNext();
            Assert.That(nodes.Current.SelectSingleNode("@href").Value, Is.EqualTo("http://dummy/$service"));
            Assert.That(nodes.Current.SelectSingleNode("@rel").Value, Is.EqualTo("schemas.sage.com/sdata/link-relations/service"));
            Assert.That(nodes.Current.SelectSingleNode("@type").Value, Is.EqualTo("application/atom+xml"));
            Assert.That(nodes.Current.SelectSingleNode("@title").Value, Is.EqualTo("Service"));
        }
    }
}