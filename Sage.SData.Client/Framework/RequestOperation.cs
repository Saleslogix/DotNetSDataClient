// Copyright (c) Sage (UK) Limited 2010. All rights reserved.
// This code may not be copied or used, except as set out in a written licence agreement
// between the user and Sage (UK) Limited, which specifically permits the user to use this
// code. Please contact Sage (UK) if you do not have such a licence. Sage will take
// appropriate legal action against those who make unauthorised use of this code.

using System;
using System.Collections.Generic;

namespace Sage.SData.Client.Framework
{
    /// <summary>
    /// Defines an operation to perform during a request.
    /// </summary>
    public class RequestOperation
    {
        private IDictionary<string, string> _form;
        private IList<AttachedFile> _files;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestOperation"/> class.
        /// </summary>
        public RequestOperation()
            : this(HttpMethod.Get)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestOperation"/> class with
        /// the specified method and content.
        /// </summary>
        /// <param name="method">One of the <see cref="HttpMethod"/> values</param>
        /// <param name="content">The input content involved in the operation.</param>
        public RequestOperation(HttpMethod method, object content = null)
        {
            Method = method;
            Content = content;
        }

        #region Properties

        /// <summary>
        /// Gets or sets the method for the request.
        /// </summary>
        public HttpMethod Method { get; set; }

        /// <summary>
        /// Gets or sets the selector for the request.
        /// </summary>
        public string Selector { get; set; }

        /// <summary>
        /// Gets or sets the input resource for the request.
        /// </summary>
        public object Content { get; set; }

        /// <summary>
        /// Gets or sets the input content type for the request.
        /// </summary>
        public MediaType? ContentType { get; set; }

        /// <summary>
        /// Gets or sets the ETag value for the request.
        /// </summary>
        public string ETag { get; set; }

        /// <summary>
        /// Gets the form data associated with the request.
        /// </summary>
        public IDictionary<string, string> Form
        {
            get { return _form ?? (_form = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)); }
        }

        /// <summary>
        /// Gets the files that will be attached to the request content.
        /// </summary>
        public IList<AttachedFile> Files
        {
            get { return _files ?? (_files = new List<AttachedFile>()); }
        }

        #endregion
    }
}