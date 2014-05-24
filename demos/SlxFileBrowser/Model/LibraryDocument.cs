using System;
using System.ComponentModel;
using Saleslogix.SData.Client;

namespace SlxFileBrowser.Model
{
    [SDataPath("libraryDocuments")]
    public class LibraryDocument
    {
        [SDataProtocolProperty]
        [Category("System")]
        public string Key { get; set; }

        [SDataProtocolProperty]
        [Category("System")]
        public string ETag { get; set; }

        public string Abstract { get; set; }
        public string Description { get; set; }
        public DateTime? ExpireDate { get; set; }
        public bool? Expires { get; set; }
        public string FileName { get; set; }
        public bool FileExists { get; set; }
        public int? FileSize { get; set; }
        public int? Flags { get; set; }
        public bool? Found { get; set; }
        public DateTime? RevisionDate { get; set; }
        public string Status { get; set; }
        public SDataResource Directory { get; set; }

        [Category("System")]
        public DateTime? CreateDate { get; set; }

        [Category("System")]
        public string CreateUser { get; set; }

        [Category("System")]
        public DateTime? ModifyDate { get; set; }

        [Category("System")]
        public string ModifyUser { get; set; }
    }
}