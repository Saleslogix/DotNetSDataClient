using System;
using System.IO;
using System.Linq;
using Saleslogix.SData.Client;
using SlxFileBrowser.Model;

namespace SlxFileBrowser.FileSystem
{
    public class SDataDriveInfo : IDriveInfo
    {
        private readonly ISDataClient _client;
        private readonly bool _formMode;
        private readonly IDirectoryInfo _directory;

        public SDataDriveInfo(ISDataClient client, bool formMode)
        {
            _client = client;
            _formMode = formMode;
            _directory = new SDataDirectoryInfo(_client, _formMode, this);
        }

        #region IDriveInfo Members

        public string Name
        {
            get { return "sdata:\\"; }
        }

        public IDirectoryInfo RootDirectory
        {
            get { return _directory; }
        }

        public IFileInfo GetFileInfo(string path)
        {
            var dirPath = Path.GetDirectoryName(path);
            var fileName = Path.GetFileName(path);
            var dir = GetDirectoryInfo(dirPath);
            var file = dir.GetFiles().FirstOrDefault(item => string.Equals(fileName, item.Name, StringComparison.OrdinalIgnoreCase));

            if (file == null)
            {
                if (path.StartsWith("sdata:\\attachments", StringComparison.OrdinalIgnoreCase))
                {
                    var attachment = new Attachment {FileName = fileName};
                    file = new AttachmentFileInfo(_client, _formMode, dir, attachment);
                }
                else
                {
                    var document = new LibraryDocument
                        {
                            FileName = fileName,
                            Directory = new SDataResource {Key = ((LibraryDirectory) ((IResourceHolder) dir).Resource).Key}
                        };
                    file = new LibraryFileInfo(_client, _formMode, dir, document);
                }
            }

            return file;
        }

        public IDirectoryInfo GetDirectoryInfo(string path)
        {
            var parts = path.Split('\\');
            IDirectoryInfo dir = null;

            foreach (var part in parts)
            {
                if (dir == null)
                {
                    if (string.Equals(part, "sdata:", StringComparison.OrdinalIgnoreCase))
                    {
                        dir = _directory;
                    }
                }
                else
                {
                    dir = dir.GetDirectories().FirstOrDefault(item => string.Equals(part, item.Name, StringComparison.OrdinalIgnoreCase));
                }

                if (dir == null)
                {
                    throw new NotSupportedException();
                }
            }

            if (dir == null)
            {
                throw new NotSupportedException();
            }

            return dir;
        }

        #endregion
    }
}