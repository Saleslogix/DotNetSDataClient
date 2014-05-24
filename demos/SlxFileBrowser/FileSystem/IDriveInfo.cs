namespace SlxFileBrowser.FileSystem
{
    public interface IDriveInfo
    {
        string Name { get; }
        IDirectoryInfo RootDirectory { get; }
        IFileInfo GetFileInfo(string path);
        IDirectoryInfo GetDirectoryInfo(string path);
    }
}