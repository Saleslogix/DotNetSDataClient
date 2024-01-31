﻿// Copyright (c) 1997-2014, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.Win32;

namespace Saleslogix.SData.Client.Mime
{
    /// <summary>
    /// Helper class for inferring file MIME types.
    /// </summary>
    public static class MimeHelper
    {
        private const string DefaultType = "application/octet-stream";
        private static IDictionary<string, string> _knownTypes;

        /// <summary>
        /// Find the MIME type of a local file using its file extension and the first few kilobytes of its content.
        /// </summary>
        /// <param name="filePath">The local file path.</param>
        #if !NETCOREAPP
        [EnvironmentPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        #endif
        public static string FindMimeType(string filePath)
        {
            try
            {
                var extension = Path.GetExtension(filePath);
                string mimeType;

                if (TryFindByExtension(extension, out mimeType) ||
                    TryFindByContent(filePath, out mimeType))
                {
                    return mimeType;
                }
            }
            catch
            {
            }

            return DefaultType;
        }

        /// <summary>
        /// Find the MIME type of a sample of raw file data.
        /// </summary>
        /// <param name="data">The raw file data.</param>
        #if !NETCOREAPP
        [EnvironmentPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        #endif
        public static string FindMimeType(byte[] data)
        {
            try
            {
                string mimeType;

                if (TryFindByData(data, out mimeType))
                {
                    return mimeType;
                }
            }
            catch
            {
            }

            return DefaultType;
        }

        private static IDictionary<string, string> KnownTypes
        {
            get
            {
                if (_knownTypes == null)
                {
                    _knownTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                    using (var classesRoot = Registry.ClassesRoot)
                    using (var typeKey = classesRoot.OpenSubKey(@"MIME\Database\Content Type"))
                    {
                        if (typeKey == null)
                        {
                            return _knownTypes;
                        }

                        foreach (var contentType in typeKey.GetSubKeyNames().Where(name => !string.IsNullOrEmpty(name)))
                        {
                            using (var contentTypeKey = typeKey.OpenSubKey(contentType))
                            {
                                if (contentTypeKey == null)
                                {
                                    continue;
                                }

                                var extension = contentTypeKey.GetValue("Extension") as string;
                                if (!string.IsNullOrEmpty(extension))
                                {
                                    _knownTypes[extension] = contentType;
                                }
                            }
                        }
                    }
                }

                return _knownTypes;
            }
        }

        private static bool TryFindByExtension(string extension, out string mimeType)
        {
            if (string.IsNullOrEmpty(extension))
            {
                mimeType = null;
                return false;
            }

            return KnownTypes.TryGetValue(extension, out mimeType);
        }

        #if !NETCOREAPP
        [EnvironmentPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        #endif
        private static bool TryFindByContent(string filePath, out string mimeType)
        {
            var file = new FileInfo(filePath);

            if (!file.Exists || file.Length == 0)
            {
                mimeType = null;
                return false;
            }

            var data = new byte[Math.Min(0x1000, file.Length)];

            using (var stream = file.OpenRead())
            {
                stream.Read(data, 0, data.Length);
            }

            if (!TryFindByData(data, out mimeType))
            {
                mimeType = null;
                return false;
            }

            var extension = Path.GetExtension(filePath);

            if (!string.IsNullOrEmpty(extension))
            {
                KnownTypes[extension] = mimeType;
            }

            return true;
        }

        #if !NETCOREAPP
        [EnvironmentPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        #endif
        private static bool TryFindByData(byte[] data, out string mimeType)
        {
            IntPtr outPtr;
            var result = NativeMethods.FindMimeFromData(IntPtr.Zero,
                                                        null,
                                                        data,
                                                        data.Length,
                                                        null,
                                                        0,
                                                        out outPtr,
                                                        0);

            if (result != 0 || outPtr == IntPtr.Zero)
            {
                mimeType = null;
                return false;
            }

            mimeType = Marshal.PtrToStringUni(outPtr);
            Marshal.FreeCoTaskMem(outPtr);
            return true;
        }

        #region Nested type: NativeMethods

        private static class NativeMethods
        {
            [DllImport("urlmon.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
            public static extern int FindMimeFromData(IntPtr pBC,
                                                      [MarshalAs(UnmanagedType.LPWStr)] string pwzUrl,
                                                      [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I1, SizeParamIndex = 3)] byte[] pBuffer,
                                                      int cbSize,
                                                      [MarshalAs(UnmanagedType.LPWStr)] string pwzMimeProposed,
                                                      int dwMimeFlags,
                                                      out IntPtr ppwzMimeOut,
                                                      int dwReserved);
        }

        #endregion
    }
}