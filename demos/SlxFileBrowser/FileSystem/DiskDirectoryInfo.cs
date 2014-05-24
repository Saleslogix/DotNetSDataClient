using System;
using System.IO;
using System.Linq;

namespace SlxFileBrowser.FileSystem
{
    public class DiskDirectoryInfo : DiskFileSystemInfo<DirectoryInfo>, IDirectoryInfo
    {
        private static readonly IDirectoryInfo[] _emptyDirectoryInfos = new IDirectoryInfo[0];
        private static readonly IFileInfo[] _emptyFileInfos = new IFileInfo[0];

        public DiskDirectoryInfo(IDriveInfo driveInfo, DirectoryInfo info)
            : base(driveInfo, info)
        {
        }

        #region IDirectoryInfo Members

        public IDirectoryInfo CreateSubdirectory(string path)
        {
            return new DiskDirectoryInfo(DriveInfo, Info.CreateSubdirectory(path));
        }

        public IDirectoryInfo[] GetDirectories()
        {
            try
            {
                Info.Refresh();
                return Info.GetDirectories().Select(dir => (IDirectoryInfo) new DiskDirectoryInfo(DriveInfo, dir)).ToArray();
            }
            catch (Exception)
            {
                return _emptyDirectoryInfos;
            }
        }

        public IFileInfo[] GetFiles()
        {
            try
            {
                Info.Refresh();
                return Info.GetFiles().Select(file => (IFileInfo) new DiskFileInfo(DriveInfo, file)).ToArray();
            }
            catch (Exception)
            {
                return _emptyFileInfos;
            }
        }

        #endregion

        public override void MoveTo(string destinationPath)
        {
            Info.Refresh();
            Info.MoveTo(destinationPath);
        }
    }
}