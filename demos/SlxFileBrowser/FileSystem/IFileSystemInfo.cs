namespace SlxFileBrowser.FileSystem
{
    public interface IFileSystemInfo
    {
        void Delete();
        void Refresh();
        void MoveTo(string destinationName);
        string Extension { get; }
        string FullName { get; }
        string Name { get; }
        IDriveInfo DriveInfo { get; }
    }
}