/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Text;
using System.Web;
using System.Web.Hosting;

namespace AIR.Common.Web
{
    public static class UrlHelper
    {
        /// <summary>
        /// Check if the url is HTTP.
        /// </summary>
        public static bool IsHttpProtocol(string uriString)
        {
            if (String.IsNullOrWhiteSpace(uriString)) return false;
            return (uriString.StartsWith("http:", StringComparison.OrdinalIgnoreCase) ||
                    uriString.StartsWith("https:", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Check if the url is FTP.
        /// </summary>
        public static bool IsFtpProtocol(string uriString)
        {
            if (String.IsNullOrWhiteSpace(uriString)) return false;
            return (uriString.StartsWith("ftp:", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Check if the url is file path.
        /// </summary>
        public static bool IsFileProtocol(string uriString)
        {
            if (String.IsNullOrWhiteSpace(uriString)) return false;
            return (uriString.StartsWith("file:", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// The URI scheme
        /// </summary>
        public static string Scheme
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    string protocol = HttpContext.Current.Request.ServerVariables["SERVER_PORT_SECURE"];
                    return (protocol == null || protocol == "0") ? "http" : "https";
                }

                return null;
            }
        }

        /// <summary>
        /// The URL protocol (HTTP or HTTPS)
        /// </summary>
        /// <remarks>
        /// Any usages of this should be replaced by the Scheme property when possible.
        /// </remarks>
        public static string Protocol
        {
            get
            {
                if (HttpContext.Current == null) return null;
                HttpRequest request = HttpContext.Current.Request;

                string protocol = request.ServerVariables["SERVER_PORT_SECURE"];
                return (protocol == null || protocol == "0") ? "http://" : "https://";
            }
        }

        /// <summary>
        /// The domain name
        /// </summary>
        public static string Host
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    return HttpContext.Current.Request.Url.Host;
                }

                return null;
            }
        }
       
        /// <summary>
        /// The URL port
        /// </summary>
        public static int Port
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    return HttpContext.Current.Request.Url.Port;
                }
                
                return 0;
            }
        }

        public static string PortSuffix
        {
            get
            {
                if (HttpContext.Current == null) return null;
                HttpRequest request = HttpContext.Current.Request;

                string port = request.ServerVariables["SERVER_PORT"];

                if (port == null || port == "80" || port == "443")
                {
                    return "";
                }
                
                return ":" + port;
            }
        }

        public static string ApplicationPath
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    return HttpContext.Current.Request.ApplicationPath;
                }

                return null;
            }
        }
        
        /// <summary>
        /// The base URL of the site
        /// </summary>
        public static string Base
        {
            get
            {
                if (HttpContext.Current == null) return null;
                HttpRequest request = HttpContext.Current.Request;

                // NOTE: We removed Port property because it was getting added for Rackspace Cloud
                // return Protocol + request.ServerVariables["SERVER_NAME"] + Port + request.ApplicationPath + "/";
                return Protocol + request.ServerVariables["SERVER_NAME"] + request.ApplicationPath + "/";
            }
        }

        /// <summary>
        /// The base URL of the site
        /// </summary>
        public static string BaseWithPort
        {
            get
            {
                if (HttpContext.Current == null) return null;
                HttpRequest request = HttpContext.Current.Request;

                return Protocol + request.ServerVariables["SERVER_NAME"] + PortSuffix + request.ApplicationPath + "/";
            }
        }

        /// <summary>
        /// Get the root url.
        /// </summary>
        /// <returns>https://tds2.airws.org</returns>
        public static string GetRoot(bool includePort = false)
        {
            if (HttpContext.Current == null) return null;
            HttpRequest request = HttpContext.Current.Request;

            string scheme;

            // get scheme
            if (request.IsSecureConnection)
            {
                scheme = "https";
            }
            else
            {
                scheme = "http";
            }

            // add scheme
            StringBuilder uri = new StringBuilder(scheme + "://");
            
            // add domain
            string hostname = request.Url.Host;
            uri.Append(hostname);

            // add port
            if (includePort)
            {
                int port = request.Url.Port;

                // only add port if different than defaults
                if (port > 0 && (port != 80 && port != 443))
                {
                    uri.Append(":");
                    uri.Append(port.ToString());
                }
            }

            return uri.ToString();
        }

        /// <summary>
        /// Get the server application url. 
        /// </summary>
        /// <returns>https://tds2.airws.org/student/</returns>
        public static string GetApplication(bool includePort = false)
        {
            if (HttpContext.Current != null)
            {
                string rootUrl = GetRoot(includePort);
                return rootUrl + HttpContext.Current.Request.ApplicationPath + "/";
            }

            return null;
        }

        /// <summary>
        /// Replace url parameters with the current server url info.
        /// </summary>
        /// <returns>Any parameters will be replaced in the returned string.</returns>
        /// <example>
        /// ORIGINAL: "{scheme}://{host}{port}/ItemScoring/v1/ItemScoring.axd"
        /// FORMATTED: "https://tds2.airws.org/ItemScoring/v1/ItemScoring.axd"
        /// </example>
        public static string Format(string url)
        {
            // use this if you want to skip individual pieces
            if (url.Contains("{root}"))
            {
                url = url.Replace("{root}", GetRoot()); // https://tds2.airws.org (no port)
            }
            else
            {
                url = url.Replace("{scheme}", Scheme); // "https"
                url = url.Replace("{host}", Host); // "tds2.airws.org"
                url = url.Replace("{port}", PortSuffix); // ":8080" (only provided if not 80/443)
            }
            
            return url;
        }

        /// <summary>
        /// Returns a site relative HTTP path from a partial path starting out with a ~.
        /// Same syntax that ASP.Net internally supports but this method can be used
        /// outside of the Page framework.
        /// 
        /// Works like Control.ResolveUrl including support for ~ syntax
        /// but returns an absolute URL.
        /// </summary>
        /// <param name="virtualPath">Any Url including those starting with ~</param>
        /// <returns>relative url</returns>
        /// <remarks>
        /// Source: http://clientdependency.codeplex.com/SourceControl/changeset/view/73172#1481746
        /// Additional: http://west-wind.com/weblog/posts/154812.aspx
        /// </remarks>
        public static string ResolveUrl(string virtualPath)
        {
            if (string.IsNullOrEmpty(virtualPath)) return virtualPath;

            // *** Absolute path - just return
            if (IsAbsolutePath(virtualPath)) return virtualPath;

            // *** We don't start with the '~' -> we don't process the Url
            if (!virtualPath.StartsWith("~")) return virtualPath;

            // *** Fix up path for ~ root app dir directory
            // VirtualPathUtility blows up if there is a 
            // query string, so we have to account for this.
            int queryStringStartIndex = virtualPath.IndexOf('?');

            if (queryStringStartIndex != -1)
            {
                string queryString = virtualPath.Substring(queryStringStartIndex);
                string baseUrl = virtualPath.Substring(0, queryStringStartIndex);

                return string.Concat(
                    VirtualPathUtility.ToAbsolute(baseUrl, HttpRuntime.AppDomainAppVirtualPath),
                    queryString);
            }

            return VirtualPathUtility.ToAbsolute(virtualPath, HttpRuntime.AppDomainAppVirtualPath);
        }

        /// <summary>
        /// Checks for an absolute http path
        /// </summary>
        /// <remarks>
        /// Takes into account this type of url:
        /// ~/pathtoresolve/page.aspx?returnurl=http://servertoredirect/resource.aspx
        /// which is not an absolute path but contains the characters to describe it as one.
        /// </remarks>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        private static bool IsAbsolutePath(string virtualPath)
        {
            // *** Absolute path - just return
            int IndexOfSlashes = virtualPath.IndexOf("://");
            int IndexOfQuestionMarks = virtualPath.IndexOf("?");

            if (IndexOfSlashes > -1 &&
                 (IndexOfQuestionMarks < 0 ||
                  (IndexOfQuestionMarks > -1 && IndexOfQuestionMarks > IndexOfSlashes)
                  )
                )
                return true;

            return false;
        }

        public static string ResolveFullUrl(string virtualPath)
        {
            Uri baseUri = new Uri(Base);
            string resolvedUrl = ResolveUrl(virtualPath);
            Uri resolveUri = new Uri(baseUri, resolvedUrl);
            return resolveUri.ToString();
        } 

        /// <summary>
        /// Returns the physical file path that corresponds to the specified virtual file path on the Web Server. 
        /// </summary>
        /// <remarks>
        /// This wraps up the built in ASP.NET ways of doing this.
        /// </remarks>
        public static string MapPath(string virtualPath)
        {
            return (HttpContext.Current != null) ? 
                HttpContext.Current.Server.MapPath(virtualPath) : 
                HostingEnvironment.MapPath(virtualPath);
        }
    }
}
