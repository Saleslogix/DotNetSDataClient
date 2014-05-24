using System.IO;

namespace SlxFileBrowser.FileSystem
{
    public abstract class DiskFileSystemInfo<T> : IFileSystemInfo
        where T : FileSystemInfo
    {
        private readonly IDriveInfo _driveInfo;
        private readonly T _info;

        protected DiskFileSystemInfo(IDriveInfo driveInfo, T info)
        {
            _driveInfo = driveInfo;
            _info = info;
        }

        protected T Info
        {
            get { return _info; }
        }

        #region IFileSystemInfo Members

        public void Delete()
        {
            _info.Delete();
        }

        public void Refresh()
        {
            _info.Refresh();
        }

        public abstract void MoveTo(string destinationName);

        public string Extension
        {
            get { return _info.Extension; }
        }

        public string FullName
        {
            get { return _info.FullName; }
        }

        public string Name
        {
            get { return _info.Name; }
        }

        public IDriveInfo DriveInfo
        {
            get { return _driveInfo; }
        }

        #endregion

        public override string ToString()
        {
            return FullName;
        }
    }
}