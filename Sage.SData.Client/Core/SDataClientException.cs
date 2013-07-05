using System;

namespace Sage.SData.Client.Core
{
    /// <summary>
    /// Exception for SDataClient
    /// </summary>
    [Serializable]
    public class SDataClientException : Exception
    {
        // Base Exception class constructors.
        /// <summary>
        /// constructor
        /// </summary>
        public SDataClientException()
        {
        }

        /// <summary>
        /// constructor
        /// </summary>
        public SDataClientException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// constructor
        /// </summary>
        public SDataClientException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}