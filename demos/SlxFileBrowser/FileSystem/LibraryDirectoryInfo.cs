using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Saleslogix.SData.Client;
using Saleslogix.SData.Client.Linq;
using SlxFileBrowser.Model;

namespace SlxFileBrowser.FileSystem
{
    public class LibraryDirectoryInfo : ICachedDirectoryInfo, IResourceHolder
    {
        private readonly ISDataClient _client;
        private readonly bool _formMode;
        private IDirectoryInfo _parent;
        private LibraryDirectory _directory;
        private IList<IDirectoryInfo> _directories;
        private IList<IFileInfo> _files;

        private LibraryDirectoryInfo(ISDataClient client, bool formMode, IDirectoryInfo parent, LibraryDirectory directory)
        {
            _client = client;
            _formMode = formMode;
            _parent = parent;
            _directory = directory;
        }

        internal static IList<IDirectoryInfo> GetDirectories(ISDataClient client, bool formMode, IDirectoryInfo parent, string id)
        {
            return client.Query<LibraryDirectory>()
                .Where(x => x.ParentId == id)
                .OrderBy(x => x.DirectoryName)
                .Select(directory => (IDirectoryInfo) new LibraryDirectoryInfo(client, formMode, parent, directory))
                .ToList();
        }

        private void RefreshLoadedDirectoryResources()
        {
            if (_directories == null)
            {
                return;
            }

            foreach (var directory in _directories.Cast<LibraryDirectoryInfo>())
            {
                directory.Refresh();
                directory.RefreshLoadedDirectoryResources();
            }
        }

        #region IResourceHolder Members

        public object Resource
        {
            get { return _directory; }
        }

        public void Save(Stream stream)
        {
            _directory = string.IsNullOrEmpty(_directory.Key)
                ? _client.Post(_directory)
                : _client.Put(_directory);
        }

        #endregion

        #region ICachedDirectoryInfo Members

        void ICachedDirectoryInfo.Add(IDirectoryInfo directory)
        {
            if (_directories != null)
            {
                _directories.Add(directory);
            }
        }

        void ICachedDirectoryInfo.Remove(IDirectoryInfo directory)
        {
            if (_directories != null)
            {
                _directories.Remove(directory);
            }
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
            if (path.IndexOf("\\", StringComparison.Ordinal) < 0)
            {
                var subDir = new LibraryDirectory
                    {
                        DirectoryName = path,
                        ParentId = _directory.Key
                    };
                var info = new LibraryDirectoryInfo(_client, _formMode, this, subDir);
                info.Save(null);

                if (_directories != null)
                {
                    _directories.Add(info);
                }

                return info;
            }

            throw new NotSupportedException();
        }

        public IDirectoryInfo[] GetDirectories()
        {
            return (_directories ?? (_directories = GetDirectories(_client, _formMode, this, _directory.Key))).ToArray();
        }

        public IFileInfo[] GetFiles()
        {
            return (_files ?? (_files = _client.Query<LibraryDocument>()
                .Where(document => (string) document.Directory["Id"] == _directory.Key)
                .OrderBy(document => document.FileName)
                .Select(document => (IFileInfo) new LibraryFileInfo(_client, _formMode, this, document))
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
            get { return Path.Combine(_parent.FullName, _directory.DirectoryName); }
        }

        public string Name
        {
            get { return _directory.DirectoryName; }
        }

        public IDriveInfo DriveInfo
        {
            get { return _parent.DriveInfo; }
        }

        public void Delete()
        {
            var parent = _parent as ICachedDirectoryInfo;
            if (parent == null)
            {
                throw new NotSupportedException();
            }

            if (!string.IsNullOrEmpty(_directory.Key))
            {
                _client.Delete(_directory);
            }
            parent.Remove(this);
        }

        public void Refresh()
        {
            _directory = _client.Get<LibraryDirectory>(_directory.Key);
            _directories = null;
            _files = null;
        }

        public void MoveTo(string destinationName)
        {
            var oldParent = _parent as ICachedDirectoryInfo;
            if (oldParent == null)
            {
                throw new NotSupportedException();
            }

            var dirName = Path.GetDirectoryName(destinationName);
            var newParent = DriveInfo.GetDirectoryInfo(dirName) as ICachedDirectoryInfo;
            if (newParent == null)
            {
                throw new NotSupportedException();
            }

            var holder = newParent as IResourceHolder;
            if (holder == null)
            {
                throw new NotSupportedException();
            }

            var libraryDir = holder.Resource as LibraryDirectory;
            if (libraryDir == null)
            {
                throw new NotSupportedException();
            }

            _directory.ParentId = libraryDir.Key;
            _directory.DirectoryName = Path.GetFileName(destinationName);
            Save(null);

            oldParent.Remove(this);
            _parent = newParent;
            newParent.Add(this);

            RefreshLoadedDirectoryResources();
        }

        #endregion

        public override string ToString()
        {
            return FullName;
        }
    }
}