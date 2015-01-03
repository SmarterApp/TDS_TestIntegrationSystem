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
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using WinSCP;
using CommonUtilities.Configuration;

namespace CommonUtilities
{
    /// <summary>
    /// This class uses the new WinSCP.dll library, first released in WinSCP 5.1.  The dll uses
    /// WinSCP.exe 5.1, so that should be installed on the server.  Previous versions will not work
    /// with this DLL.  Info about the library: http://winscp.net/eng/docs/library
    /// Instead of installing WinSCP on the server, we could use the portable version and make sure
    /// it is copied to the bin directory of each app that's using it.  Note that even the portable 
    /// version by default will write seed files and temp files to system dirs, though I believe it can 
    /// be configured to be completely portable if required.  This would be done in the exe's preferences.
    /// Info about using the portable WinSCP: http://winscp.net/eng/docs/portable
    /// </summary>
    public class WinSCPLibWrapper
    {
        //Use WinSCPDebugLogFilePath instead.
        //// If set, WinSCP will log to this file.  Default path is current exe path.
        //private const string WINSCP_DEBUG_LOG_FILE = null; // "WinSCP_Debug.log";

        private const int DEDAULT_FTP_PORT = 21;
        private const int DEDAULT_SFTP_PORT = 22;
        private const int NO_PORT = -1;

        public const int NO_KEEP_ALIVE = -1;

        private const string WINSCP_EXE = "WinSCP.exe";
        private const string WINSCP_COM = "WinSCP.com";

        private string WinSCPDebugLogFilePath { get; set; }
        private int KeepAliveInterval { get; set; }

        /// <summary>
        /// Ctor.  WinSCP debug logging is turned off.
        /// </summary>
        public WinSCPLibWrapper() : this(null, null) { }

        /// <summary>
        /// Ctor.  Can provide path/filename for logging debug info from WinSCP.dll.
        /// Default path is current exe path if only filename is provided.  Path can be relative or absolute.
        /// Pass null or String.Empty or use ctor with no params to turn off WinSCP debug logging for the session.
        /// Also added keepAliveInterval to try it for Oregon.  TODO: refactor, esp if the keepalive thing works.
        /// </summary>
        /// <param name="winSCPDebugLogFilePath"></param>
        public WinSCPLibWrapper(string winSCPDebugLogFilePath, int? keepAliveInterval)
        {
            this.WinSCPDebugLogFilePath = winSCPDebugLogFilePath;
            this.KeepAliveInterval = keepAliveInterval ?? NO_KEEP_ALIVE;
        }

        /// <summary>
        /// Retreive a list of files from an FTP or SFTP site.  If there is a file with
        /// the same name in the target directory, it will be overwritten.
        /// </summary>
        /// <param name="remoteUrl">The full URL or host URL of the SFTP or FTP site.
        /// Should be prefixed with protocol (sftp:// or ftp://).  If there is no protocol, 
        /// SFTP is assumed.  Can also include the port if not using the default port of 
        /// 21 for FTP and 22 for SFTP.  The URL can contain the full path to the desired 
        /// directory, but must at least contain the host url or name, in which case 
        /// remoteDir can be used to specify the rest of the path.
        /// Examples:
        /// 192.168.240.75/_Datafiles/mn_ext_airast_org/MDE/ResolutionFiles/
        /// 192.168.240.75:27/_Datafiles/mn_ext_airast_org/MDE/ResolutionFiles
        /// 192.168.240.75
        /// sftp://192.168.240.75/_Datafiles/mn_ext_airast_org
        /// ftp://38.118.83.22/datafiles/tis_astprojects_org/ResolutionFileDrop/
        /// ftp://38.118.83.22:987
        /// </param>
        /// <param name="userName">FTP/SFTP account username</param>
        /// <param name="password"></param>
        /// <param name="fileNames">A List of filenames to download</param>
        /// <param name="localDir">An absolute path indicating where to download the files to.</param>
        /// <param name="remoteDir">If not included in the remoteUrl, the relative path to the files to download</param>
        /// <param name="winSCPexePath">The path to the WinSCP.exe that will be used by the WinSCP.dll.
        /// Must be at least v 5.1.</param>
        /// <param name="hostKey">The ssh host key of the SFTP server, or the FTPS cert when using FTP over SSL.  
        /// If not set, it is assumed that the copy of WinSCP.exe at winSCPexePath has cached the key.  N/A for FTP w/o SSL.</param>
        /// <param name="sslRequired">For FTP over SSL</param>
        /// <param name="transferMode">Binary (default), Text, Auto</param>
        /// <param name="timeoutInSeconds">Maximal interval between two consecutive outputs from WinSCP console session, 
        /// before TimeoutException is thrown. The default is one minute if a value is not provided.  I don't think we'll
        /// ever need to set this.  I tested putting/getting an 850MB file that took hours w/o setting this value and it worked fine.</param>
        /// <param name="removeSourceFiles">Set to true to delete the files from the source directory after download</param>
        public void GetFiles(string remoteUrl, string userName, string password,
            List<string> fileNames, string localDir, string remoteDir, string winSCPexePath,
            string hostKey, bool sslRequired, FTPConst.TransferMode transferMode, int? timeoutInSeconds, bool removeSourceFiles)
        {
            Uri uri = GetUri(remoteUrl, remoteDir);
            SessionOptions sessionOptions = CreateSessionOptions(uri, userName, password, hostKey, sslRequired);
            using (Session session = CreateSession(sessionOptions, winSCPexePath, timeoutInSeconds))
            {
                TransferOptions transferOptions = new TransferOptions();
                transferOptions.TransferMode = ConvertTransferMode(transferMode);

                // open the session
                session.Open(sessionOptions);

                // the local destination for the downloaded files needs to be a directory.
                //  (must end in "\").
                if (!localDir.EndsWith(@"\"))
                    localDir = String.Format("{0}\\", localDir);

                foreach (string fileName in fileNames)
                {
                    TransferOperationResult transferResult =
                        session.GetFiles(Path.Combine(GetRelativePath(uri), fileName), localDir, removeSourceFiles, transferOptions);

                    // will throw exception with first error encountered.
                    transferResult.Check();
                }
            }
        }

        /// <summary>
        /// Retreive a file or files from an FTP or SFTP site.  If there is a file with
        /// the same name in the target directory, it will be overwritten.  (See overload 
        /// for more info about parameters).
        /// </summary>
        /// <param name="remoteUrl"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="fileNameOrWildcard">A single filename or a wildcard pattern indicating which files
        /// to download.  Ex: fileToGet.xml, *.xml</param>
        /// <param name="localDir"></param>
        /// <param name="remoteDir"></param>
        /// <param name="winSCPexePath"></param>
        /// <param name="hostKey"></param>
        /// <param name="sslRequired"></param>
        /// <param name="transferMode"></param>
        /// <param name="timeoutInSeconds"></param>
        /// <param name="removeSourceFiles"></param>
        public void GetFiles(string remoteUrl, string userName, string password,
            string fileNameOrWildcard, string localDir, string remoteDir, string winSCPexePath,
            string hostKey, bool sslRequired, FTPConst.TransferMode transferMode, int? timeoutInSeconds, bool removeSourceFiles)
        {
            List<string> fileNames = new List<string>();
            fileNames.Add(fileNameOrWildcard);
            GetFiles(remoteUrl, userName, password, fileNames, localDir, remoteDir, winSCPexePath, hostKey, sslRequired, transferMode, timeoutInSeconds, removeSourceFiles);
        }

        /// <summary>
        /// Upload files to an FTP or SFTP site.  If a file exists on the S/FTP server
        /// with the same filename, it will be overwritten.  (See GetFiles for more info 
        /// on the parameters).
        /// </summary>
        /// <param name="remoteUrl"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="fileNames"></param>
        /// <param name="localDir">The source directory of the files to upload.</param>
        /// <param name="remoteDir"></param>
        /// <param name="winSCPexePath"></param>
        /// <param name="hostKey"></param>
        /// <param name="sslRequired"></param>
        /// <param name="transferMode"></param>
        /// <param name="timeoutInSeconds"></param>
        /// <param name="removeSourceFiles">Remove a file from the source directory after it is uploaded.</param>
        public void PutFiles(string remoteUrl, string userName, string password,
            List<string> fileNames, string localDir, string remoteDir, string winSCPexePath,
            string hostKey, bool sslRequired, FTPConst.TransferMode transferMode, int? timeoutInSeconds, bool removeSourceFiles)
        {
            Uri uri = GetUri(remoteUrl, remoteDir);
            SessionOptions sessionOptions = CreateSessionOptions(uri, userName, password, hostKey, sslRequired);
            using (Session session = CreateSession(sessionOptions, winSCPexePath, timeoutInSeconds))
            {
                TransferOptions transferOptions = new TransferOptions();
                transferOptions.PreserveTimestamp = false;  // timestamp on server should not reflect that of source file.
                transferOptions.TransferMode = ConvertTransferMode(transferMode);

                // open the session
                session.Open(sessionOptions);

                foreach(string fileName in fileNames)
                {
                    string localPath = fileName;
                    if (!String.IsNullOrEmpty(localDir))
                        localPath = Path.Combine(localDir, localPath);
                    TransferOperationResult transferResult
                        = session.PutFiles(localPath, GetRelativePath(uri), removeSourceFiles, transferOptions);

                    // will throw exception with first error encountered.
                    transferResult.Check();
                }
            }
        }

        /// <summary>
        /// Upload a file or files to an FTP or SFTP site.  If a file exists on the S/FTP server
        /// with the same filename, it will be overwritten.  (See GetFiles for more info 
        /// on the parameters).
        /// </summary>
        /// <param name="remoteUrl"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="fileNameOrWildcard"></param>
        /// <param name="localDir"></param>
        /// <param name="remoteDir"></param>
        /// <param name="winSCPexePath"></param>
        /// <param name="hostKey"></param>
        /// <param name="sslRequired"></param>
        /// <param name="transferMode"></param>
        /// <param name="timeoutInSeconds"></param>
        /// <param name="removeSourceFiles"></param>
        public void PutFiles(string remoteUrl, string userName, string password,
            string fileNameOrWildcard, string localDir, string remoteDir, string winSCPexePath,
            string hostKey, bool sslRequired, FTPConst.TransferMode transferMode, int? timeoutInSeconds, bool removeSourceFiles)
        {
            List<string> fileNames = new List<string>();
            fileNames.Add(fileNameOrWildcard);
            PutFiles(remoteUrl, userName, password, fileNames, localDir, remoteDir, winSCPexePath, hostKey, sslRequired, transferMode, timeoutInSeconds, removeSourceFiles);
        }

        /// <summary>
        /// Rename a file or move it to another location on the S/FTP server.
        /// An exception will be thrown if a file with the same name exists at
        /// the target location.
        /// </summary>
        /// <param name="remoteUrl"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="from">Relative path (incl filename) to the file that will be moved or renamed.  If the
        /// remoteUrl contains the full path to the source directory, this will just be the filename.</param>
        /// <param name="to">New filename or relative path (incl filename) to target directory if moving a file.</param>
        /// <param name="winSCPexePath"></param>
        /// <param name="hostKey"></param>
        /// <param name="sslRequired"></param>
        /// <param name="timeoutInSeconds"></param>
        /// <param name="createMissingDestDirs">If true, and the full path in the "to" param doesn't
        /// exist, it will attempt to create the missing dirs in that path.  Acount must have rights 
        /// to do create directories on the FTP server.</param>
        public void RenameOrMoveFile(string remoteUrl, string userName, string password,
            string from, string to, string winSCPexePath,string hostKey, bool sslRequired, int? timeoutInSeconds, 
            bool createMissingDestDirs)
        {
            string fromDir = Path.GetDirectoryName(from);
            string fromFileName = Path.GetFileName(from);
            string toDir = Path.GetDirectoryName(to);
            string toFileName = Path.GetFileName(to);

            Uri fromUri = GetUri(remoteUrl, fromDir);
            Uri toUri = GetUri(remoteUrl, toDir);

            SessionOptions sessionOptions = CreateSessionOptions(fromUri, userName, password, hostKey, sslRequired);
            using (Session session = CreateSession(sessionOptions, winSCPexePath, timeoutInSeconds))
            {
                // open the session
                session.Open(sessionOptions);

                // if the full path in "to" doesn't exist and createMissingDestDirs 
                //  is true, attempt to create the missing directory/ies.
                //  If createMissingDestDirs == false and the path doesn't exist, 
                //  just let session.MoveFile throw the exception.
                if (createMissingDestDirs && toUri.LocalPath != "/" && !session.FileExists(toUri.LocalPath))
                {
                    int locDirExists = -1;
                    // Start with the full path and find the first existing
                    //  subdir.  Stop before segment 0, which is just "/".
                    //  We don't want to allow creating dirs at the root of
                    //  the host.  If an existing directory is found, save the 
                    //  location of that dir in the Segments array.  Note that 
                    //  loc isn't actually an index, it refers to the size of
                    //  the Segments array as we navigate up the directory 
                    //  hierarchy.  locDirExists will be the size of the Segments
                    //  array where the first existing directory is the last element.
                    for (int loc = toUri.Segments.Length - 1; loc > 1; loc--)
                    {
                        string[] subPathSegs = new string[loc];
                        Array.Copy(toUri.Segments, subPathSegs, loc);
                        string subPath = String.Join("", subPathSegs);
                        // removing trailing slash from dir when checking for
                        //  the existence of a dir.  Not sure why, but with FTP, the
                        //  directory was not if it had a trailing slash.  SFTP was though.
                        //  Fortunately, both work with the trailing slash removed.
                        if (session.FileExists(subPath.Substring(0, subPath.Length - 1)))
                        {
                            locDirExists = loc;
                            break;
                        }
                    }

                    // if an existing dir was found, start creating dirs from
                    //  idxDirExists + 1 until the full path is created.
                    // If not found, just let session.MoveFile throw the exception.
                    if (locDirExists > -1)
                    {
                        for (int loc = locDirExists + 1; loc <= toUri.Segments.Length; loc++)
                        {
                            string[] subPathSegs = new string[loc];
                            Array.Copy(toUri.Segments, subPathSegs, loc);
                            string subPath = String.Join("", subPathSegs);
                            session.CreateDirectory(subPath);
                        }
                    }
                }

                // move/rename the file
                session.MoveFile(Path.Combine(GetRelativePath(fromUri), fromFileName), 
                    Path.Combine(GetRelativePath(toUri), toFileName));
            }
        }

        /// <summary>
        /// Rename a file or move it to another location on the S/FTP server.
        /// An exception will be thrown if a file with the same name exists at
        /// the target location.  Will not create any missing directories in the "to" path.
        /// </summary>
        /// <param name="remoteUrl"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="winSCPexePath"></param>
        /// <param name="hostKey"></param>
        /// <param name="sslRequired"></param>
        /// <param name="timeoutInSeconds"></param>
        public void RenameOrMoveFile(string remoteUrl, string userName, string password,
           string from, string to, string winSCPexePath, string hostKey, bool sslRequired, int? timeoutInSeconds)
        {
            RenameOrMoveFile(remoteUrl, userName, password, from, to, winSCPexePath, hostKey, sslRequired, timeoutInSeconds, false);
        }

        /// <summary>
        /// List files in a directory on an S/FTP site.  The list that's returned will include only 
        /// qualifying files (based on fileNameRegExPattern and createdOnOrAfter) and will include
        /// directories only if includeDirectories = true.  Note that the WinSCP.dll does not support
        /// wildcards (ex: sftp.drcsurveys.com/Usr/AIR/SBAC014/TestFiles/*.xml), so the fileNameRegExPattern
        /// will have to be used to return only certain file types, or WinSCPWrapper.GetFileListDetails
        /// can be used.
        /// </summary>
        /// <param name="remoteUrl">The full path to the directory to list.</param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="winSCPexePath"></param>
        /// <param name="hostKey"></param>
        /// <param name="sslRequired"></param>
        /// <param name="timeoutInSeconds"></param>
        /// <param name="fileNameRegExPattern">This method will only return files that match this 
        /// regular expression.  If blank/null, it will return all files.</param>
        /// <param name="createdOnOrAfter">This method will only return files where the last write
        /// datetime is >= this value.  If null, it will return all files.</param>
        /// <param name="includeDirectories">If true, it will also return directories.  
        /// fileNameRegExPattern and createdOnOrAfter will not be applied to directories.</param>
        /// <returns></returns>
        public List<RemoteFile> GetFileListDetails(string remoteUrl, string userName, string password,
            string winSCPexePath, string hostKey, bool sslRequired, int? timeoutInSeconds, 
            string fileTypeWildcard, string fileNameRegExPattern, DateTime? createdOnOrAfter, bool includeDirectories)
        {
            List<RemoteFile> files = new List<RemoteFile>();
            Uri uri = GetUri(remoteUrl, null);
            SessionOptions sessionOptions = CreateSessionOptions(uri, userName, password, hostKey, sslRequired);
            using (Session session = CreateSession(sessionOptions, winSCPexePath, timeoutInSeconds))
            {
                // open the session and retreive the file list
                session.Open(sessionOptions);
                string path = GetRelativePath(uri);
                //TODO: this is not working.  ListDirectory method appears to append a "/" to the 
                //  end of the string, so the call errors out.  Ex: ls "/dir/subdir/*.xml/".
                //  I've posted the bug to WinSCP's support forum.
                //if (!String.IsNullOrEmpty(fileTypeWildcard))
                //    path = Path.Combine(path, fileTypeWildcard);
                RemoteDirectoryInfo directory = session.ListDirectory(path);
                foreach (RemoteFileInfo fileInfo in directory.Files)
                {
                    if ((fileInfo.IsDirectory && includeDirectories)
                        ||
                        (!fileInfo.IsDirectory
                        && (createdOnOrAfter == null || fileInfo.LastWriteTime >= createdOnOrAfter.Value)
                        && (String.IsNullOrEmpty(fileNameRegExPattern) || Regex.IsMatch(fileInfo.Name, fileNameRegExPattern, RegexOptions.Singleline))))
                    {
                        files.Add(RemoteFile.Create(fileInfo.IsDirectory, fileInfo.LastWriteTime, fileInfo.Name, fileInfo.Length));
                    }
                }
            }
            return files;
        }

        /// <summary>
        /// Lists files in a directory on an S/FTP site.  The list that's returned will include only 
        /// qualifying files (based on fileNameRegExPattern and createdOnOrAfter).  This overload will
        /// not include directories, which is the default behaviour of WinSCPWrapper.GetFileListDetails.
        /// Note that the WinSCP.dll does not support wildcards (ex: sftp.drcsurveys.com/Usr/AIR/SBAC014/TestFiles/*.xml), 
        /// so the fileNameRegExPattern will have to be used to return only certain file types, or
        /// WinSCPWrapper.GetFileListDetails can be used.
        /// </summary>
        /// <param name="remoteUrl"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="winSCPexePath"></param>
        /// <param name="hostKey"></param>
        /// <param name="sslRequired"></param>
        /// <param name="timeoutInSeconds"></param>
        /// <param name="fileNameRegExPattern"></param>
        /// <param name="createdOnOrAfter"></param>
        /// <returns></returns>
        public List<RemoteFile> GetFileListDetails(string remoteUrl, string userName, string password,
            string winSCPexePath, string hostKey, bool sslRequired, int? timeoutInSeconds,
            string fileTypeWildcard, string fileNameRegExPattern, DateTime? createdOnOrAfter)
        {
            return GetFileListDetails(remoteUrl, userName, password, winSCPexePath, hostKey, sslRequired,
                timeoutInSeconds, fileTypeWildcard, fileNameRegExPattern, createdOnOrAfter, false);
        }

        /// <summary>
        /// Takes a path on the ftp server to a file or directory and returns true
        /// if that file or directory exists.
        /// </summary>
        /// <param name="path">Full path to file or directory on remote S/FTP site</param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="winSCPexePath"></param>
        /// <param name="hostKey"></param>
        /// <param name="sslRequired"></param>
        /// <param name="timeoutInSeconds"></param>
        /// <returns></returns>
        public bool FileOrDirectoryExists(string path, string userName, string password,
            string winSCPexePath, string hostKey, bool sslRequired, int? timeoutInSeconds)
        {
            bool exists = false;
            Uri uri = GetUri(Path.GetDirectoryName(path), null);
            string fileName = Path.GetFileName(path);
            SessionOptions sessionOptions = CreateSessionOptions(uri, userName, password, hostKey, sslRequired);
            using (Session session = CreateSession(sessionOptions, winSCPexePath, timeoutInSeconds))
            {
                session.Open(sessionOptions);
                if (String.IsNullOrEmpty(fileName))
                    // Not sure why, but FTP could not find the directory with the trailing slash.
                    //  SFTP was fine.  However, both work if the trailing slash is removed.  Note
                    //  that because we're using GetUri, the directory path is guaranteed to have
                    //  a trailing slash.
                    exists = session.FileExists(uri.LocalPath.Substring(0, uri.LocalPath.Length - 1));
                else
                    exists = session.FileExists(Path.Combine(uri.LocalPath, fileName));
            }
            return exists;
        }

        #region "private methods"

        private Uri GetUri(string remoteUrl, string remoteDir)
        {
            // so that we don't have to worry about calling methods on 
            //  a null remoteDir
            remoteDir = remoteDir ?? "";

            // make sure we've got forward slashes.  If the remoteUrl contains a protocol but
            //  the slashes were reversed to back-slashes by using Path.GetDirectoryName, make 
            //  sure the protocol is intact after the first replace.  Since \ is considered an 
            //  escape char, ftp:\\ will end up as ftp:/ after the first replace.
            remoteUrl = remoteUrl.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            if(!remoteUrl.Contains("://"))
                remoteUrl = remoteUrl.Replace(Path.VolumeSeparatorChar.ToString(), ":/");
            remoteDir = remoteDir.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            // Uri ctor below doesn't work if remoteDir has a / in front.  That seems to
            //  imply the root.  Also, remove the / from the remoteUrl if it's there.
            if (remoteUrl.StartsWith("/"))
                remoteUrl = remoteUrl.Substring(1);
            if (remoteDir.StartsWith("/"))
                remoteDir = remoteDir.Substring(1);

            // also make sure that any directories end in /
            if (!(String.IsNullOrEmpty(remoteUrl) || remoteUrl.EndsWith("/")))
                remoteUrl = String.Format("{0}/", remoteUrl);
            if (!(String.IsNullOrEmpty(remoteDir) || remoteDir.EndsWith("/")))
                remoteDir = String.Format("{0}/", remoteDir);

            // getting the host and local paths are not supported
            //  for relative Uri's, so we need to convert to an absolute
            //  path, which requires the protocol.  Generally, we specify the protocol
            //  for ftp but not for sftp, so if there's no protocol on the URL, we're 
            //  going to assume SFTP.
            if (!remoteUrl.Contains(@"://"))
                remoteUrl = String.Format(@"sftp://{0}", remoteUrl);

            // create a Uri from the remoteUrl
            Uri uri = new Uri(remoteUrl);

            // create a new Uri appending the remoteDir
            if(!String.IsNullOrEmpty(remoteDir))
                uri = new Uri(uri, remoteDir);

            return uri;
        }

        private SessionOptions CreateSessionOptions(Uri uri, string userName, string password, string hostKey, bool sslRequired)
        {
            SessionOptions sessionOptions = new SessionOptions();
            sessionOptions.HostName = uri.DnsSafeHost;
            sessionOptions.UserName = userName;
            sessionOptions.Password = password;

            if (uri.Scheme.Equals("sftp", StringComparison.InvariantCultureIgnoreCase))
            {
                sessionOptions.Protocol = Protocol.Sftp;
                
                // if a host key was provided, use it.  If not provided, the host key
                //  must be cached in the exe's storage (ini file preferable).
                if(!String.IsNullOrEmpty(hostKey))
                    sessionOptions.SshHostKeyFingerprint = hostKey;
            }
            else if (uri.Scheme.StartsWith("ftp", StringComparison.InvariantCultureIgnoreCase))
            {
                sessionOptions.Protocol = Protocol.Ftp;
                if (sslRequired)
                {
                    //TODO: ever use implicit?  The only FTP-over-SSL site we use currently
                    //  is Oregon's client site, and they require explicit.
                    sessionOptions.FtpSecure = FtpSecure.ExplicitSsl;
                    if (!String.IsNullOrEmpty(hostKey))
                        sessionOptions.TlsHostCertificateFingerprint = hostKey;
                }
            }
            else
            {
                throw new ApplicationException(String.Format("Unsupported protocol: {0}", uri.Scheme));
            }

            // set the port if one has been specified in the url
            if(uri.Port != NO_PORT &&
                ((sessionOptions.Protocol == Protocol.Ftp && uri.Port != DEDAULT_FTP_PORT)
                    || (sessionOptions.Protocol == Protocol.Sftp && uri.Port != DEDAULT_SFTP_PORT)))
            {
                sessionOptions.PortNumber = uri.Port;
            }

            // set keepalives if configured.  Not supported directly through the API yet.
            //  See http://winscp.net/forum/viewtopic.php?t=11479
            //  Also, http://winscp.net/eng/docs/rawsettings
            if (KeepAliveInterval > 0)
            {
                sessionOptions.AddRawSettings("PingType", "2"); //0 = Off, 1 = Sending of null SSH packets, 2 = Executing dummy protocol commands
                sessionOptions.AddRawSettings("PingIntervalSecs", KeepAliveInterval.ToString());
                // WinSCP site admin recommends this, but I don't see it in the list of documented commands.  
                //    Will try it anyway.  The more the merrier.
                sessionOptions.AddRawSettings("FtpPingType", "2");
                sessionOptions.AddRawSettings("FtpPingInterval", KeepAliveInterval.ToString());
            }

            return sessionOptions;
        }

        private Session CreateSession(SessionOptions sessionOptions, string winSCPexePath, int? timeoutInSeconds)
        {
            Session session = new Session();

            // for debuggin winscp...
            if (!String.IsNullOrEmpty(WinSCPDebugLogFilePath))
                session.DebugLogPath = WinSCPDebugLogFilePath;

            // The WinSCP.dll is supposed to be able to find WinSCP.exe in the bin directory
            //  of the current app, or in the installation directory.  Unfortunately, the installation
            //  directory appears to be hardcoded to C:\Program Files\WinSCP\, but on Windows 7 the
            //  path is actually C:\Program Files (x86)\WinSCP\.  So we still have to set the path
            //  here even when WinSCP is installed on the server.  It must be the full path
            //  including the filename.  Ex: "C:\Program Files (x86)\WinSCP\WinSCP.exe".  If we use
            //  the portable approach with the exe in the app's bin dir, this path will not need to be set.
            //
            //  Also, unlike the WinSCPWrapper, the dll requires the winscp exe, not the com.  
            //  Just in case the config setting specifies the com, we'll change to exe.
            if (!String.IsNullOrEmpty(winSCPexePath))
                session.ExecutablePath =
                    Regex.Replace(winSCPexePath, WINSCP_COM, WINSCP_EXE,
                        RegexOptions.IgnoreCase | RegexOptions.RightToLeft | RegexOptions.Singleline);

            // if using SFTP or Explicit SSL-over-FTP and no host key was provided, the host key must have been
            //  pre-cached in the executable's ini file.  Not sure why, but in order to
            //  use the exe's cache, we need to set this property to false.
            if ((sessionOptions.Protocol == Protocol.Sftp && String.IsNullOrEmpty(sessionOptions.SshHostKeyFingerprint))
                || (sessionOptions.Protocol == Protocol.Ftp && sessionOptions.FtpSecure == FtpSecure.ExplicitSsl && String.IsNullOrEmpty(sessionOptions.TlsHostCertificateFingerprint)))
                session.DefaultConfiguration = false;

            // if a timeout was specified, configure it.  Otherwise, leave
            //  the default value (60 seconds).
            if ((timeoutInSeconds ?? -1) > -1)
                session.Timeout = new TimeSpan(0, 0, timeoutInSeconds.Value);

            return session;
        }

        private TransferMode ConvertTransferMode(FTPConst.TransferMode transferMode)
        {
            TransferMode retVal = TransferMode.Automatic;
            switch (transferMode)
            {
                case FTPConst.TransferMode.Auto:
                    retVal = TransferMode.Automatic;
                    break;
                case FTPConst.TransferMode.Binary:
                    retVal = TransferMode.Binary;
                    break;
                case FTPConst.TransferMode.Text:
                    retVal = TransferMode.Ascii;
                    break;
            }
            return retVal;
        }

        /// <summary>
        /// Returns the uri's path as a relative path, w/o scheme, host name, or query portion of the URI.
        /// </summary>
        /// <param name="uriPath"></param>
        /// <returns></returns>
        private string GetRelativePath(Uri uri)
        {
            // This is the full absolute path from the host.  If the host IS the
            //  full path, it'll just be "/".  Prefix a "." in order to make this
            //  a relative path.
            return String.Format(".{0}", uri.AbsolutePath);
        }

        #endregion
    }
}
