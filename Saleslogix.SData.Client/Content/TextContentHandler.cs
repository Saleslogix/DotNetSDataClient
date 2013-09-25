using System.IO;
using Sage.SData.Client.Utilities;

namespace Sage.SData.Client.Content
{
    public class TextContentHandler : IContentHandler
    {
        public object ReadFrom(Stream stream)
        {
            Guard.ArgumentNotNull(stream, "stream");

            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public void WriteTo(object obj, Stream stream, INamingScheme namingScheme = null)
        {
            Guard.ArgumentNotNull(obj, "obj");
            Guard.ArgumentNotNull(stream, "stream");

            using (var writer = new StreamWriter(stream))
            {
                writer.Write(obj.ToString());
            }
        }
    }
}