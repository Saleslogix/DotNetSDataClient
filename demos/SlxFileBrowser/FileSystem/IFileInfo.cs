using System.IO;

namespace SlxFileBrowser.FileSystem
{
    public interface IFileInfo : IFileSystemInfo
    {
        Stream Open(FileMode mode, FileAccess access);
    }
}