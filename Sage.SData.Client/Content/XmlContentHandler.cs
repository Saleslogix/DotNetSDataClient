using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Sage.SData.Client.Framework;
using Sage.SData.Client.Utilities;

namespace Sage.SData.Client.Content
{
    public class XmlContentHandler : IContentHandler
    {
        public object ReadFrom(Stream stream)
        {
            Guard.ArgumentNotNull(stream, "stream");

            using (var memory = new MemoryStream())
            {
                stream.CopyTo(memory);

                Tracking tracking;
                if (TryDeserializeObject(memory, out tracking))
                {
                    return tracking;
                }

                Diagnoses diagnoses;
                if (TryDeserializeObject(memory, out diagnoses))
                {
                    return diagnoses;
                }

                Diagnosis diagnosis;
                if (TryDeserializeObject(memory, out diagnosis))
                {
                    return new Diagnoses {diagnosis};
                }

                memory.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(memory))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private static bool TryDeserializeObject<T>(Stream stream, out T obj)
        {
            var serializer = new XmlSerializer(typeof (T));
            stream.Seek(0, SeekOrigin.Begin);

            try
            {
                obj = (T) serializer.Deserialize(stream);
                return true;
            }
            catch (XmlException)
            {
            }
            catch (InvalidOperationException)
            {
            }

            obj = default(T);
            return false;
        }

        public void WriteTo(object obj, Stream stream, INamingScheme namingScheme = null)
        {
            Guard.ArgumentNotNull(obj, "obj");
            Guard.ArgumentNotNull(stream, "stream");

            var type = obj.GetType();
            if (Type.GetTypeCode(type) == TypeCode.Object)
            {
                var serializer = new XmlSerializer(type);
                serializer.Serialize(stream, obj);
            }
            else
            {
                var writer = new StreamWriter(stream);
                writer.Write(obj.ToString());
                writer.Flush();
            }
        }
    }
}