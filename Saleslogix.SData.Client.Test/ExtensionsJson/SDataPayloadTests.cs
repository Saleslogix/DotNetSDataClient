using System;
using NUnit.Framework;

// ReSharper disable InconsistentNaming

namespace Saleslogix.SData.Client.Test.ExtensionsJson
{
    [TestFixture]
    public class SDataPayloadTests
    {
        [Test]
        public void Typical_Payload()
        {
            const string json = @"
                {
                  ""$key"":""43660""
                  ""orderDate"":""2001-07-01"",
                  ""shipDate"":null,
                  ""contact"":{
                    ""$key"":""216"",
                    ""$url"":""http://www.example.com/sdata/myApp/myContract/-/contacts('216')"",
                    ""$lookup"":""http://www.example.com/sdata/myApp/myContract/-/contacts""
                  }
                  ""orderLines"":{
                    ""$url"":""http://www.example.com/sdata/myApp/myContract/-/salesOrderLines?where=salesOrderID%20eq%2043660"",
                    ""$resources"":[]
                  }
                }";
            var payload = Helpers.ReadJson<SDataResource>(json);

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
            Assert.That(obj, Is.Empty);
        }

        [Test]
        public void Object_Property_Without_Attributes()
        {
            const string json = @"
                {
                  ""contact"": {
                    ""firstName"":""John"",
                    ""lastName"":""Smith""
                  }
                }";
            var payload = Helpers.ReadJson<SDataResource>(json);

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
            const string json = @"
                {
                  ""orderLines"":{
                    ""$resources"":[]
                  }
                }";
            var payload = Helpers.ReadJson<SDataResource>(json);

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
            const string json = @"
                {
                  ""orderLines"":{
                    ""$resources"":[]
                  }
                }";
            var payload = Helpers.ReadJson<SDataResource>(json);

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
            const string json = @"
                {
                  ""orderLines"":{
                    ""$resources"":[
                      {
                        ""$key"":""43660-1""
                      }
                    ]
                  }
                }";
            var payload = Helpers.ReadJson<SDataResource>(json);

            Assert.That(payload.Count, Is.EqualTo(1));

            object value;
            Assert.IsTrue(payload.TryGetValue("orderLines", out value));
            Assert.That(value, Is.InstanceOf<SDataCollection<SDataResource>>());
            var col = (SDataCollection<SDataResource>) value;
            Assert.That(col.Count, Is.EqualTo(1));

            var item = col[0];
            Assert.That(item.Key, Is.EqualTo("43660-1"));
            Assert.That(item, Is.Empty);
        }

        [Test]
        public void Collection_Property_Without_Attributes()
        {
            const string json = @"
                {
                  ""orderLines"":{
                    ""$resources"":[
                      {
                        ""$key"":""43660-1""
                      },
                      {
                        ""$key"":""43660-2""
                      }
                    ]
                  }
                }";
            var payload = Helpers.ReadJson<SDataResource>(json);

            Assert.That(payload.Count, Is.EqualTo(1));

            object value;
            Assert.IsTrue(payload.TryGetValue("orderLines", out value));
            Assert.That(value, Is.InstanceOf<SDataCollection<SDataResource>>());
            var col = (SDataCollection<SDataResource>) value;
            Assert.That(col.Count, Is.EqualTo(2));

            var item = col[0];
            Assert.That(item.Key, Is.EqualTo("43660-1"));
            Assert.That(item, Is.Empty);

            item = col[1];
            Assert.That(item.Key, Is.EqualTo("43660-2"));
            Assert.That(item, Is.Empty);
        }

        [Test]
        public void Unnested_Collection_Items()
        {
            const string json = @"
                {
                  ""origin"":""http://www.example.com/sdata/myApp1/myContract/-/accounts"",
                  ""digestEntry"":{
                    ""$resources"":[
                      {
                        ""tick"":5
                      },
                      {
                        ""tick"":11
                      }
                    ]
                  }
                }";
            var payload = Helpers.ReadJson<SDataResource>(json);

            Assert.That(payload.Count, Is.EqualTo(2));

            object value;
            Assert.IsTrue(payload.TryGetValue("origin", out value));
            Assert.That(value, Is.EqualTo("http://www.example.com/sdata/myApp1/myContract/-/accounts"));

            Assert.IsTrue(payload.TryGetValue("digestEntry", out value));
            Assert.That(value, Is.InstanceOf<SDataCollection<SDataResource>>());
            var col = (SDataCollection<SDataResource>) value;
            Assert.That(col.Count, Is.EqualTo(2));

            var item = col[0];
            Assert.That(item.Count, Is.EqualTo(1));
            Assert.IsTrue(item.TryGetValue("tick", out value));
            Assert.That(value, Is.EqualTo(5));

            item = col[1];
            Assert.That(item.Count, Is.EqualTo(1));
            Assert.IsTrue(item.TryGetValue("tick", out value));
            Assert.That(value, Is.EqualTo(11));
        }

        [Test]
        public void Loaded_Collection_Infers_Item_Resource_Name()
        {
            const string json = @"
                {
                  ""orderLines"":{
                    ""$resources"":[
                      {
                        ""$key"":""43660-1""
                      },
                      {
                        ""$key"":""43660-2""
                      }
                    ]
                  }
                }";
            var payload = Helpers.ReadJson<SDataResource>(json);
            Assert.That(payload["orderLines"], Is.InstanceOf<SDataCollection<SDataResource>>());
        }

        [Test]
        public void Primitive_Values_Formatted_Appropriately()
        {
            var payload = new SDataResource
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
                                  {"DateTimeOffset", DateTimeOffset.Now}
                              };
            var nav = Helpers.WriteJson(payload);

            var assertDoesNotThrow = new Action<string, Func<object, object>>(
                (name, action) =>
                    {
                        var value = nav[name];
                        Assert.That(value, Is.Not.Null);
                        Assert.DoesNotThrow(() => action(value));
                    });
            assertDoesNotThrow("byte", x => Convert.ToByte(x));
            assertDoesNotThrow("sbyte", x => Convert.ToSByte(x));
            assertDoesNotThrow("short", x => Convert.ToInt16(x));
            assertDoesNotThrow("ushort", x => Convert.ToUInt16(x));
            assertDoesNotThrow("int", x => Convert.ToInt32(x));
            assertDoesNotThrow("uint", x => Convert.ToUInt32(x));
            assertDoesNotThrow("long", x => Convert.ToInt64(x));
            assertDoesNotThrow("ulong", x => Convert.ToUInt64(x));
            assertDoesNotThrow("bool", x => Convert.ToBoolean(x));
            assertDoesNotThrow("char", x => Convert.ToChar(x));
            assertDoesNotThrow("float", x => Convert.ToSingle(x));
            assertDoesNotThrow("double", x => Convert.ToDouble(x));
            assertDoesNotThrow("decimal", x => Convert.ToDecimal(x));
            assertDoesNotThrow("Guid", x => new Guid(x.ToString()));
            assertDoesNotThrow("DateTime", x => (string) x);
            assertDoesNotThrow("DateTimeOffset", x => (string) x);
        }

        [Test]
        public void Object_Property_With_Single_Child_Property()
        {
            const string json = @"
                {
                  ""response"":{
                    ""unitPrice"":""100""
                  }
                }";
            var payload = Helpers.ReadJson<SDataResource>(json);

            object value;
            Assert.That(payload.TryGetValue("response", out value), Is.True);
            Assert.That(value, Is.InstanceOf<SDataResource>());
        }

        [Test]
        public void Uri_Property_Should_Be_Escaped_When_Written()
        {
            var payload = new SDataResource
                              {
                                  Url = new Uri("http://localhost/sdata/invoices('`%^ []{}<>')")
                              };
            var nav = Helpers.WriteJson(payload);
            var url = nav["$url"];
            Assert.That(url, Is.EqualTo("http://localhost/sdata/invoices('%60%25%5E%20%5B%5D%7B%7D%3C%3E')"));
        }
    }
}