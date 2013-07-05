using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Sage.SData.Client.Framework
{
    /// <summary>
    /// The exception that is thrown when an error occurs on an SData server.
    /// </summary>
    [Serializable]
    public class SDataException : WebException
    {
        private readonly Diagnoses _diagnoses;
        private readonly HttpStatusCode? _statusCode;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public SDataException(WebException innerException)
            : base(innerException.Message, innerException, innerException.Status, innerException.Response)
        {
            if (Response == null)
            {
                return;
            }

            var httpResponse = Response as HttpWebResponse;
            _statusCode = httpResponse != null ? httpResponse.StatusCode : (HttpStatusCode?) null;
            MediaType contentType;

            if (MediaTypeNames.TryGetMediaType(Response.ContentType, out contentType) && contentType == MediaType.Xml)
            {
                using (var memoryStream = new MemoryStream())
                {
                    using (var responseStream = Response.GetResponseStream())
                    {
                        responseStream.CopyTo(memoryStream);
                    }

                    memoryStream.Seek(0, SeekOrigin.Begin);
                    _diagnoses = memoryStream.DeserializeXml<Diagnoses>();

                    if (_diagnoses == null)
                    {
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        var diagnosis = memoryStream.DeserializeXml<Diagnosis>();

                        if (diagnosis != null)
                        {
                            _diagnoses = new Diagnoses {diagnosis};
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public SDataException(Diagnoses diagnoses, HttpStatusCode statusCode)
        {
            _diagnoses = diagnoses;
            _statusCode = statusCode;
        }

        protected SDataException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _diagnoses = (Diagnoses) info.GetValue("Diagnoses", typeof (Diagnoses));
            _statusCode = (HttpStatusCode?) info.GetValue("StatusCode", typeof (HttpStatusCode?));
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Diagnoses", _diagnoses);
            info.AddValue("StatusCode", _statusCode);
        }

        /// <summary>
        /// 
        /// </summary>
        public Diagnoses Diagnoses
        {
            get { return _diagnoses; }
        }

        /// <summary>
        /// Gets the HTTP status code associated with the exception.
        /// </summary>
        public HttpStatusCode? StatusCode
        {
            get { return _statusCode; }
        }

        /// <summary>
        /// Gets a message that describes the exception.
        /// </summary>
        public override string Message
        {
            get
            {
                return _diagnoses != null
                           ? string.Join(Environment.NewLine, _diagnoses.Select(diagnosis => diagnosis.Message).ToArray())
                           : base.Message;
            }
        }
    }
}