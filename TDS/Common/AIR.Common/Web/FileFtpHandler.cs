/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.IO;
using System.Net;
using System.Web;
using AIR.Common.Configuration;

namespace AIR.Common.Web
{
    /// <summary>
    /// 
    /// </summary>
    public class FileFtpHandler : IHttpHandler
    {
        // Methods
        public FileFtpHandler()
        {
        }

        public void ProcessRequest(HttpContext context)
        {
            ProcessRequestInternal(context);
        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }

        internal static void ProcessRequestInternal(HttpContext context, string pathOverride)
        {
            HttpResponse httpResponse = context.Response;

            string physicalPath = context.Request.PhysicalPath;

            if (pathOverride != null)
            {
                physicalPath = pathOverride;
            }

            // write out file from ftp to web stream
            WriteFtp(physicalPath, httpResponse.OutputStream);
            httpResponse.ContentType = MimeMapping.GetMapping(physicalPath);

            // set cache headers
            StaticFileHandler2.SetCachingPolicy(context.Response.Cache);
        }

        internal static void ProcessRequestInternal(HttpContext context)
        {
            ProcessRequestInternal(context, null);
        }

        /// <summary>
        /// The serverUri parameter should use the ftp:// scheme.
        /// Example: ftp://air.org/someFile.txt.
        /// </summary>
        public static bool AllowScheme(string filePath)
        {
            if (AppSettingsHelper.GetBoolean("Debug.AllowFTP"))
            {
                // check if file path is ftp scheme
                return (filePath != null && filePath.StartsWith(@"ftp://"));
            }

            return false;
        }

        public static Stream OpenStream(Uri requestUri)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(requestUri);
            //request.Credentials = new NetworkCredential(username, password, domain);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.UseBinary = true;

            FtpWebResponse response = (FtpWebResponse) request.GetResponse();
            return response.GetResponseStream();
        }

        public static Stream OpenStream(string ftpFilePath)
        {
            ftpFilePath = ftpFilePath.Replace("\\", "/");
            Uri requestUri = new Uri(ftpFilePath);
            return OpenStream(requestUri);
        }

        /// <summary>
        /// Check if the file exists on the ftp server.
        /// </summary>
        public static bool Exists(Uri requestUri)
        {
            // http://stackoverflow.com/questions/347897/how-to-check-if-file-exists-on-ftp-before-ftpwebrequest

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(requestUri);
            request.Method = WebRequestMethods.Ftp.GetFileSize;

            try
            {
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    return false;
                }
            }

            return true;
        }

        public static void WriteFtp(string ftpFilePath, Stream outputStream)
        {
            Stream inputStream = OpenStream(ftpFilePath);

            using (inputStream)
            {
                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[bufferSize];

                readCount = inputStream.Read(buffer, 0, bufferSize);
                
                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);
                    readCount = inputStream.Read(buffer, 0, bufferSize);
                }

                inputStream.Close();
            }
        }

    }
}
