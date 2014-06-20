// Copyright (c) 1997-2014, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Linq;
using System.Net;
using Saleslogix.SData.Client.Content;
using Saleslogix.SData.Client.Utilities;

#if !PCL && !NETFX_CORE && !SILVERLIGHT
using System.Runtime.Serialization;
using System.Security.Permissions;
#endif

namespace Saleslogix.SData.Client.Framework
{
    /// <summary>
    /// The exception that is thrown when an error occurs on an SData server.
    /// </summary>
    [Serializable]
    public class SDataException : WebException
    {
        private readonly object _content;
        private readonly Diagnoses _diagnoses;
        private readonly HttpStatusCode? _statusCode;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public SDataException(WebException innerException)
            : base(GetMessage(innerException), innerException, innerException.Status, innerException.Response)
        {
            if (Response == null)
            {
                return;
            }

            var httpResponse = Response as HttpWebResponse;
            _statusCode = httpResponse != null ? httpResponse.StatusCode : (HttpStatusCode?) null;
            MediaType contentType;

            if (MediaTypeNames.TryGetMediaType(Response.ContentType, out contentType))
            {
                var handler = ContentManager.GetHandler(contentType);
                if (handler != null)
                {
                    using (var responseStream = Response.GetResponseStream())
                    {
                        _content = handler.ReadFrom(responseStream);
                    }

                    if (ContentHelper.IsDictionary(_content))
                    {
                        var diagnosis = ContentHelper.Deserialize<Diagnosis>(_content);
                        if (diagnosis != null)
                        {
                            _diagnoses = new Diagnoses {diagnosis};
                        }
                    }
                    else if (ContentHelper.IsCollection(_content))
                    {
                        var diagnoses = ContentHelper.Deserialize<Diagnoses>(_content);
                        if (diagnoses != null)
                        {
                            _diagnoses = diagnoses;
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

#if !PCL && !NETFX_CORE && !SILVERLIGHT
        protected SDataException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _content = info.GetValue("Content", typeof (object));
            _diagnoses = (Diagnoses) info.GetValue("Diagnoses", typeof (Diagnoses));
            _statusCode = (HttpStatusCode?) info.GetValue("StatusCode", typeof (HttpStatusCode?));
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Content", _content);
            info.AddValue("Diagnoses", _diagnoses);
            info.AddValue("StatusCode", _statusCode);
        }
#endif

        public object Content
        {
            get { return _content; }
        }

        /// <summary>
        /// Gets the collection of diagnoses responsible for the exception.
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
                var message = _diagnoses != null
                    ? string.Join(Environment.NewLine, _diagnoses.Select(diagnosis => diagnosis.Message).ToArray())
                    : null;
                return !string.IsNullOrEmpty(message) ? message : base.Message;
            }
        }

        private static string GetMessage(Exception exception)
        {
            Guard.ArgumentNotNull(exception, "exception");
            return exception.Message;
        }
    }
}