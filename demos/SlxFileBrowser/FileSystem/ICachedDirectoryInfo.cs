namespace SlxFileBrowser.FileSystem
{
    internal interface ICachedDirectoryInfo : IDirectoryInfo
    {
        void Add(IDirectoryInfo directory);
        void Remove(IDirectoryInfo directory);
        void Add(IFileInfo file);
        void Remove(IFileInfo file);
    }
}