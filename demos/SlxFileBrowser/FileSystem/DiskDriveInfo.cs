using System;
using System.IO;

namespace SlxFileBrowser.FileSystem
{
    public class DiskDriveInfo : IDriveInfo
    {
        private readonly DriveInfo _driveInfo;
        private IDirectoryInfo _rootDirectory;

        public DiskDriveInfo(DriveInfo driveInfo)
        {
            _driveInfo = driveInfo;
        }

        public override string ToString()
        {
            return Name;
        }

        #region IDriveInfo Members

        public string Name
        {
            get { return _driveInfo.Name; }
        }

        public IDirectoryInfo RootDirectory
        {
            get { return _rootDirectory ?? (_rootDirectory = new DiskDirectoryInfo(this, _driveInfo.RootDirectory)); }
        }

        public IFileInfo GetFileInfo(string path)
        {
            path = Environment.ExpandEnvironmentVariables(path);
            try
            {
                return new DiskFileInfo(this, new FileInfo(path));
            }
            catch (PathTooLongException ex)
            {
                ex.Data["path"] = path;
                throw;
            }
        }

        public IDirectoryInfo GetDirectoryInfo(string path)
        {
            path = Environment.ExpandEnvironmentVariables(path);
            return new DiskDirectoryInfo(this, new DirectoryInfo(path));
        }

        #endregion
    }
}