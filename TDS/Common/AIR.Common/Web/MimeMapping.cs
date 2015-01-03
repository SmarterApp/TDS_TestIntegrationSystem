/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Collections;

namespace AIR.Common.Web
{
    /// <summary>
    /// A collection of well known MIME types.
    /// </summary>
    public class MimeMapping
    {
        // Fields
        private static Hashtable _extensionToMimeMappingTable = new Hashtable(200, StringComparer.CurrentCultureIgnoreCase);

        // Methods
        static MimeMapping()
        {
            // CUSTOM:
            AddMapping(".swf", "application/x-shockwave-flash");
            AddMapping(".flv", "video/x-flv");
            AddMapping(".png", "image/png");
            AddMapping(".ogg", "application/ogg"); // audio/ogg
            AddMapping(".m4a", "audio/mp4");
            AddMapping(".mp4", "video/mp4");

            // ASP.NET's:
            AddMapping(".323", "text/h323");
            AddMapping(".asx", "video/x-ms-asf");
            AddMapping(".acx", "application/internet-property-stream");
            AddMapping(".ai", "application/postscript");
            AddMapping(".aif", "audio/x-aiff");
            AddMapping(".aiff", "audio/aiff");
            AddMapping(".axs", "application/olescript");
            AddMapping(".aifc", "audio/aiff");
            AddMapping(".asr", "video/x-ms-asf");
            AddMapping(".avi", "video/x-msvideo");
            AddMapping(".asf", "video/x-ms-asf");
            AddMapping(".au", "audio/basic");
            AddMapping(".application", "application/x-ms-application");
            AddMapping(".bin", "application/octet-stream");
            AddMapping(".bas", "text/plain");
            AddMapping(".bcpio", "application/x-bcpio");
            AddMapping(".bmp", "image/bmp");
            AddMapping(".cdf", "application/x-cdf");
            AddMapping(".cat", "application/vndms-pkiseccat");
            AddMapping(".crt", "application/x-x509-ca-cert");
            AddMapping(".c", "text/plain");
            AddMapping(".css", "text/css");
            AddMapping(".cer", "application/x-x509-ca-cert");
            AddMapping(".crl", "application/pkix-crl");
            AddMapping(".cmx", "image/x-cmx");
            AddMapping(".csh", "application/x-csh");
            AddMapping(".cod", "image/cis-cod");
            AddMapping(".cpio", "application/x-cpio");
            AddMapping(".clp", "application/x-msclip");
            AddMapping(".crd", "application/x-mscardfile");
            AddMapping(".deploy", "application/octet-stream");
            AddMapping(".dll", "application/x-msdownload");
            AddMapping(".dot", "application/msword");
            AddMapping(".doc", "application/msword");
            AddMapping(".dvi", "application/x-dvi");
            AddMapping(".dir", "application/x-director");
            AddMapping(".dxr", "application/x-director");
            AddMapping(".der", "application/x-x509-ca-cert");
            AddMapping(".dib", "image/bmp");
            AddMapping(".dcr", "application/x-director");
            AddMapping(".disco", "text/xml");
            AddMapping(".exe", "application/octet-stream");
            AddMapping(".etx", "text/x-setext");
            AddMapping(".evy", "application/envoy");
            AddMapping(".eml", "message/rfc822");
            AddMapping(".eps", "application/postscript");
            AddMapping(".flr", "x-world/x-vrml");
            AddMapping(".fif", "application/fractals");
            AddMapping(".gtar", "application/x-gtar");
            AddMapping(".gif", "image/gif");
            AddMapping(".gz", "application/x-gzip");
            AddMapping(".hta", "application/hta");
            AddMapping(".htc", "text/x-component");
            AddMapping(".htt", "text/webviewhtml");
            AddMapping(".h", "text/plain");
            AddMapping(".hdf", "application/x-hdf");
            AddMapping(".hlp", "application/winhlp");
            AddMapping(".html", "text/html");
            AddMapping(".htm", "text/html");
            AddMapping(".hqx", "application/mac-binhex40");
            AddMapping(".isp", "application/x-internet-signup");
            AddMapping(".iii", "application/x-iphone");
            AddMapping(".ief", "image/ief");
            AddMapping(".ivf", "video/x-ivf");
            AddMapping(".ins", "application/x-internet-signup");
            AddMapping(".ico", "image/x-icon");
            AddMapping(".jpg", "image/jpeg");
            AddMapping(".jfif", "image/pjpeg");
            AddMapping(".jpe", "image/jpeg");
            AddMapping(".jpeg", "image/jpeg");
            AddMapping(".js", "application/x-javascript");
            AddMapping(".lsx", "video/x-la-asf");
            AddMapping(".latex", "application/x-latex");
            AddMapping(".lsf", "video/x-la-asf");
            AddMapping(".manifest", "application/x-ms-manifest");
            AddMapping(".mhtml", "message/rfc822");
            AddMapping(".mny", "application/x-msmoney");
            AddMapping(".mht", "message/rfc822");
            AddMapping(".mid", "audio/mid");
            AddMapping(".mpv2", "video/mpeg");
            AddMapping(".man", "application/x-troff-man");
            AddMapping(".mvb", "application/x-msmediaview");
            AddMapping(".mpeg", "video/mpeg");
            AddMapping(".m3u", "audio/x-mpegurl");
            AddMapping(".mdb", "application/x-msaccess");
            AddMapping(".mpp", "application/vnd.ms-project");
            AddMapping(".m1v", "video/mpeg");
            AddMapping(".mpa", "video/mpeg");
            AddMapping(".me", "application/x-troff-me");
            AddMapping(".m13", "application/x-msmediaview");
            AddMapping(".movie", "video/x-sgi-movie");
            AddMapping(".m14", "application/x-msmediaview");
            AddMapping(".mpe", "video/mpeg");
            AddMapping(".mp2", "video/mpeg");
            AddMapping(".mov", "video/quicktime");
            AddMapping(".mp3", "audio/mpeg");
            AddMapping(".mpg", "video/mpeg");
            AddMapping(".ms", "application/x-troff-ms");
            AddMapping(".nc", "application/x-netcdf");
            AddMapping(".nws", "message/rfc822");
            AddMapping(".oda", "application/oda");
            AddMapping(".ods", "application/oleobject");
            AddMapping(".pmc", "application/x-perfmon");
            AddMapping(".p7r", "application/x-pkcs7-certreqresp");
            AddMapping(".p7b", "application/x-pkcs7-certificates");
            AddMapping(".p7s", "application/pkcs7-signature");
            AddMapping(".pmw", "application/x-perfmon");
            AddMapping(".ps", "application/postscript");
            AddMapping(".p7c", "application/pkcs7-mime");
            AddMapping(".pbm", "image/x-portable-bitmap");
            AddMapping(".ppm", "image/x-portable-pixmap");
            AddMapping(".pub", "application/x-mspublisher");
            AddMapping(".pnm", "image/x-portable-anymap");
            AddMapping(".pml", "application/x-perfmon");
            AddMapping(".p10", "application/pkcs10");
            AddMapping(".pfx", "application/x-pkcs12");
            AddMapping(".p12", "application/x-pkcs12");
            AddMapping(".pdf", "application/pdf");
            AddMapping(".pps", "application/vnd.ms-powerpoint");
            AddMapping(".p7m", "application/pkcs7-mime");
            AddMapping(".pko", "application/vndms-pkipko");
            AddMapping(".ppt", "application/vnd.ms-powerpoint");
            AddMapping(".pmr", "application/x-perfmon");
            AddMapping(".pma", "application/x-perfmon");
            AddMapping(".pot", "application/vnd.ms-powerpoint");
            AddMapping(".prf", "application/pics-rules");
            AddMapping(".pgm", "image/x-portable-graymap");
            AddMapping(".qt", "video/quicktime");
            AddMapping(".ra", "audio/x-pn-realaudio");
            AddMapping(".rgb", "image/x-rgb");
            AddMapping(".ram", "audio/x-pn-realaudio");
            AddMapping(".rmi", "audio/mid");
            AddMapping(".ras", "image/x-cmu-raster");
            AddMapping(".roff", "application/x-troff");
            AddMapping(".rtf", "application/rtf");
            AddMapping(".rtx", "text/richtext");
            AddMapping(".sv4crc", "application/x-sv4crc");
            AddMapping(".spc", "application/x-pkcs7-certificates");
            AddMapping(".setreg", "application/set-registration-initiation");
            AddMapping(".snd", "audio/basic");
            AddMapping(".stl", "application/vndms-pkistl");
            AddMapping(".setpay", "application/set-payment-initiation");
            AddMapping(".stm", "text/html");
            AddMapping(".shar", "application/x-shar");
            AddMapping(".sh", "application/x-sh");
            AddMapping(".sit", "application/x-stuffit");
            AddMapping(".spl", "application/futuresplash");
            AddMapping(".sct", "text/scriptlet");
            AddMapping(".scd", "application/x-msschedule");
            AddMapping(".sst", "application/vndms-pkicertstore");
            AddMapping(".src", "application/x-wais-source");
            AddMapping(".sv4cpio", "application/x-sv4cpio");
            AddMapping(".tex", "application/x-tex");
            AddMapping(".tgz", "application/x-compressed");
            AddMapping(".t", "application/x-troff");
            AddMapping(".tar", "application/x-tar");
            AddMapping(".tr", "application/x-troff");
            AddMapping(".tif", "image/tiff");
            AddMapping(".txt", "text/plain");
            AddMapping(".texinfo", "application/x-texinfo");
            AddMapping(".trm", "application/x-msterminal");
            AddMapping(".tiff", "image/tiff");
            AddMapping(".tcl", "application/x-tcl");
            AddMapping(".texi", "application/x-texinfo");
            AddMapping(".tsv", "text/tab-separated-values");
            AddMapping(".ustar", "application/x-ustar");
            AddMapping(".uls", "text/iuls");
            AddMapping(".vcf", "text/x-vcard");
            AddMapping(".vtt", "text/vtt");
            AddMapping(".wps", "application/vnd.ms-works");
            AddMapping(".wav", "audio/wav");
            AddMapping(".wrz", "x-world/x-vrml");
            AddMapping(".wri", "application/x-mswrite");
            AddMapping(".wks", "application/vnd.ms-works");
            AddMapping(".wmf", "application/x-msmetafile");
            AddMapping(".wcm", "application/vnd.ms-works");
            AddMapping(".wrl", "x-world/x-vrml");
            AddMapping(".wdb", "application/vnd.ms-works");
            AddMapping(".wsdl", "text/xml");
            AddMapping(".xml", "text/xml");
            AddMapping(".xlm", "application/vnd.ms-excel");
            AddMapping(".xaf", "x-world/x-vrml");
            AddMapping(".xla", "application/vnd.ms-excel");
            AddMapping(".xls", "application/vnd.ms-excel");
            AddMapping(".xof", "x-world/x-vrml");
            AddMapping(".xlt", "application/vnd.ms-excel");
            AddMapping(".xlc", "application/vnd.ms-excel");
            AddMapping(".xsl", "text/xml");
            AddMapping(".xbm", "image/x-xbitmap");
            AddMapping(".xlw", "application/vnd.ms-excel");
            AddMapping(".xpm", "image/x-xpixmap");
            AddMapping(".xwd", "image/x-xwindowdump");
            AddMapping(".xsd", "text/xml");
            AddMapping(".z", "application/x-compress");
            AddMapping(".zip", "application/x-zip-compressed");
            AddMapping(".*", "application/octet-stream");
        }

        private MimeMapping()
        {
        }

        private static void AddMapping(string extension, string MimeType)
        {
            _extensionToMimeMappingTable.Add(extension, MimeType);
        }

        public static string GetMapping(string FileName)
        {
            string str = null;
            int startIndex = FileName.LastIndexOf('.');
            if ((0 < startIndex) && (startIndex > FileName.LastIndexOf('\\')))
            {
                str = (string) _extensionToMimeMappingTable[FileName.Substring(startIndex)];
            }
            if (str == null)
            {
                str = (string) _extensionToMimeMappingTable[".*"];
            }
            return str;
        }
    }


}
