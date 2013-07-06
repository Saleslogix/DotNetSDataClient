using System;
using System.IO;
using System.Net.Mime;
using Sage.SData.Client.Mime;

namespace Sage.SData.Client.Framework
{
    /// <summary>
    /// Represents a file that's been attached to a request or response.
    /// </summary>
    public class AttachedFile
    {
        private readonly string _contentType;
        private readonly string _fileName;
        private readonly Stream _stream;

        /// <summary>
        /// Initializes a new instance of the <see cref="AttachedFile"/> class.
        /// </summary>
        /// <param name="part">A multipart MIME part containing the attached file.</param>
        public AttachedFile(MimePart part)
            : this(part.ContentType, GetFileName(part.ContentDisposition), part.Content)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttachedFile"/> class.
        /// </summary>
        /// <param name="contentType">The MIME content type of an attached file.</param>
        /// <param name="fileName">The file name of an attached file.</param>
        /// <param name="stream">The <see cref="Stream"/> object that points to the content of an attached file.</param>
        public AttachedFile(string contentType, string fileName, Stream stream)
        {
            _contentType = contentType;
            _fileName = fileName;
            _stream = stream;
        }

        private static string GetFileName(ContentDisposition contentDisposition)
        {
            if (contentDisposition.FileName != null)
            {
                return contentDisposition.FileName;
            }

            var fileName = contentDisposition.Parameters["filename*"];
            if (fileName == null)
            {
                return null;
            }

            var pos = fileName.IndexOf("''", StringComparison.Ordinal);
            if (pos >= 0)
            {
                fileName = fileName.Substring(pos + 2);
            }

            return Uri.UnescapeDataString(fileName);
        }

        /// <summary>
        /// Gets the MIME content type of an attached file.
        /// </summary>
        public string ContentType
        {
            get { return _contentType; }
        }

        /// <summary>
        /// Gets the file name of an attached file.
        /// </summary>
        public string FileName
        {
            get { return _fileName; }
        }

        /// <summary>
        /// Gets the <see cref="Stream"/> object that points to the content of an attached file.
        /// </summary>
        public Stream Stream
        {
            get { return _stream; }
        }
    }
}