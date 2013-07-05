using System;
using System.Runtime.Serialization;

namespace Sage.SData.Client
{
    [Serializable]
    public class SDataClientException : Exception
    {
        public SDataClientException()
        {
        }

        public SDataClientException(string message)
            : base(message)
        {
        }

        public SDataClientException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected SDataClientException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}