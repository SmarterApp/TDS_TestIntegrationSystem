/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace AIR.Common.Diagnostics
{
    public class StreamWriterCache
    {
        private readonly object _padlock = new object();
        private static readonly Dictionary<string, CachedStream> _fileStreams = new Dictionary<string, CachedStream>();

        private static string NormalizePath(string path)
        {
            if (Directory.Exists(path))
            {
                return path;
            }

            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
        }

        public StreamWriter GetStreamWriter(string path)
        {
            CachedStream cs = null;

            if (!_fileStreams.TryGetValue(path, out cs))
            {
                lock (_padlock)
                {
                    if (!_fileStreams.TryGetValue(path, out cs))
                    {
                        string actualPath = NormalizePath(path);

                        // TODO: need to make the filestream pluggable for testing purposes
                        FileStream fs = new FileStream(actualPath, FileMode.Append, FileAccess.Write, FileShare.Read);
                        cs = new CachedStream();
                        cs.StreamWriter = new StreamWriter(fs);
                        
                        _fileStreams.Add(path, cs);
                    }
                }
            }

            cs.LastAccessTime = DateTime.Now;
            return cs.StreamWriter;
        }

        /// <summary>
        /// Searches the cache of StreamWriters and disposes and removes any that haven't been used accessed since
        /// the notUsedSince parameter.
        /// </summary>
        /// <param name="notUsedSince">The time </param>
        public void ClearOldStreams(DateTime notUsedSince)
        {
            Queue<string> _removals = new Queue<string>();

            lock (_padlock)
            {
                foreach (KeyValuePair<string, CachedStream> kvp in _fileStreams)
                {
                    if (kvp.Value.LastAccessTime <= notUsedSince)
                    {
                        kvp.Value.StreamWriter.Dispose();
                        // can't remove items whilst enumerating so remember them for later
                        _removals.Enqueue(kvp.Key);
                    }
                }

                while (_removals.Count > 0)
                {
                    _fileStreams.Remove(_removals.Dequeue());
                }
            }
        }

        private class CachedStream
        {
            internal CachedStream() { }

            private DateTime lastAccessTime;
            public StreamWriter streamWriter;

            /// <summary>
            /// The last time an item was accessed
            /// </summary>
            public DateTime LastAccessTime
            {
                get { return lastAccessTime; }
                set { lastAccessTime = value; }
            }

            /// <summary>
            /// The cached StreamWriter
            /// </summary>
            public StreamWriter StreamWriter
            {
                get { return streamWriter; }
                set { streamWriter = value; }
            }

        }
    }
}
