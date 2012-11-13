using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Sage.SData.Client.Framework
{
    public static class StreamExtensions
    {
        public static void CopyTo(this Stream source, Stream destination)
        {
            var buffer = new byte[0x1000];
            int num;
            while ((num = source.Read(buffer, 0, buffer.Length)) != 0)
            {
                destination.Write(buffer, 0, num);
            }
        }

        public static T DeserializeXml<T>(this Stream stream)
        {
            var serializer = new XmlSerializer(typeof (T));

            try
            {
                return (T) serializer.Deserialize(stream);
            }
            catch (XmlException)
            {
            }
            catch (InvalidOperationException)
            {
            }

            return default(T);
        }
    }
}