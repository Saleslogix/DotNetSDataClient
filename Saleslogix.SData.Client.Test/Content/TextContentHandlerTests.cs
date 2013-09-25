using System.IO;
using System.Text;
using NUnit.Framework;
using Saleslogix.SData.Client.Content;

// ReSharper disable InconsistentNaming

namespace Saleslogix.SData.Client.Test.Content
{
    [TestFixture]
    public class TextContentHandlerTests
    {
        [Test]
        public void Read_Test()
        {
            object result;
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes("hello")))
            {
                result = new TextContentHandler().ReadFrom(stream);
            }
            Assert.That(result, Is.EqualTo("hello"));
        }

        [Test]
        public void Write_Test()
        {
            object result;
            using (var stream = new MemoryStream())
            {
                new TextContentHandler().WriteTo("hello", stream);
                result = Encoding.UTF8.GetString(stream.ToArray());
            }
            Assert.That(result, Is.EqualTo("hello"));
        }
    }
}