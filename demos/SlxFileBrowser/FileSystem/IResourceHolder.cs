using System.IO;

namespace SlxFileBrowser.FileSystem
{
    public interface IResourceHolder
    {
        object Resource { get; }
        void Save(Stream stream);
    }
}