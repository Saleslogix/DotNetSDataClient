using System.IO;

namespace SlxFileBrowser.FileSystem
{
    public abstract class ResourceFileInfo : IFileInfo, IResourceHolder
    {
        protected ResourceFileInfo(IDirectoryInfo directory)
        {
            Directory = directory;
        }

        protected IDirectoryInfo Directory { get; set; }
        protected abstract string FileName { get; }
        protected abstract Stream LoadFile();

        #region IResourceHolder Members

        public abstract object Resource { get; }
        public abstract void Save(Stream stream);

        #endregion

        #region IFileInfo Members

        public Stream Open(FileMode mode, FileAccess access)
        {
            if (access == FileAccess.Read)
            {
                return LoadFile();
            }

            var directory = Directory as ICachedDirectoryInfo;
            if (directory != null)
            {
                directory.Add(this);
            }

            return new ResourceStream(this);
        }

        #endregion

        #region IFileSystemInfo Members

        public string Extension
        {
            get { return Path.GetExtension(FileName); }
        }

        public string FullName
        {
            get { return Path.Combine(Directory.FullName, FileName); }
        }

        public string Name
        {
            get { return FileName; }
        }

        public IDriveInfo DriveInfo
        {
            get { return Directory.DriveInfo; }
        }

        public virtual void Delete()
        {
            var directory = Directory as ICachedDirectoryInfo;
            if (directory != null)
            {
                directory.Remove(this);
            }
        }

        public abstract void Refresh();

        public abstract void MoveTo(string destinationName);

        #endregion

        public override string ToString()
        {
            return FullName;
        }

        private class ResourceStream : MemoryStream
        {
            private readonly IResourceHolder _holder;
            private bool _isSaving;

            public ResourceStream(IResourceHolder holder)
            {
                _holder = holder;
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing && !_isSaving)
                {
                    _isSaving = true;
                    Seek(0, SeekOrigin.Begin);
                    _holder.Save(this);
                }

                base.Dispose(disposing);
            }
        }
    }
}