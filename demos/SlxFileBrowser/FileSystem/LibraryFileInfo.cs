using System;
using System.IO;
using System.Linq;
using Saleslogix.SData.Client;
using Saleslogix.SData.Client.Framework;
using SlxFileBrowser.Model;

namespace SlxFileBrowser.FileSystem
{
    public class LibraryFileInfo : ResourceFileInfo
    {
        private readonly ISDataClient _client;
        private readonly bool _formMode;
        private LibraryDocument _document;

        public LibraryFileInfo(ISDataClient client, bool formMode, IDirectoryInfo directory, LibraryDocument document)
            : base(directory)
        {
            _client = client;
            _formMode = formMode;
            _document = document;
        }

        protected override string FileName
        {
            get { return _document.FileName; }
        }

        protected override Stream LoadFile()
        {
            if (string.IsNullOrEmpty(_document.Key))
            {
                return Stream.Null;
            }

            if (_formMode)
            {
                var results = _client.Execute<byte[]>(new SDataParameters
                    {
                        Path = "libraryDocuments(" + SDataUri.FormatConstant(_document.Key) + ")/file"
                    });
                return results.Content != null ? new MemoryStream(results.Content) : Stream.Null;
            }
            else
            {
                var results = _client.Execute(new SDataParameters
                    {
                        Path = "libraryDocuments(" + SDataUri.FormatConstant(_document.Key) + ")",
                        Precedence = 0,
                        ExtensionArgs = {{"includeFile", "true"}}
                    });
                return results.Files.Count == 1 ? results.Files[0].Stream : Stream.Null;
            }
        }

        public override object Resource
        {
            get { return _document; }
        }

        public override void Save(Stream stream)
        {
            if (stream == null)
            {
                _document = string.IsNullOrEmpty(_document.Key)
                    ? _client.Post(_document)
                    : _client.Put(_document);
                return;
            }

            var parms = new SDataParameters
                {
                    Files = {new AttachedFile(null, _document.FileName, stream)}
                };

            if (string.IsNullOrEmpty(_document.Key))
            {
                parms.Method = HttpMethod.Post;
                parms.Path = "libraryDocuments";
            }
            else
            {
                parms.Method = HttpMethod.Put;
                parms.Path = "libraryDocuments(" + SDataUri.FormatConstant(_document.Key) + ")";
                parms.ETag = _document.ETag;
            }

            if (_formMode)
            {
                foreach (var prop in typeof (LibraryDocument).GetProperties())
                {
                    var name = _client.NamingScheme.GetName(prop);
                    if (!name.StartsWith("$") && !new[] {"createDate", "createUser", "modifyDate", "modifyUser"}.Contains(name))
                    {
                        var value = prop.GetValue(_document, null);
                        if (value != null)
                        {
                            parms.Form[name] = value.ToString();
                        }
                    }
                }

                parms.Path += "/file";
                var results = _client.Execute(parms);
                if (!string.IsNullOrEmpty(results.Location))
                {
                    var selector = new SDataUri(results.Location).GetPathSegment(4).Selector;
                    _document.Key = selector.Substring(1, selector.Length - 2);
                    _document.ETag = results.ETag;
                }
            }
            else
            {
                parms.Content = _document;
                _document = _client.Execute<LibraryDocument>(parms).Content;
            }
        }

        public override void Delete()
        {
            if (!string.IsNullOrEmpty(_document.Key))
            {
                _client.Delete(_document);
            }

            base.Delete();
        }

        public override void Refresh()
        {
            _document = _client.Get<LibraryDocument>(_document.Key);
        }

        public override void MoveTo(string destinationName)
        {
            var oldDirectory = Directory as ICachedDirectoryInfo;
            if (oldDirectory == null)
            {
                throw new NotSupportedException();
            }

            var dirName = Path.GetDirectoryName(destinationName);
            var newDirectory = DriveInfo.GetDirectoryInfo(dirName) as ICachedDirectoryInfo;
            if (newDirectory == null)
            {
                throw new NotSupportedException();
            }

            var holder = newDirectory as IResourceHolder;
            if (holder == null)
            {
                throw new NotSupportedException();
            }

            var libraryDir = holder.Resource as LibraryDirectory;
            if (libraryDir == null)
            {
                throw new NotSupportedException();
            }

            _document.Directory.Key = libraryDir.Key;
            _document.FileName = Path.GetFileName(destinationName);
            Save(null);

            oldDirectory.Remove(this);
            Directory = newDirectory;
            newDirectory.Add(this);
        }
    }
}