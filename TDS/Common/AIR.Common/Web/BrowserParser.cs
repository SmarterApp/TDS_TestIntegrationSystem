/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;

namespace AIR.Common.Web
{
    /// <summary>
    /// Used to parse the current http requests browser info.
    /// </summary>
    /// <remarks>
    /// UA regex's:
    /// https://github.com/tobie/ua-parser/blob/master/regexes.yaml
    /// https://github.com/faisalman/ua-parser-js/
    /// </remarks>
    public class BrowserParser
    {
        private readonly string _userAgent;
        private readonly HttpBrowserCapabilities _browserCaps;

        // parser lookups
        static readonly Dictionary<BrowserOS, Regex> _osVersionParsers = new Dictionary<BrowserOS, Regex>();
        static readonly Dictionary<string, Regex> _browserVersionParsers = new Dictionary<string, Regex>(StringComparer.InvariantCultureIgnoreCase);

        // platforms
        static readonly List<string> _platformWindows = new List<string>(new[] { "Windows", "Win32", "Win64" });
        static readonly List<string> _platformOSX = new List<string>(new[] { "Macintosh", "MacPPC", "MacIntel" });
        static readonly List<string> _platformLinux = new List<string>(new[] { "X11", "Linux", "BSD" });
        static readonly List<string> _platformIOS = new List<string>(new[] { "iPad", "iPod", "iPhone" });

        static BrowserParser()
        {
            LoadOSVersionParsers();
            LoadBrowserVersionParsers();
        }

        static void AddOSVersionParser(BrowserOS os, string regex)
        {
            _osVersionParsers.Add(os, new Regex(regex, RegexOptions.Compiled));
        }

        static void AddBrowserVersionParser(string name, string regex)
        {
            _browserVersionParsers.Add(name, new Regex(regex, RegexOptions.Compiled));
        }

        static void LoadOSVersionParsers()
        {
            AddOSVersionParser(BrowserOS.OSX, @"Mac OS X (\d+\.\d+)");
            AddOSVersionParser(BrowserOS.Android, @"Android (\d+\.\d+)");
            AddOSVersionParser(BrowserOS.IOS, @"i[PSa-z\s]+;.*?CPU\s[OSPa-z\s]+(?:(\d_\d)|;)");
            AddOSVersionParser(BrowserOS.Chrome, @"Chrome\/(\d+\.\d+)");
            AddOSVersionParser(BrowserOS.Windows, @"Windows NT (\d+\.\d+)");
        }

        static void LoadBrowserVersionParsers()
        {
            AddBrowserVersionParser("AIRSecureBrowser", @"AIRSecureBrowser/(\d+\.\d+)");
            AddBrowserVersionParser("AIRMobileSecureBrowser", @"AIRMobile[SecureBrowser]*/(\d+\.\d+)");
        }

        static double ParseVersion(string userAgent, Regex regex)
        {
            double version = default(double);

            MatchCollection matches = regex.Matches(userAgent);

            if (matches.Count > 0 && matches[0].Groups.Count >= 2)
            {
                string value = matches[0].Groups[1].Value;

                if (!string.IsNullOrEmpty(value))
                {
                    value = value.Replace('_', '.');
                    double.TryParse(value, out version);
                }
            }

            return version;
        }

        double ParseOSVersion(BrowserOS os)
        {
            Regex versionParser;

            if (_osVersionParsers.TryGetValue(os, out versionParser))
            {
                return ParseVersion(UserAgent, versionParser);
            }

            return 0;
        }

        double ParseBrowserVersion(string name)
        {
            Regex versionParser;

            if (_browserVersionParsers.TryGetValue(name, out versionParser))
            {
                return ParseVersion(UserAgent, versionParser);
            }

            return 0;
        }

        public static BrowserParser Current
        {
            get
            {
                return new BrowserParser();
            }
        }

        public BrowserParser(string userAgent, HttpBrowserCapabilities browserCaps)
        {
            _userAgent = userAgent;
            _browserCaps = browserCaps;
        }

        public BrowserParser()
        {
            _userAgent = HttpContext.Current.Request.UserAgent;
            _browserCaps = HttpContext.Current.Request.Browser;
        }

        /// <summary>
        /// The full user agent for the browser
        /// </summary>
        public string UserAgent
        {
            get { return _userAgent; }
        }

        /// <summary>
        /// The full OS marketing name (e.x., Windows 98)
        /// </summary>
        public string OSFullName
        {
            get
            {
                object osObj = _browserCaps.Capabilities["OS"];
                string osFullName = (osObj != null) ? osObj.ToString() : "Unknown";

                // BUG #155451: Diagnostic page displays incorrect OS (Macintosh OS X) for iOS SB 3.0
                if (UserAgent.Contains("iPad"))
                {
                    return "iOS";
                }

                // check if the browser could not be detected
                if (osFullName == "Unknown")
                {
                    // BUG #11956: Mac OS X 10.4
                    // EXAMPLE: Mozilla/5.0 (Macintosh; U; PPC Mac OS X Mach-O; en-US; rv:1.8.1.7) Gecko/20090728 (SZcr0DctLYrAxsYhQbnS) AIRSecureBrowser/3.0
                    if (UserAgent.Contains("OS X"))
                    {
                        return "Macintosh OS X";
                    }

                    // EXAMPLE: Mozilla/5.0 (X11; CrOS x86_64 2913.331.0) AppleWebKit/537.11 (KHTML, like Gecko) Chrome/23.0.127.111 Safari/537.11 AIRSecureBrowser/1.1
                    if (UserAgent.Contains(" CrOS "))
                    {
                        return "Chrome OS";
                    }
                }

                // Windows 7
                if (UserAgent.Contains("Windows NT 6.1"))
                {
                    return "Windows 7";
                }

                // BUG #54823: Windows 8
                // EXAMPLE: Mozilla/5.0 (Windows NT 6.2; WOW64; rv:10.0.1) Gecko/20100101 Firefox/10.0.1
                if (UserAgent.Contains("Windows NT 6.2"))
                {
                    return "Windows 8";
                }

                if (UserAgent.Contains("Windows NT 6.3"))
                {
                    return "Windows 8.1";
                }

                if (UserAgent.Contains("Android"))
                {
                    return "Android";
                }

                return osFullName;
            }
        }

        /// <summary>
        /// The general OS platform being used (e.x., WINDOWS)
        /// </summary>
        public BrowserOS OSName
        {
            get
            {
                if (UserAgent.Contains(" CrOS "))
                {
                    return BrowserOS.Chrome;
                }

                if (UserAgent.Contains("Android"))
                {
                    return BrowserOS.Android;
                }

                if (_platformIOS.Any(s => UserAgent.Contains(s))) {
                    return BrowserOS.IOS; // NOTE: needs to be before OS X
                }

                if (_platformOSX.Any(s => UserAgent.Contains(s)))
                {
                    return BrowserOS.OSX;
                }

                if (_platformWindows.Any(s => UserAgent.Contains(s)))
                {
                    return BrowserOS.Windows;
                }

                if (_platformLinux.Any(s => UserAgent.Contains(s)))
                {
                    return BrowserOS.Linux;
                }
                
                return BrowserOS.Unknown;
            }
        }

        public double OSVersion
        {
            get { return ParseOSVersion(OSName); }
        }

        /// <summary>
        /// Browsers name (e.x., Firefox)
        /// </summary>
        public string Name
        {
            get
            {
                // check for secure browsers
                if (UserAgent != null)
                {
                    if (UserAgent.Contains("AIRSecureBrowser"))
                    {
                        return "AIRSecureBrowser";
                    }

                    if (UserAgent.Contains("AIRMobile"))
                    {
                        return "AIRMobileSecureBrowser";
                    }
                }

                if (_browserCaps.Browser != null)
                {
                    if (_browserCaps.Browser == "InternetExplorer")
                    {
                        return "IE";
                    }
                }

                // return ASP.NET browser name
                return _browserCaps.Browser ?? "";
            }
        }

        public bool IsIE
        {
            get
            {
                if (UserAgent == null) return false;
                return UserAgent.Contains("MSIE") || 
                       UserAgent.Contains("Trident");
            }
        }

        public bool IsSafari
        {
            get
            {
                if (UserAgent == null) return false;
                return UserAgent.Contains("Safari");
            }
        }

        public bool IsChrome
        {
            get
            {
                if (UserAgent == null) return false;
                return UserAgent.Contains("Chrome");
            }
        }

        public bool IsWebKit
        {
            get
            {
                if (UserAgent == null) return false;
                return UserAgent.Contains("AppleWebKit");
            }
        }

        public bool IsFirefox
        {
            get
            {
                if (UserAgent == null) return false;

                // ignore browsers that are "like Gecko" (Safari, IE 11)
                if (UserAgent.Contains("like Gecko")) return false;
                
                // check if Firefox
                return (UserAgent.Contains("Firefox") || UserAgent.Contains("Gecko"));
            }
        }

        /// <summary>
        /// Check if browser is capable of playing ogg audio.
        /// </summary>
        public bool SupportsOggAudio
        {
            get
            {
                // mobile devices do not support ogg regardless of the browser
                if (OSName == BrowserOS.IOS ||
                    OSName == BrowserOS.Android) return false;

                // return browsers that support ogg natively
                return (IsFirefox || IsChrome);
            }
        }

        /// <summary>
        /// The browsers version (e.x., 2.0)
        /// </summary>
        /// <remarks>
        /// This is only major and minor version #'s
        /// </remarks>
        public double Version
        {
            get
            {
                // check if custom version parser
                double version = ParseBrowserVersion(Name);
                if (version > 0) return version;

                // this code is not working in .NET 4.0, (e.x., Firefox 3.5: Max=3 + Min=5.0 = 8)
                // return browserCapabilities.MajorVersion + browserCapabilities.MinorVersion;

                // version string is an integer and a decimal
                if (_browserCaps.Version != null)
                {
                    Double.TryParse(_browserCaps.Version, out version);
                }

                return version;
            }
        }

        /// <summary>
        /// Get the type of hardware this OS is running.
        /// </summary>
        public string HardwareArchitecture
        {
            get
            {
                if (OSName == BrowserOS.OSX)
                {
                    if (UserAgent.Contains("Intel")) return "Intel";
                    if (UserAgent.Contains("PPC")) return "PPC";
                }
                else if (OSName == BrowserOS.IOS)
                {
                    if (UserAgent.Contains("iPad")) return "iPad";
                    if (UserAgent.Contains("iPhone")) return "iPhone";
                    if (UserAgent.Contains("iPod")) return "iPod";
                }
                else if (OSName == BrowserOS.Chrome)
                {
                    if (UserAgent.Contains("i686")) return "i686";
                    if (UserAgent.Contains("x86_64")) return "x86_64";
                    if (UserAgent.Contains("armv7l")) return "ARM";
                }
                else if (OSName == BrowserOS.Windows)
                {
                    if (UserAgent.Contains("ARM;")) return "ARM";
                    else return "Intel";
                }
                
                return null;
            }
        }

        /// <summary>
        /// Check if the browser is one our valid secure browsers.
        /// </summary>
        public bool IsSecureBrowser
        {
            get
            {
                // this can be null if using load testing app of some sort
                if (string.IsNullOrEmpty(UserAgent)) return false;

                // check for desktop SB
                if (Name == "AIRSecureBrowser") return true;

                // check for mobile SB
                if (Name == "AIRMobileSecureBrowser") return true;

                return false;
            }
        }
    }

    public enum BrowserOS
    {
        Unknown,
        Windows,
        OSX,
        Linux,
        IOS,
        Android,
        Chrome
    }

}
