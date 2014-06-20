// Copyright (c) 1997-2014, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Collections.Generic;
using Saleslogix.SData.Client.Framework;

namespace Saleslogix.SData.Client
{
    public class SDataParameters
    {
        private IDictionary<string, string> _form;
        private IList<AttachedFile> _files;
        private IDictionary<string, string> _extensionArgs;

        public MediaType[] Accept { get; set; }
        public HttpMethod Method { get; set; }
        public object Content { get; set; }
        public MediaType? ContentType { get; set; }
        public string ETag { get; set; }

        public IDictionary<string, string> Form
        {
            get { return _form ?? (_form = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)); }
        }

        public IList<AttachedFile> Files
        {
            get { return _files ?? (_files = new List<AttachedFile>()); }
        }

        public string Path { get; set; }
        public int? StartIndex { get; set; }
        public int? Count { get; set; }
        public string Where { get; set; }
        public string OrderBy { get; set; }
        public string Search { get; set; }
        public string Include { get; set; }
        public int? Precedence { get; set; }
        public string Select { get; set; }
        public bool? IncludeSchema { get; set; }
        public bool? ReturnDelta { get; set; }
        public string TrackingId { get; set; }
        public MediaType? Format { get; set; }
        public string Language { get; set; }
        public string Version { get; set; }

        public IDictionary<string, string> ExtensionArgs
        {
            get { return _extensionArgs ?? (_extensionArgs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)); }
        }
    }
}