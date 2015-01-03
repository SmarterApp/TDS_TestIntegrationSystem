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
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace CommonUtilities
{
	public class WinSCPWrapper
	{
		private static string winscpOutput = "";
		private static string winscpError = "";

        /// <summary>
        /// Downloads the specified files from a remote SFTP server to a local directory using WinSCP
        /// </summary>
        /// <param name="winSCPexe">The local path to the WinSCP executable</param>
        /// <param name="remoteUrl">The URL of the remote SFTP server.  Can include the remote directory as well.
        /// Ex: 174.143.51.37/_datafiles/ext_vendors_sftp/Minnesota/Testing/Extracts
        /// Or: 174.143.51.37</param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="fileNames">A list of filenames to download</param>
        /// <param name="localDir">The local directory that the files will be downloaded to</param>
        /// <param name="remoteDir">The remote dir that the files should be downloaded from.
        /// Path.Combine(remoteUrl, remoteDir) represents the source directory.  This param 
        /// can be null or String.Empty if remoteUrl contains the full path.</param>
        /// <returns></returns>
		public static bool SFTPGetFiles(string winSCPexe, string remoteUrl, string userName, string password,
			List<string> fileNames, string localDir, string remoteDir)
		{
			return SFTPDoFiles(winSCPexe, remoteUrl, userName, password, fileNames, localDir, remoteDir, "get", false);
		}

        /// <summary>
        /// Downloads files that match the wildcard string from a remote SFTP server to a local directory using WinSCP.
        /// </summary>
        /// <param name="winSCPexe">The local path to the WinSCP executable</param>
        /// <param name="remoteUrl">The URL of the remote SFTP server.  Can include the remote directory as well.
        /// Ex: 174.143.51.37/_datafiles/ext_vendors_sftp/Minnesota/Testing/Extracts
        /// Or: 174.143.51.37</param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="fileNameWithWildcard">Ex: Extract*.xml will download all matching files.  Downloaded
        /// files will have the same names as in the source dir.</param>
        /// <param name="localDir">The local directory that the files will be downloaded to</param>
        /// <param name="remoteDir">The remote dir that the files should be downloaded from.
        /// Path.Combine(remoteUrl, remoteDir) represents the source directory.  This param 
        /// can be null or String.Empty if remoteUrl contains the full path.</param>
        /// <returns></returns>
        public static bool SFTPGetFiles(string winSCPexe, string remoteUrl, string userName, string password,
            string fileNameWithWildcard, string localDir, string remoteDir)
        {
            List<string> fileNames = new List<string>();
            fileNames.Add(fileNameWithWildcard);
            return SFTPDoFiles(winSCPexe, remoteUrl, userName, password, fileNames, localDir, remoteDir, "get", true);
        }

		/// <summary>
		/// Send files to an SFTP site using WinSCP
		/// </summary>
		/// <param name="winSCPexe">The location of the WinSCP executable.</param>
        /// <param name="remoteUrl">The URL of the remote SFTP server.  Can include the remote directory as well.
        /// Ex: 174.143.51.37/_datafiles/ext_vendors_sftp/Minnesota/Testing/Extracts
        /// Or: 174.143.51.37</param>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		/// <param name="fileNames">A list of filenames to upload</param>
		/// <param name="localDir">The local dir that contains the filenames to upload</param>
        /// <param name="remoteDir">The remote dir that the files should be uploaded to.
        /// Path.Combine(remoteUrl, remoteDir) represents the source directory.  This param 
        /// can be null or String.Empty if remoteUrl contains the full path.</param>
		/// <returns></returns>
		public static bool SFTPPutFiles(string winSCPexe, string remoteUrl, string userName, string password,
			List<string> fileNames, string localDir, string remoteDir)
		{
			return SFTPDoFiles(winSCPexe, remoteUrl, userName, password, fileNames, localDir, remoteDir, "put", false);
		}

        /// <summary>
        /// Send files matching a wildcard string to an SFTP site using WinSCP
        /// </summary>
        /// <param name="winSCPexe">The location of the WinSCP executable.</param>
        /// <param name="remoteUrl">The URL of the remote SFTP server.  Can include the remote directory as well.
        /// Ex: 174.143.51.37/_datafiles/ext_vendors_sftp/Minnesota/Testing/Extracts
        /// Or: 174.143.51.37</param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="fileNameWithWildcard">Ex: Extract*.xml will upload all matching files.
        /// Uploaded files will have the same names as in the source dir.</param>
        /// <param name="localDir">The local dir that contains the filenames to upload</param>
        /// <param name="remoteDir">The remote dir that the files should be uploaded to.
        /// Path.Combine(remoteUrl, remoteDir) represents the source directory.  This param 
        /// can be null or String.Empty if remoteUrl contains the full path.</param>
        /// <returns></returns>
        public static bool SFTPPutFiles(string winSCPexe, string remoteUrl, string userName, string password,
            string fileNameWithWildcard, string localDir, string remoteDir)
        {
            List<string> fileNames = new List<string>();
            fileNames.Add(fileNameWithWildcard);
            return SFTPDoFiles(winSCPexe, remoteUrl, userName, password, fileNames, localDir, remoteDir, "put", true);
        }
        /// <summary>
        /// Send files to an SFTP site using WinSCP
        /// </summary>
        /// <param name="winSCPexe">The location of the WinSCP executable.</param>
        /// <param name="remoteUrl">The URL of the remote SFTP server.  Can include the remote directory as well.
        /// Ex: 174.143.51.37/_datafiles/ext_vendors_sftp/Minnesota/Testing/Extracts
        /// Or: 174.143.51.37</param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="hostKey"></param>
        /// <param name="fileNames">A list of filenames to upload</param>
        /// <param name="localDir">The local dir that contains the filenames to upload</param>
        /// <param name="remoteDir">The remote dir that the files should be uploaded to.
        /// Path.Combine(remoteUrl, remoteDir) represents the source directory.  This param 
        /// can be null or String.Empty if remoteUrl contains the full path.</param>
        /// <returns></returns>
        public static bool SFTPPutFiles(string winSCPexe, string remoteUrl, string userName, string password, string hostKey,
            List<string> fileNames, string localDir, string remoteDir)
        {
            return SFTPDoFiles(winSCPexe, remoteUrl, userName, password, hostKey, fileNames, localDir, remoteDir, "put", false);
        }
        /// <summary>
		/// 
		/// </summary>
		/// <param name="winSCPexe"></param>
		/// <param name="remoteUrl"></param>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		/// <param name="fileNames"></param>
		/// <param name="localDir"></param>
		/// <param name="remoteDir"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		private static bool SFTPDoFiles(string winSCPexe, string remoteUrl, string userName, string password,
			List<string> fileNames, string localDir, string remoteDir, string action, bool useWildcard)
		{
            Process winscp = CreateAndOpenWinSCPProcess(winSCPexe, remoteUrl, userName, password);

            // If the remoteUrl contains the full path to the remote directory 
            //  and the remoteDir param is null or Empty, we need to make the 
            //  remoteDir relative to the current remoteUrl
            if (String.IsNullOrEmpty(remoteDir))
                remoteDir = ".";

			foreach (string filename in fileNames)
			{
                string path = remoteDir;
                string localPath = localDir;
                string doString = "";

                switch (action)
                {
                    case "put":
                        localPath = Path.Combine(localDir, filename);
                        if (useWildcard)
                        {
                            // if using a wildcard for a put, the remote destination for
                            //  the uploaded files need to be a directory.  In this case, 
                            //  it's required that the remote path ends with a "/"
                            if (!path.EndsWith("/"))
                                path = String.Format("{0}/", path);
                        }
                        else
                        {
                            // not using wildcard; write the file to the remote dir
                            //  using the same filename as the local source
                            path = Path.Combine(remoteDir, filename);
                        }
                        // make sure that the remote path uses forward slashes instead of back slashes
                        path = path.Replace('\\', '/');
                        doString = "put \"" + localPath + "\" \"" + path + "\"";
                        break;
                    case "get":
                        path = Path.Combine(remoteDir, filename);
                        // make sure that the remote path uses forward slashes instead of back slashes
                        path = path.Replace('\\', '/');
                        if (useWildcard)
                        {
                            // if using a wildcard for the get, the local destination for
                            //  the downloaded files needs to be a directory.  In this case, 
                            //  it's required that the local path ends in "\".
                            if (!localPath.EndsWith(@"\"))
                                localPath = String.Format("{0}\\", localPath);
                        }
                        else
                        {
                            // not using a wildcard; write the file to the local dir
                            //  using the same filename as the remote source
                            localPath = Path.Combine(localDir, filename);
                        }
                        doString = "get \"" + path + "\" \"" + localPath + "\"";
                        break;
                }

				Console.WriteLine("Doing: " + doString);
				winscp.StandardInput.WriteLine(doString);
			}

            ExitAndCloseWinSCP(winscp);

			return true;
		}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="winSCPexe"></param>
        /// <param name="remoteUrl"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="hostKey"></param>
        /// <param name="fileNames"></param>
        /// <param name="localDir"></param>
        /// <param name="remoteDir"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        private static bool SFTPDoFiles(string winSCPexe, string remoteUrl, string userName, string password, string hostKey,
            List<string> fileNames, string localDir, string remoteDir, string action, bool useWildcard)
        {
            Process winscp = CreateAndOpenWinSCPProcess(winSCPexe, remoteUrl, userName, password, hostKey);

            // If the remoteUrl contains the full path to the remote directory 
            //  and the remoteDir param is null or Empty, we need to make the 
            //  remoteDir relative to the current remoteUrl
            if (String.IsNullOrEmpty(remoteDir))
                remoteDir = ".";

            foreach (string filename in fileNames)
            {
                string path = remoteDir;
                string localPath = localDir;
                string doString = "";
                FileInfo fi = new FileInfo(filename);
                switch (action)
                {
                    case "put":
                        localPath = Path.Combine(localDir, filename);
                        if (useWildcard)
                        {
                            // if using a wildcard for a put, the remote destination for
                            //  the uploaded files need to be a directory.  In this case, 
                            //  it's required that the remote path ends with a "/"
                            if (!path.EndsWith("/"))
                                path = String.Format("{0}/", path);
                        }
                        else
                        {
                            // not using wildcard; write the file to the remote dir
                            //  using the same filename as the local source
                            path = Path.Combine(remoteDir, fi.Name);
                        }
                        // make sure that the remote path uses forward slashes instead of back slashes
                        path = path.Replace('\\', '/');
                        doString = "put \"" + localPath + "\" \"" + path + "\"";
                        break;
                    case "get":
                        path = Path.Combine(remoteDir, filename);
                        // make sure that the remote path uses forward slashes instead of back slashes
                        path = path.Replace('\\', '/');
                        if (useWildcard)
                        {
                            // if using a wildcard for the get, the local destination for
                            //  the downloaded files needs to be a directory.  In this case, 
                            //  it's required that the local path ends in "\".
                            if (!localPath.EndsWith(@"\"))
                                localPath = String.Format("{0}\\", localPath);
                        }
                        else
                        {
                            // not using a wildcard; write the file to the local dir
                            //  using the same filename as the remote source
                            localPath = Path.Combine(localDir, filename);
                        }
                        doString = "get \"" + path + "\" \"" + localPath + "\"";
                        break;
                }

                Console.WriteLine("Doing: " + doString);
                winscp.StandardInput.WriteLine(doString);
            }

            ExitAndCloseWinSCP(winscp);

            return true;
        }

        /// <summary>
        /// Rename a file, move a file, or move any files matching a wildcard expression.
        /// </summary>
        /// <param name="winSCPexe">Local path to the WinSCP.com application</param>
        /// <param name="remoteUrl">remote path to the directory containing the file(s) to rename or move.
        /// Ex: 174.143.51.37/_datafiles/ext_vendors_sftp/Minnesota/Testing/Extracts </param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="filename">Filename in this directory to rename or move (or both).
        /// The filename can contain a wildcard (ex: "Extract*.xml").  In this case, the "to"
        /// param must be a directory ending in "/" and no renaming is possible</param>
        /// <param name="to">A new filename if renaming a single file.  A new filename with a
        /// relative path if moving and renaming a single file (ex: "./Processed/newfilename.xml").
        /// A relative path if moving a file or moving files matching a wildcard expression 
        /// (ex: ./Processed/").  Note that if this param is a directory, it must end in a "/".</param>
        public static void RenameOrMoveFile(string winSCPexe, string remoteUrl, string userName, string password,
            string filename, string to)
        {
            // make sure forward slashes are used if a path is provided
            Process winscp = CreateAndOpenWinSCPProcess(winSCPexe, remoteUrl, userName, password);
            to = to.Replace("\\", "/");
            winscp.StandardInput.WriteLine(String.Format("mv \"{0}\" \"{1}\"", filename, to));
            ExitAndCloseWinSCP(winscp);
        }

        /// <summary>
        /// Lists files on the SFTP server using WinSCP and returns a list of RemoteFile objects
        /// </summary>
        /// <param name="winSCPexe">Local path to the the WinSCP executable</param>
        /// <param name="remoteUrl">Full path to the directory that will be listed</param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="fileTypeWildcard">To list only xml files, can pass *.xml</param>
        /// <param name="fileNameRegExPattern">For specific matching of filenames, can use this to filter results</param>
        /// <param name="createdOnOrAfter">Can also filter results by the create date on the server.</param>
        /// <returns></returns>
        public static List<RemoteFile> GetFileListDetails(string winSCPexe, string remoteUrl, string userName, string password,
            string fileTypeWildcard, string fileNameRegExPattern, DateTime? createdOnOrAfter)
        {
            List<RemoteFile> files = new List<RemoteFile>();

            Process winscp = CreateAndOpenWinSCPProcess(winSCPexe, remoteUrl, userName, password);

            string listDir = "./";
            if (!String.IsNullOrEmpty(fileTypeWildcard))
                listDir = Path.Combine(listDir, fileTypeWildcard);
            listDir = listDir.Replace("\\", "/"); // make sure the path has forward slashes

            winscp.StandardInput.WriteLine(String.Format("ls {0}", listDir));

            ExitAndCloseWinSCP(winscp);

            string[] lines = winscpOutput.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                RemoteFile f = RemoteFile.Create(line);
                if (f != null
                    && !f.IsDirectory
                    && (createdOnOrAfter == null || f.CreateDate >= createdOnOrAfter.Value)
                    && (String.IsNullOrEmpty(fileNameRegExPattern) || Regex.IsMatch(f.Name, fileNameRegExPattern, RegexOptions.Singleline)))
                {
                    files.Add(f);
                }
            }

            return files;
        }

        #region "Open and close WinSCP Process"

        private static Process CreateAndOpenWinSCPProcess(string winSCPexe, string remoteUrl, string userName, string password)
        {
            Process winscp = new Process();
            winscp.StartInfo.FileName = winSCPexe;
            winscp.StartInfo.UseShellExecute = false;

            winscp.StartInfo.RedirectStandardOutput = true;
            winscpOutput = "";
            winscp.StartInfo.RedirectStandardError = true;
            winscpError = "";

            // Set our event handlers to asynchronously read the output.
            winscp.OutputDataReceived += new DataReceivedEventHandler(winscp_OutputDataReceived);
            winscp.ErrorDataReceived += new DataReceivedEventHandler(winscp_ErrorDataReceived);

            winscp.StartInfo.RedirectStandardInput = true;
            winscp.StartInfo.CreateNoWindow = true;
            winscp.Start();

            winscp.BeginOutputReadLine();
            winscp.BeginErrorReadLine();

            winscp.StandardInput.WriteLine("option batch abort");
            winscp.StandardInput.WriteLine("option confirm off");
            string openSFTP = @"open sftp://" + userName + ":" + password + "@" + remoteUrl;
            winscp.StandardInput.WriteLine(openSFTP);

            return winscp;
        }

        private static void ExitAndCloseWinSCP(Process winscp)
        {
            winscp.StandardInput.Close();

            winscp.WaitForExit();
            if (winscp.ExitCode != 0)
            {
                string report = "WinSCP returned an exit code of " + winscp.ExitCode.ToString();
                winscp.Close();
                if (winscpError.Length > 0)
                    report += Environment.NewLine + "WinSCP error log:" + Environment.NewLine + winscpError;
                if (winscpOutput.Length > 0)
                    report += Environment.NewLine + "WinSCP transaction log:" + Environment.NewLine + winscpOutput;
                throw new Exception(report);
            }
            winscp.Close();

            if (winscpError.Length > 0)
            {
                string report = "WinSCP reported an error.";
                report += Environment.NewLine + "WinSCP error log:" + Environment.NewLine + winscpError;
                if (winscpOutput.Length > 0)
                    report += Environment.NewLine + "WinSCP transaction log:" + Environment.NewLine + winscpOutput;
                throw new Exception(report);
            }

            if (Regex.Match(winscpOutput, "[Ee]rror(s|) ").Success
                || Regex.Match(winscpOutput, " [Ee]rror(s|)").Success)
            {
                string report = "WinSCP logged an error.";
                report += Environment.NewLine + "WinSCP transaction log:" + Environment.NewLine + winscpOutput;
                throw new Exception(report);
            }
        }

        private static Process CreateAndOpenWinSCPProcess(string winSCPexe, string remoteUrl, string userName, string password, string hostKey)
        {
            Process winscp = new Process();
            winscp.StartInfo.FileName = winSCPexe;
            winscp.StartInfo.UseShellExecute = false;

            winscp.StartInfo.RedirectStandardOutput = true;
            winscpOutput = "";
            winscp.StartInfo.RedirectStandardError = true;
            winscpError = "";

            // Set our event handlers to asynchronously read the output.
            winscp.OutputDataReceived += new DataReceivedEventHandler(winscp_OutputDataReceived);
            winscp.ErrorDataReceived += new DataReceivedEventHandler(winscp_ErrorDataReceived);

            winscp.StartInfo.RedirectStandardInput = true;
            winscp.StartInfo.CreateNoWindow = true;
            winscp.Start();

            winscp.BeginOutputReadLine();
            winscp.BeginErrorReadLine();

            winscp.StandardInput.WriteLine("option batch abort");
            winscp.StandardInput.WriteLine("option confirm off");
            string openSFTP = @"open sftp://" + userName + ":" + password + "@" + remoteUrl;
            //string openSFTP = @"open sftp://" + userName + ":" + password + "@" + remoteUrl + " -hostkey=\"" + hostKey + "\"";
            winscp.StandardInput.WriteLine(openSFTP);

            return winscp;
        }

        #endregion

		static void winscp_OutputDataReceived(object sender, DataReceivedEventArgs outLine)
		{
			if (!String.IsNullOrEmpty(outLine.Data))
				winscpOutput += outLine.Data + Environment.NewLine;
		}

		static void winscp_ErrorDataReceived(object sender, DataReceivedEventArgs errorLine)
		{
			if (!String.IsNullOrEmpty(errorLine.Data))
				winscpOutput += errorLine.Data + Environment.NewLine;
		}
	}
}
