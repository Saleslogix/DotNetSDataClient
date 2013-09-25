using System.IO;

namespace Sage.SData.Client.Content
{
    public interface IContentHandler
    {
        object ReadFrom(Stream stream);
        void WriteTo(object obj, Stream stream, INamingScheme namingScheme = null);
    }
}