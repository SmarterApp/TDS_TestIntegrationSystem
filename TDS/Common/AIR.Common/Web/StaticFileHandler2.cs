/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Web;
using System.Web.UI.MobileControls.Adapters;

namespace AIR.Common.Web
{
    /// <summary>
    /// This is a clone of the internal StaticFileHandler except you can override the file path
    /// </summary>
    internal class StaticFileHandler2 : IHttpHandler
    {
        // Fields
        private const int DEFAULT_CACHE_THRESHOLD = 262144;
        private const int ERROR_ACCESS_DENIED = 5;

        // Methods
        public StaticFileHandler2()
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

        private static void BuildFileItemResponse(HttpContext context, string fileName, long fileSize, DateTime lastModifiedTime, string strETag)
        {
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;
            bool readIntoMemory = false;
            int num = 262144;
            bool flag2 = false;
            string str = request.Headers["Range"];

            if ((str != null) && str.StartsWith("bytes"))
            {
                flag2 = true;
            }
            
            if (flag2)
            {
                SendEntireEntity(context, strETag, lastModifiedTime);
            }
            
            if (((fileSize <= num) && !request.RequestType.Equals("(GETSOURCE)")) && !request.RequestType.Equals("(HEADSOURCE)"))
            {
                readIntoMemory = true;
            }
            
            if (readIntoMemory)
            {
                response.WriteFile(fileName, readIntoMemory);
            }
            else
            {
                response.TransmitFile(fileName);
            }
            
            response.ContentType = MimeMapping.GetMapping(fileName);
            response.AppendHeader("Accept-Ranges", "bytes");
            
            if (readIntoMemory)
            {
                response.Cache.AddValidationCallback(new HttpCacheValidateHandler(StaticFileHandler2.CacheValidateHandler), null);
                response.AddFileDependency(fileName);
                //response.Cache.SetExpires(DateTime.Now.AddDays(1));
            }
        }

        private static void CacheValidateHandler(HttpContext context, object data, ref HttpValidationStatus validationStatus)
        {
            if (((context.Request.Headers["Range"] != null) || context.Request.RequestType.Equals("(GETSOURCE)")) || context.Request.RequestType.Equals("(HEADSOURCE)"))
            {
                validationStatus = HttpValidationStatus.IgnoreThisRequest;
            }
        }

        private static bool CompareETags(string strETag1, string strETag2)
        {
            if (strETag1.Equals("*") || strETag2.Equals("*"))
            {
                return true;
            }

            if (strETag1.StartsWith("W/"))
            {
                strETag1 = strETag1.Substring(2);
            }
            
            if (strETag2.StartsWith("W/"))
            {
                strETag2 = strETag2.Substring(2);
            }
            
            return strETag2.Equals(strETag1);
        }

        internal static string GenerateETag(HttpContext context, DateTime lastModTime)
        {
            StringBuilder builder = new StringBuilder();
            long num = DateTime.Now.ToFileTime();
            long num2 = lastModTime.ToFileTime();
            builder.Append("\"");
            builder.Append(num2.ToString("X8", CultureInfo.InvariantCulture));
            builder.Append(":");
            builder.Append(num.ToString("X8", CultureInfo.InvariantCulture));
            builder.Append("\"");
            
            if ((DateTime.Now.ToFileTime() - num2) <= 30000000L)
            {
                return ("W/" + builder.ToString());
            }
            
            return builder.ToString();
        }

        internal static bool IsSecurityError(int ErrorCode)
        {
            return (ErrorCode == 5);
        }

        internal static void ProcessRequestInternal(HttpContext context)
        {
            ProcessRequestInternal(context, null);
        }

        internal static void ProcessRequestInternal(HttpContext context, string pathOverride)
        {
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;
            
            string physicalPath = context.Request.PhysicalPath;
            string fileName = Path.GetFileName(physicalPath);

            if (pathOverride != null)
            {
                physicalPath = pathOverride;
            }

            FileInfo info;
            if (!FileExists(physicalPath))
            {
                string error = "File does not exist";
                if (context.IsDebuggingEnabled) error += String.Format(": {0}", physicalPath);
                throw new HttpException(404, error);
            }
            
            try
            {
                info = new FileInfo(physicalPath);
            }
            catch (IOException exception)
            {
                throw new HttpException(404, "Error trying to enumerate files", exception);
            }
            catch (SecurityException exception2)
            {
                throw new HttpException(401, "File enumerator access denied", exception2);
            }

            if (info.Length == 0)
            {
                throw new HttpException(404, "File is empty");
            }

            if ((info.Attributes & FileAttributes.Hidden) != 0)
            {
                throw new HttpException(404, "File is hidden");
            }
            
            if (physicalPath[physicalPath.Length - 1] == '.')
            {
                throw new HttpException(404, "File does not exist");
            }
            
            if ((info.Attributes & FileAttributes.Directory) != 0)
            {
                if (request.Path.EndsWith("/"))
                {
                    throw new HttpException(403, SR.GetString("Missing star mapping"));
                }
                response.Redirect(request.Path + "/");
            }
            else
            {
                DateTime lastModTime = new DateTime(info.LastWriteTime.Year, info.LastWriteTime.Month, info.LastWriteTime.Day, info.LastWriteTime.Hour, info.LastWriteTime.Minute, info.LastWriteTime.Second, 0);
                
                if (lastModTime > DateTime.Now)
                {
                    lastModTime = DateTime.Now;
                }
                
                string strETag = GenerateETag(context, lastModTime);
                
                try
                {
                    BuildFileItemResponse(context, physicalPath, info.Length, lastModTime, strETag);
                }
                catch (Exception exception3)
                {
                    if ((exception3 is ExternalException) && IsSecurityError(((ExternalException) exception3).ErrorCode))
                    {
                        throw new HttpException(401, "Resource access forbidden");
                    }
                }

                // set cache headers
                HttpCachePolicy cachePolicy = context.Response.Cache;
                SetCachingPolicy(cachePolicy);
                
                // set cache file info
                cachePolicy.SetETag(strETag);
                cachePolicy.SetLastModified(lastModTime);
            }
        }

        /// <summary>
        /// Sets the caching used for resources on the http response cache policy.
        /// </summary>
        public static void SetCachingPolicy(HttpCachePolicy cachePolicy)
        {
            cachePolicy.SetCacheability(HttpCacheability.Private);
            cachePolicy.SetNoServerCaching();
            cachePolicy.SetMaxAge(TimeSpan.FromMinutes(5));

            /* NOTE: The two calls below prevent resources from being stored on 
               disk but they also set the expiration to be immediate. Which means 
               that if we preload images (e.x., grid) then the next time they are 
               requested they will again go back to the server. So we cannot use these. */
            // cachePolicy.SetNoStore();
            // cachePolicy.SetCacheability(HttpCacheability.NoCache);
        }

        internal static bool SendEntireEntity(HttpContext context, string strETag, DateTime lastModifiedTime)
        {
            bool flag = false;
            string str = context.Request.Headers["If-Range"];
            
            if (str == null)
            {
                return false;
            }
            
            if (str[0] == '"')
            {
                if (!CompareETags(str, strETag))
                {
                    flag = true;
                }
                return flag;
            }
            
            try
            {
                DateTime time = DateTime.Parse(str, CultureInfo.InvariantCulture);
                if (DateTime.Compare(lastModifiedTime, time) == 1)
                {
                    flag = true;
                }
            }
            catch
            {
                flag = true;
            }
            
            return flag;
        }

        internal static bool FileExists(string filename)
        {
            bool flag = false;
            
            try
            {
                flag = File.Exists(filename);
            }
            catch
            {
            }
            
            return flag;
        }


    }
}
