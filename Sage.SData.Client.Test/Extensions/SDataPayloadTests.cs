using System;
using System.Xml;
using NUnit.Framework;
using Sage.SData.Client.Framework;

// ReSharper disable InconsistentNaming

namespace Sage.SData.Client.Test.Extensions
{
    [TestFixture]
    public class SDataPayloadTests
    {
        [Test]
        public void Typical_Payload()
        {
            const string xml = @"
                    <entry xmlns=""http://www.w3.org/2005/Atom"">
                      <sdata:payload xmlns:sdata=""http://schemas.sage.com/sdata/2008/1"">
                        <salesOrder sdata:key=""43660""
                                    xmlns=""http://schemas.sage.com/myContract""
                                    xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
                          <orderDate>2001-07-01</orderDate>
                          <shipDate xsi:nil=""true"" />
                          <contact sdata:key=""216"" 
                                   sdata:uri=""http://www.example.com/sdata/myApp/myContract/-/contacts('216')"" 
                                   sdata:lookup=""http://www.example.com/sdata/myApp/myContract/-/contacts""/>
                          <orderLines sdata:uri=""http://www.example.com/sdata/myApp/myContract/-/salesOrderLines?where=salesOrderID%20eq%2043660""/>
                        </salesOrder>
                      </sdata:payload>
                    </entry>";
            var payload = Helpers.ReadAtom<SDataResource>(xml);

            Assert.That(payload.XmlLocalName, Is.EqualTo("salesOrder"));
            Assert.That(payload.XmlNamespace, Is.EqualTo("http://schemas.sage.com/myContract"));
            Assert.That(payload.Key, Is.EqualTo("43660"));
            Assert.That(payload.Count, Is.EqualTo(4));

            object value;
            Assert.IsTrue(payload.TryGetValue("orderDate", out value));
            Assert.That(value, Is.EqualTo("2001-07-01"));

            Assert.IsTrue(payload.TryGetValue("shipDate", out value));
            Assert.That(value, Is.Null);

            Assert.IsTrue(payload.TryGetValue("contact", out value));
            Assert.That(value, Is.InstanceOf<SDataResource>());
            var obj = (SDataResource) value;
            Assert.That(obj.Key, Is.EqualTo("216"));
            Assert.That(obj.Url, Is.EqualTo(new Uri("http://www.example.com/sdata/myApp/myContract/-/contacts('216')")));
            Assert.That(obj.Lookup, Is.EqualTo("http://www.example.com/sdata/myApp/myContract/-/contacts"));
            Assert.That(obj, Is.Empty);

            Assert.IsTrue(payload.TryGetValue("orderLines", out value));
            Assert.That(value, Is.InstanceOf<SDataCollection<SDataResource>>());
            var col = (SDataCollection<SDataResource>) value;
            Assert.That(col.Url, Is.EqualTo(new Uri("http://www.example.com/sdata/myApp/myContract/-/salesOrderLines?where=salesOrderID%20eq%2043660")));
            Assert.That(col, Is.Empty);
        }

        [Test]
        public void Object_Property_Without_Attributes()
        {
            const string xml = @"
                    <entry xmlns=""http://www.w3.org/2005/Atom"">
                      <sdata:payload xmlns:sdata=""http://schemas.sage.com/sdata/2008/1"">
                        <salesOrder>
                          <contact>
                            <firstName>John</firstName>
                            <lastName>Smith</lastName>
                          </contact>
                        </salesOrder>
                      </sdata:payload>
                    </entry>";
            var payload = Helpers.ReadAtom<SDataResource>(xml);

            Assert.That(payload.XmlLocalName, Is.EqualTo("salesOrder"));
            Assert.That(payload.Count, Is.EqualTo(1));

            object value;
            Assert.IsTrue(payload.TryGetValue("contact", out value));
            Assert.That(value, Is.InstanceOf<SDataResource>());
            var obj = (SDataResource) value;
            Assert.That(obj.Count, Is.EqualTo(2));

            Assert.IsTrue(obj.TryGetValue("firstName", out value));
            Assert.That(value, Is.EqualTo("John"));

            Assert.IsTrue(obj.TryGetValue("lastName", out value));
            Assert.That(value, Is.EqualTo("Smith"));
        }

        [Test]
        public void Empty_Collection_Property_Without_Attributes()
        {
            const string xml = @"
                    <atom:entry xmlns:atom=""http://www.w3.org/2005/Atom"">
                      <sdata:payload xmlns:sdata=""http://schemas.sage.com/sdata/2008/1"">
                        <salesOrder>
                          <orderLines />
                        </salesOrder>
                      </sdata:payload>
                    </atom:entry>";
            var payload = Helpers.ReadAtom<SDataResource>(xml);

            Assert.That(payload.XmlLocalName, Is.EqualTo("salesOrder"));
            Assert.That(payload.Count, Is.EqualTo(1));

            object value;
            Assert.IsTrue(payload.TryGetValue("orderLines", out value));
            Assert.That(value, Is.InstanceOf<SDataCollection<SDataResource>>());
            var col = (SDataCollection<SDataResource>) value;
            Assert.That(col, Is.Empty);
        }

        [Test]
        public void Empty_Collection_Property_Without_Attributes_Or_Namespace()
        {
            const string xml = @"
                    <atom:entry xmlns:atom=""http://www.w3.org/2005/Atom"">
                      <sdata:payload xmlns:sdata=""http://schemas.sage.com/sdata/2008/1"">
                        <x:salesOrder xmlns:x=""http://schemas.sage.com/dynamic/2007"">
                          <orderLines />
                        </x:salesOrder>
                      </sdata:payload>
                    </atom:entry>";
            var payload = Helpers.ReadAtom<SDataResource>(xml);

            Assert.That(payload.XmlLocalName, Is.EqualTo("salesOrder"));
            Assert.That(payload.Count, Is.EqualTo(1));

            object value;
            Assert.IsTrue(payload.TryGetValue("orderLines", out value));
            Assert.That(value, Is.InstanceOf<SDataCollection<SDataResource>>());
            var col = (SDataCollection<SDataResource>) value;
            Assert.That(col, Is.Empty);
        }

        [Test]
        public void Collection_Of_One_Property_Without_Attributes()
        {
            const string xml = @"
                    <entry xmlns=""http://www.w3.org/2005/Atom"">
                      <sdata:payload xmlns:sdata=""http://schemas.sage.com/sdata/2008/1"">
                        <salesOrder xmlns:sdata=""http://schemas.sage.com/sdata/2008/1"">
                          <orderLines sdata:uri=""http://www.example.com/sdata/myApp/myContract/-/salesOrderLines?where=salesOrderID%20eq%2043660"">
                            <salesOrderLine sdata:key=""43660-1"" />
                          </orderLines>
                        </salesOrder>
                      </sdata:payload>
                    </entry>";
            var payload = Helpers.ReadAtom<SDataResource>(xml);

            Assert.That(payload.XmlLocalName, Is.EqualTo("salesOrder"));
            Assert.That(payload.Count, Is.EqualTo(1));

            object value;
            Assert.IsTrue(payload.TryGetValue("orderLines", out value));
            Assert.That(value, Is.InstanceOf<SDataCollection<SDataResource>>());
            var col = (SDataCollection<SDataResource>) value;
            Assert.That(col.Count, Is.EqualTo(1));

            var item = col[0];
            Assert.That(item.XmlLocalName, Is.EqualTo("salesOrderLine"));
            Assert.That(item.Key, Is.EqualTo("43660-1"));
            Assert.That(item, Is.Empty);
        }

        [Test]
        public void Collection_Property_Without_Attributes()
        {
            const string xml = @"
                    <entry xmlns=""http://www.w3.org/2005/Atom"">
                      <sdata:payload xmlns:sdata=""http://schemas.sage.com/sdata/2008/1"">
                        <salesOrder xmlns:sdata=""http://schemas.sage.com/sdata/2008/1"">
                          <orderLines>
                            <salesOrderLine sdata:key=""43660-1"" />
                            <salesOrderLine sdata:key=""43660-2"" />
                          </orderLines>
                        </salesOrder>
                      </sdata:payload>
                    </entry>";
            var payload = Helpers.ReadAtom<SDataResource>(xml);

            Assert.That(payload.XmlLocalName, Is.EqualTo("salesOrder"));
            Assert.That(payload.Count, Is.EqualTo(1));

            object value;
            Assert.IsTrue(payload.TryGetValue("orderLines", out value));
            Assert.That(value, Is.InstanceOf<SDataCollection<SDataResource>>());
            var col = (SDataCollection<SDataResource>) value;
            Assert.That(col.Count, Is.EqualTo(2));

            var item = col[0];
            Assert.That(item.XmlLocalName, Is.EqualTo("salesOrderLine"));
            Assert.That(item.Key, Is.EqualTo("43660-1"));
            Assert.That(item, Is.Empty);

            item = col[1];
            Assert.That(item.XmlLocalName, Is.EqualTo("salesOrderLine"));
            Assert.That(item.Key, Is.EqualTo("43660-2"));
            Assert.That(item, Is.Empty);
        }

        [Test]
        public void Unnested_Collection_Items()
        {
            const string xml = @"
                    <entry xmlns=""http://www.w3.org/2005/Atom"">
                      <sdata:payload xmlns:sdata=""http://schemas.sage.com/sdata/2008/1"">
                        <digest xmlns=""http://schemas.sage.com/sdata/sync/2008/1"">
                          <origin>http://www.example.com/sdata/myApp1/myContract/-/accounts</origin>
                          <digestEntry>
                            <tick>5</tick>
                          </digestEntry>
                          <digestEntry>
                            <tick>11</tick>
                          </digestEntry>
                        </digest>
                      </sdata:payload>
                    </entry>";
            var payload = Helpers.ReadAtom<SDataResource>(xml);

            Assert.That(payload.XmlLocalName, Is.EqualTo("digest"));
            Assert.That(payload.XmlNamespace, Is.EqualTo("http://schemas.sage.com/sdata/sync/2008/1"));
            Assert.That(payload.Count, Is.EqualTo(2));

            object value;
            Assert.IsTrue(payload.TryGetValue("origin", out value));
            Assert.That(value, Is.EqualTo("http://www.example.com/sdata/myApp1/myContract/-/accounts"));

            Assert.IsTrue(payload.TryGetValue("digestEntry", out value));
            Assert.That(value, Is.InstanceOf<SDataCollection<SDataResource>>());
            var col = (SDataCollection<SDataResource>) value;
            Assert.That(col.Count, Is.EqualTo(2));

            var item = col[0];
            Assert.That(item.XmlLocalName, Is.EqualTo("digestEntry"));
            Assert.That(item.Count, Is.EqualTo(1));
            Assert.IsTrue(item.TryGetValue("tick", out value));
            Assert.That(value, Is.EqualTo("5"));

            item = col[1];
            Assert.That(item.XmlLocalName, Is.EqualTo("digestEntry"));
            Assert.That(item.Count, Is.EqualTo(1));
            Assert.IsTrue(item.TryGetValue("tick", out value));
            Assert.That(value, Is.EqualTo("11"));
        }

        [Test]
        public void Loaded_Collection_Infers_Item_Resource_Name()
        {
            const string xml = @"
                    <entry xmlns=""http://www.w3.org/2005/Atom"">
                      <sdata:payload xmlns:sdata=""http://schemas.sage.com/sdata/2008/1"">
                        <salesOrder xmlns:sdata=""http://schemas.sage.com/sdata/2008/1"">
                          <orderLines>
                            <salesOrderLine sdata:key=""43660-1"" />
                            <salesOrderLine sdata:key=""43660-2"" />
                          </orderLines>
                        </salesOrder>
                      </sdata:payload>
                    </entry>";
            var payload = Helpers.ReadAtom<SDataResource>(xml);
            Assert.That(payload["orderLines"], Is.InstanceOf<SDataCollection<SDataResource>>());
            var orderLines = (SDataCollection<SDataResource>) payload["orderLines"];
            Assert.That(orderLines.XmlLocalName, Is.EqualTo("salesOrderLine"));
        }

        [Test]
        public void Written_Collection_Uses_Item_Resource_Name()
        {
            var payload = new SDataResource("salesOrder")
                              {
                                  {
                                      "orderLines", new SDataCollection<SDataResource>("salesOrderLine")
                                                        {
                                                            new SDataResource {Key = "43660-1"}
                                                        }
                                  }
                              };
            var nav = Helpers.WriteAtom(payload);
            var node = nav.SelectSingleNode("*//salesOrder/orderLines/salesOrderLine");
            Assert.That(node, Is.Not.Null);
        }

        [Test]
        public void Primitive_Values_Formatted_Appropriately()
        {
            var payload = new SDataResource("salesOrder")
                              {
                                  {"byte", byte.MaxValue},
                                  {"sbyte", sbyte.MaxValue},
                                  {"short", short.MaxValue},
                                  {"ushort", ushort.MaxValue},
                                  {"int", int.MaxValue},
                                  {"uint", uint.MaxValue},
                                  {"long", long.MaxValue},
                                  {"ulong", ulong.MaxValue},
                                  {"bool", true},
                                  {"char", 'z'},
                                  {"float", float.MaxValue},
                                  {"double", double.MaxValue},
                                  {"decimal", decimal.MaxValue},
                                  {"Guid", Guid.NewGuid()},
                                  {"DateTime", DateTime.Now},
                                  {"DateTimeOffset", DateTimeOffset.Now},
                                  {"TimeSpan", DateTime.Now.TimeOfDay}
                              };
            var nav = Helpers.WriteAtom(payload);
            nav = nav.SelectSingleNode("*//salesOrder");

            var assertDoesNotThrow = new Action<string, Action<string>>(
                (name, action) =>
                    {
                        var node = nav.SelectSingleNode(name);
                        Assert.That(node, Is.Not.Null);
                        Assert.DoesNotThrow(() => action(node.Value));
                    });
            assertDoesNotThrow("byte", x => XmlConvert.ToByte(x));
            assertDoesNotThrow("sbyte", x => XmlConvert.ToSByte(x));
            assertDoesNotThrow("short", x => XmlConvert.ToInt16(x));
            assertDoesNotThrow("ushort", x => XmlConvert.ToUInt16(x));
            assertDoesNotThrow("int", x => XmlConvert.ToInt32(x));
            assertDoesNotThrow("uint", x => XmlConvert.ToUInt32(x));
            assertDoesNotThrow("long", x => XmlConvert.ToInt64(x));
            assertDoesNotThrow("ulong", x => XmlConvert.ToUInt64(x));
            assertDoesNotThrow("bool", x => XmlConvert.ToBoolean(x));
            assertDoesNotThrow("char", x => XmlConvert.ToChar(x));
            assertDoesNotThrow("float", x => XmlConvert.ToSingle(x));
            assertDoesNotThrow("double", x => XmlConvert.ToDouble(x));
            assertDoesNotThrow("decimal", x => XmlConvert.ToDecimal(x));
            assertDoesNotThrow("Guid", x => XmlConvert.ToGuid(x));
            assertDoesNotThrow("DateTime", x => XmlConvert.ToDateTime(x, XmlDateTimeSerializationMode.RoundtripKind));
            assertDoesNotThrow("DateTimeOffset", x => XmlConvert.ToDateTimeOffset(x));
            assertDoesNotThrow("TimeSpan", x => XmlConvert.ToTimeSpan(x));
        }

        [Test]
        public void Collection_Items_Can_Be_In_Different_Namespace()
        {
            var payload = new SDataResource("tradingAccount", "http://gcrm.com")
                              {
                                  {
                                      "emails", new SDataCollection<SDataResource>("email")
                                                    {
                                                        new SDataResource
                                                            {
                                                                XmlNamespace = "http://common.com"
                                                            }
                                                    }
                                  }
                              };
            var nav = Helpers.WriteAtom(payload);
            var mgr = new XmlNamespaceManager(nav.NameTable);
            mgr.AddNamespace("g", "http://gcrm.com");
            mgr.AddNamespace("c", "http://common.com");
            var node = nav.SelectSingleNode("*//g:tradingAccount/g:emails/c:email", mgr);
            Assert.That(node, Is.Not.Null);
        }

        [Test]
        public void Object_Property_With_Single_Child_Property()
        {
            const string xml = @"
                    <entry xmlns=""http://www.w3.org/2005/Atom"">
                      <sdata:payload xmlns:sdata=""http://schemas.sage.com/sdata/2008/1"">
                        <productComputeSimplePrice>
                          <response>
                            <unitPrice>100</unitPrice>
                          </response>
                        </productComputeSimplePrice>
                      </sdata:payload>
                    </entry>";
            var payload = Helpers.ReadAtom<SDataResource>(xml);

            object value;
            Assert.That(payload.TryGetValue("response", out value), Is.True);
            Assert.That(value, Is.InstanceOf<SDataResource>());
        }

        [Test]
        public void Uri_Property_Should_Be_Escaped_When_Written()
        {
            var payload = new SDataResource
                              {
                                  XmlLocalName = "person",
                                  XmlNamespace = "http://test.com",
                                  Url = new Uri("http://localhost/person('`%^ []{}<>')")
                              };
            var nav = Helpers.WriteAtom(payload);
            var mgr = new XmlNamespaceManager(nav.NameTable);
            mgr.AddNamespace("sdata", Common.SData.Namespace);
            mgr.AddNamespace("test", "http://test.com");
            var node = nav.SelectSingleNode("*//test:person/@sdata:uri", mgr);
            Assert.That(node, Is.Not.Null);
            Assert.That(node.Value, Is.EqualTo("http://localhost/person('%60%25%5E%20%5B%5D%7B%7D%3C%3E')"));
        }
    }
}