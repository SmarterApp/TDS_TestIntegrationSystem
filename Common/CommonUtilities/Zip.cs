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
using System.Linq;
using System.Text;
//using ICSharpCode.SharpZipLib.Zip;
using System.IO;
//using ICSharpCode.SharpZipLib.Checksums;

namespace CommonUtilities
{
    public static class Zip
    {
        public static string ZipFile(string outputFileName, string inputFileName)
        {
            // uses best compression value, since this method would probably only be used to shrink file size
            return ZipFileList(outputFileName, new List<FileInfo>() { new FileInfo(inputFileName) }, 9);
        }

        public static string ZipDirectory(string outputFileName, string folderName)
        {
            // this was default from previous ZipDirectory method, so keeping it.  Use overload to change it.
            return ZipDirectory(outputFileName, folderName, 9);
        }

        public static string ZipDirectory(string outputFileName, string folderName, int compressionLevel)
        {
            return ZipFileList(outputFileName, new DirectoryInfo(folderName).EnumerateFiles(), compressionLevel);
        }

        public static string ZipFileList(string outputFileName, List<string> fileNames)
        {
            // best compression is 9, but it is very slow; using 3 as default
            return ZipFileList(outputFileName, fileNames, 3);
        }

        public static string ZipFileList(string outputFileName, List<string> fileNames, int compressionLevel)
        {
            return ZipFileList(outputFileName, fileNames.Select(f => new FileInfo(f)).ToList<FileInfo>(), compressionLevel);
        }

        public static string ZipFileList(string outputFileName, IEnumerable<FileInfo> files, int compressionLevel)
        {
            /*Crc32 crc = new Crc32();

            using (ZipOutputStream zipStream = new ZipOutputStream(new FileStream(outputFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None)))
            {
                zipStream.SetLevel(compressionLevel);

                foreach (FileInfo file in files)
                {
                    ZipEntry entry = new ZipEntry(file.Name);
                    entry.DateTime = file.LastWriteTime;
                    //entry.Comment = "Item Response file"; // we use this for more than item response files.  Not sure where this appears though.
                    entry.ZipFileIndex = 1;
                    entry.Size = file.Length;
                    zipStream.PutNextEntry(entry);
                    crc.Reset();

                    byte[] buffer = new byte[4096 * 5];
                    int byteCount = 0;
                    using (BufferedStream buffstream = new BufferedStream(file.OpenRead()))
                    {
                        byteCount = buffstream.Read(buffer, 0, buffer.Length);
                        while (byteCount > 0)
                        {
                            zipStream.Write(buffer, 0, byteCount);
                            crc.Update(buffer, 0, byteCount);
                            byteCount = buffstream.Read(buffer, 0, buffer.Length);
                        }
                    }
                    // note: this might cause problems if used on a MAC OS. This might have to be set before
                    //       writing to the zipStream.
                    entry.Crc = crc.Value;

                    zipStream.Flush();
                    //zipStream.CloseEntry();
                }
                zipStream.Finish();
            }*/
            return outputFileName;
              
        }

        public static void Unzip(string pathToZipFile, string targetDirectory)
        {
            //FastZip fz = new FastZip();
            //fz.ExtractZip(pathToZipFile, targetDirectory, null);
        }
    }
}
