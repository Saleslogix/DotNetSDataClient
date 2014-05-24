using System;
using System.IO;

namespace SlxFileBrowser.FileSystem
{
    public class DiskFileInfo : DiskFileSystemInfo<FileInfo>, IFileInfo
    {
        public DiskFileInfo(IDriveInfo driveInfo, FileInfo info)
            : base(driveInfo, info)
        {
        }

        #region IFileInfo Members

        public Stream Open(FileMode mode, FileAccess access)
        {
            Info.Refresh();
            return Info.Open(mode, access);
        }

        #endregion

        public override void MoveTo(string destinationPath)
        {
            Info.Refresh();
            if (!Info.Exists)
            {
                return;
            }

            try
            {
                Info.MoveTo(destinationPath);
            }
            catch (Exception)
            {
                if (!File.Exists(destinationPath) || File.GetAttributes(destinationPath).HasFlag(FileAttributes.Directory))
                {
                    throw;
                }

                var fileAttributes = File.GetAttributes(destinationPath);
                if (fileAttributes.HasFlag(FileAttributes.ReadOnly))
                {
                    File.SetAttributes(destinationPath, fileAttributes & ~FileAttributes.ReadOnly);
                }
                File.Delete(destinationPath);
                Info.MoveTo(destinationPath);
            }
        }
    }
}