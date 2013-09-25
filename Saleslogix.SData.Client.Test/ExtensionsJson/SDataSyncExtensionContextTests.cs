using System;
using NUnit.Framework;
using Saleslogix.SData.Client.Framework;

// ReSharper disable InconsistentNaming

namespace Saleslogix.SData.Client.Test.ExtensionsJson
{
    [TestFixture]
    public class SDataSyncExtensionContextTests
    {
        [Test]
        public void Typical_Feed()
        {
            const string xml = @"
                {
                  ""$syncMode"":""catchUp"",
                  ""$digest"":{
                    ""origin"":""http://www.example.com/sdata/myApp1/myContract/-/accounts"",
                    ""digestEntry"":[
                      {
                        ""endpoint"":""http://www.example.com/sdata/myApp1/myContract/-/accounts"",
                        ""tick"":6,
                        ""stamp"":""2008-10-30T17:23:08Z"",
                        ""conflictPriority"":2
                      },
                      {
                        ""endpoint"":""http://www.example.com/sdata/myApp2/myContract/-/accounts"",
                        ""tick"":10,
                        ""stamp"":""2008-10-30T12:16:51Z"",
                        ""conflictPriority"":1
                      }
                    ]
                  },
                  ""$resources"":[]
                }";
            var feed = Helpers.ReadJson<SDataCollection<SDataResource>>(xml);

            var syncMode = feed.SyncMode;
            Assert.That(syncMode, Is.EqualTo(SyncMode.CatchUp));

            var digest = feed.SyncDigest;
            Assert.That(digest, Is.Not.Null);
            Assert.That(digest.Origin, Is.EqualTo("http://www.example.com/sdata/myApp1/myContract/-/accounts"));
            Assert.That(digest.Entries.Length, Is.EqualTo(2));

            var entry = digest.Entries[0];
            Assert.That(entry.EndPoint, Is.EqualTo("http://www.example.com/sdata/myApp1/myContract/-/accounts"));
            Assert.That(entry.Tick, Is.EqualTo(6L));
            Assert.That(entry.Stamp, Is.EqualTo(new DateTime(2008, 10, 30, 17, 23, 08)));
            Assert.That(entry.ConflictPriority, Is.EqualTo(2));

            entry = digest.Entries[1];
            Assert.That(entry.EndPoint, Is.EqualTo("http://www.example.com/sdata/myApp2/myContract/-/accounts"));
            Assert.That(entry.Tick, Is.EqualTo(10L));
            Assert.That(entry.Stamp, Is.EqualTo(new DateTime(2008, 10, 30, 12, 16, 51)));
            Assert.That(entry.ConflictPriority, Is.EqualTo(1));
        }

        [Test]
        public void Typical_Entry()
        {
            const string json = @"
                {
                  ""$syncState"":{
                    ""endpoint"":""http://www.example.com/sdata/myApp1/myContract/-/accounts"",
                    ""tick"":5,
                    ""stamp"":""2008-10-30T14:55:43Z""
                  }
                }";
            var entry = Helpers.ReadJson<SDataResource>(json);

            var syncState = entry.SyncState;
            Assert.That(syncState.EndPoint, Is.EqualTo("http://www.example.com/sdata/myApp1/myContract/-/accounts"));
            Assert.That(syncState.Tick, Is.EqualTo(5L));
            Assert.That(syncState.Stamp, Is.EqualTo(new DateTime(2008, 10, 30, 14, 55, 43)));
        }
    }
}