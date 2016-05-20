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

namespace TDS.ItemScoringEngine
{
    public static class ItemScorerUtil
    {
        /// <summary>
        /// Retrieve contents from URI as a String
        /// </summary>
        /// <param name="uri">URI of rubric</param>
        /// <returns>Contents of rubric</returns>
        public static String GetContent(Uri uri)
        {
            using (var stream = GetContentStream(uri))
            {
                if (stream != null)
                        using (var reader = new StreamReader(stream))
                            return reader.ReadToEnd();
                    }
            return null;
        }

        /// <summary>
        /// Retrieve the contents from the URI as a stream
        /// </summary>
        /// <param name="uri">URI of rubric</param>
        /// <returns>Stream for the rubric contents</returns>
        public static Stream GetContentStream(Uri uri)
        {
            if (uri.IsFile)
            {
                return File.Open(uri.LocalPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            }

            WebRequest request = WebRequest.Create(uri);
            var webRequest = request as HttpWebRequest;
            if (webRequest != null)
            {
                webRequest.KeepAlive = false;
            }

            return request.GetResponse().GetResponseStream();
        }
    }
}
