using System;
using System.ComponentModel;
using Saleslogix.SData.Client;

namespace SlxFileBrowser.Model
{
    [SDataPath("libraryDirectories")]
    public class LibraryDirectory
    {
        [SDataProtocolProperty]
        [Category("System")]
        public string Key { get; set; }

        [SDataProtocolProperty]
        [Category("System")]
        public string ETag { get; set; }

        public string DirectoryName { get; set; }
        public bool? Found { get; set; }
        public string FullPath { get; set; }
        public string ParentId { get; set; }

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