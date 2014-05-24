using System;
using System.IO;
using System.Linq;
using Saleslogix.SData.Client;
using Saleslogix.SData.Client.Framework;
using SlxFileBrowser.Model;

namespace SlxFileBrowser.FileSystem
{
    public class AttachmentFileInfo : ResourceFileInfo
    {
        private readonly ISDataClient _client;
        private readonly bool _formMode;
        private Attachment _attachment;

        public AttachmentFileInfo(ISDataClient client, bool formMode, IDirectoryInfo directory, Attachment attachment)
            : base(directory)
        {
            _client = client;
            _formMode = formMode;
            _attachment = attachment;
        }

        protected override string FileName
        {
            get { return _attachment.FileName; }
        }

        protected override Stream LoadFile()
        {
            if (string.IsNullOrEmpty(_attachment.Key))
            {
                return Stream.Null;
            }

            if (_formMode)
            {
                var results = _client.Execute<byte[]>(new SDataParameters
                    {
                        Path = "attachments(" + SDataUri.FormatConstant(_attachment.Key) + ")/file"
                    });
                return results.Content != null ? new MemoryStream(results.Content) : Stream.Null;
            }
            else
            {
                var results = _client.Execute(new SDataParameters
                    {
                        Path = "attachments(" + SDataUri.FormatConstant(_attachment.Key) + ")",
                        Precedence = 0,
                        ExtensionArgs = {{"includeFile", "true"}}
                    });
                return results.Files.Count == 1 ? results.Files[0].Stream : Stream.Null;
            }
        }

        public override object Resource
        {
            get { return _attachment; }
        }

        public override void Save(Stream stream)
        {
            if (stream == null)
            {
                _attachment = string.IsNullOrEmpty(_attachment.Key)
                    ? _client.Post(_attachment)
                    : _client.Put(_attachment);
                return;
            }

            var parms = new SDataParameters
                {
                    Files = {new AttachedFile(null, _attachment.FileName, stream)}
                };

            if (string.IsNullOrEmpty(_attachment.Key))
            {
                parms.Method = HttpMethod.Post;
                parms.Path = "attachments";
            }
            else
            {
                parms.Method = HttpMethod.Put;
                parms.Path = "attachments(" + SDataUri.FormatConstant(_attachment.Key) + ")";
                parms.ETag = _attachment.ETag;
            }

            if (_formMode)
            {
                foreach (var prop in typeof (Attachment).GetProperties())
                {
                    var name = _client.NamingScheme.GetName(prop);
                    if (!name.StartsWith("$") && !new[] {"physicalFileName", "fileExists", "fileSize", "createDate", "createUser", "modifyDate", "modifyUser"}.Contains(name))
                    {
                        var value = prop.GetValue(_attachment, null);
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
                    _attachment.Key = selector.Substring(1, selector.Length - 2);
                    _attachment.ETag = results.ETag;
                }
            }
            else
            {
                parms.Content = _attachment;
                _attachment = _client.Execute<Attachment>(parms).Content;
            }
        }

        public override void Delete()
        {
            if (!string.IsNullOrEmpty(_attachment.Key))
            {
                _client.Delete(_attachment);
            }

            base.Delete();
        }

        public override void Refresh()
        {
            _attachment = _client.Get<Attachment>(_attachment.Key);
        }

        public override void MoveTo(string destinationName)
        {
            var destinationDir = Path.GetDirectoryName(destinationName);

            if (string.Equals(Directory.FullName, destinationDir, StringComparison.OrdinalIgnoreCase))
            {
                _attachment.FileName = Path.GetFileName(destinationName);
                Save(null);
            }
            else
            {
                throw new NotSupportedException();
            }
        }
    }
}