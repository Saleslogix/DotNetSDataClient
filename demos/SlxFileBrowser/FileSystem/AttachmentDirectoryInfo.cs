using System;
using System.Collections.Generic;
using System.Linq;
using Saleslogix.SData.Client;
using Saleslogix.SData.Client.Linq;
using SlxFileBrowser.Model;

namespace SlxFileBrowser.FileSystem
{
    public class AttachmentDirectoryInfo : ICachedDirectoryInfo
    {
        private readonly ISDataClient _client;
        private readonly bool _formMode;
        private readonly IDirectoryInfo _parent;
        private IList<IFileInfo> _files;

        public AttachmentDirectoryInfo(ISDataClient client, bool formMode, IDirectoryInfo parent)
        {
            _client = client;
            _formMode = formMode;
            _parent = parent;
        }

        #region ICachedDirectoryInfo Members

        void ICachedDirectoryInfo.Add(IDirectoryInfo directory)
        {
            throw new NotSupportedException();
        }

        void ICachedDirectoryInfo.Remove(IDirectoryInfo directory)
        {
            throw new NotSupportedException();
        }

        void ICachedDirectoryInfo.Add(IFileInfo file)
        {
            if (_files != null)
            {
                _files.Add(file);
            }
        }

        void ICachedDirectoryInfo.Remove(IFileInfo file)
        {
            if (_files != null)
            {
                _files.Remove(file);
            }
        }

        #endregion

        #region IDirectoryInfo Members

        public IDirectoryInfo CreateSubdirectory(string path)
        {
            throw new NotSupportedException();
        }

        public IDirectoryInfo[] GetDirectories()
        {
            return new IDirectoryInfo[0];
        }

        public IFileInfo[] GetFiles()
        {
            return (_files ?? (_files = _client.Query<Attachment>()
                .OrderBy(attachment => attachment.FileName)
                .Select(attachment => (IFileInfo) new AttachmentFileInfo(_client, _formMode, this, attachment))
                .ToList())).ToArray();
        }

        #endregion

        #region IFileSystemInfo Members

        public string Extension
        {
            get { return null; }
        }

        public string FullName
        {
            get { return "sdata:\\Attachments"; }
        }

        public string Name
        {
            get { return "Attachments"; }
        }

        public IDriveInfo DriveInfo
        {
            get { return _parent.DriveInfo; }
        }

        public void Delete()
        {
            throw new NotSupportedException();
        }

        public void Refresh()
        {
            _files = null;
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