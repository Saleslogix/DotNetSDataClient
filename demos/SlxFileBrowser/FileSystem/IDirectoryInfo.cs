namespace SlxFileBrowser.FileSystem
{
    public interface IDirectoryInfo : IFileSystemInfo
    {
        IDirectoryInfo CreateSubdirectory(string path);
        IDirectoryInfo[] GetDirectories();
        IFileInfo[] GetFiles();
    }
}