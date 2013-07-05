using System;
using System.Collections.ObjectModel;
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
        private readonly Collection<Diagnosis> _diagnoses;
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
            MediaType mediaType;

            if (MediaTypeNames.TryGetMediaType(Response.ContentType, out mediaType) && mediaType == MediaType.Xml)
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
                            _diagnoses = new Collection<Diagnosis> {diagnosis};
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public SDataException(Collection<Diagnosis> diagnoses, HttpStatusCode statusCode)
        {
            _diagnoses = diagnoses;
            _statusCode = statusCode;
        }

        protected SDataException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _diagnoses = (Collection<Diagnosis>) info.GetValue("Diagnoses", typeof (Collection<Diagnosis>));
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
        /// Gets the high level diagnostic information returned from the server.
        /// </summary>
        [Obsolete("Use the Diagnoses property instead.")]
        public Diagnosis Diagnosis
        {
            get { return _diagnoses != null && _diagnoses.Count > 0 ? _diagnoses[0] : null; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Collection<Diagnosis> Diagnoses
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