using System;
using System.Linq;
using Saleslogix.SData.Client;

namespace SlxFileBrowser.FileSystem
{
    public class SDataDirectoryInfo : IDirectoryInfo
    {
        private readonly ISDataClient _client;
        private readonly bool _formMode;
        private readonly IDriveInfo _drive;
        private IDirectoryInfo[] _directories;

        public SDataDirectoryInfo(ISDataClient client, bool formMode, IDriveInfo drive)
        {
            _client = client;
            _formMode = formMode;
            _drive = drive;
        }

        #region IDirectoryInfo Members

        public IDirectoryInfo CreateSubdirectory(string path)
        {
            throw new NotSupportedException();
        }

        public IDirectoryInfo[] GetDirectories()
        {
            return _directories ?? (_directories = new[] {new AttachmentDirectoryInfo(_client, _formMode, this)}
                .Concat(LibraryDirectoryInfo.GetDirectories(_client, _formMode, this, "0"))
                .ToArray());
        }

        public IFileInfo[] GetFiles()
        {
            return new IFileInfo[0];
        }

        #endregion

        #region IFileSystemInfo Members

        public string Extension
        {
            get { return null; }
        }

        public string FullName
        {
            get { return "sdata:\\"; }
        }

        public string Name
        {
            get { return "sdata:\\"; }
        }

        public IDriveInfo DriveInfo
        {
            get { return _drive; }
        }

        public void Delete()
        {
            throw new NotSupportedException();
        }

        public void Refresh()
        {
            _directories = null;
        }

        public void MoveTo(string destinationName)
        {
            throw new NotSupportedException();
        }

        #endregion

        public override string ToString()
        {
            return FullName;
        }
    }
}