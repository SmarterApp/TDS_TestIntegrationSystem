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
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Globalization;
using CommonUtilities;

namespace CommonUtilities
{
	public class FTPClient
	{
		#region Properties
		private FtpWebRequest ftpRequest;
		public FtpWebRequest FtpRequest
		{
			get
			{
				return ftpRequest;
			}
		}

		private FtpWebResponse ftpResponse;
		public FtpWebResponse FtpResponse
		{
			get
			{
				return ftpResponse;
			}
		}

		private string serverURL = string.Empty;
		public string ServerURL
		{
			get
			{
				return serverURL;
			}
		}
		
		private string ftpUserName = string.Empty;
		public  string FtpUserName
		{
			get
			{
				return ftpUserName;
			}
		}

		private string ftpPassword = string.Empty;
		public string FtpPassword
		{
			get
			{
				return ftpPassword;
			}
		}

		private string ftpDomain = string.Empty;
		public string FtpDomain
		{
			get
			{
				return ftpDomain;
			}
		}


		#endregion

		/// <summary>
		/// Initializing the class with the URL, username and password information.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="username"></param>
		/// <param name="password"></param>
		public FTPClient(String url, String username, String password, String domain)
		{
			this.serverURL = url;
			this.ftpUserName = username;
			this.ftpPassword = password;
			this.ftpDomain = domain;
		}

		public FTPClient(String url, String username, String password)
		{
			this.serverURL = url;
			this.ftpUserName = username;
			this.ftpPassword = password;
			this.ftpDomain = string.Empty;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="remoteUrl"></param>
		/// <param name="localFile"></param>
		/// <param name="remoteFileNameAndPath"></param>
		/// <param name="sslRequired"></param>
		/// <returns></returns>
		public bool FtpExists(String remoteUrl, string remoteFileNameAndPath, bool sslRequired, out string errors)
		{
			byte[] remoteFileData = null;
			bool exists = false;
			errors = string.Empty;

			WebClient client = new WebClient();
			client.BaseAddress = ServerURL;
			client.Credentials = new NetworkCredential(FtpUserName, FtpPassword);

			if (sslRequired)
			{
				ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(MyCertValidationCb);
			}
			try
			{
				remoteFileData = client.DownloadData(remoteUrl + remoteFileNameAndPath);
			}
			catch (WebException e)
			{
				FtpWebResponse ftpWebResponse = ((FtpWebResponse)e.Response);
				string status = ftpWebResponse.StatusDescription;
				string stackTrace = e.StackTrace;
				if (status == null || status.Length == 0)
				{
					status = e.Message;
				}
				if (status.Contains("system cannot find the file specified"))
				{
					exists = false;
				}
				errors += status + "; " + stackTrace;
				return exists;
			}
			catch (Exception e)
			{
				exists = false;
				errors += e.Message + ";" + e.StackTrace;
				throw e;
			}
			if (remoteFileData.Length > 0)
			{
				exists = true;
				remoteFileData = null;
			}
			return exists;
		}


		/// <summary>
		/// Upload a file to the FTP Server directory
		/// </summary>
		/// <param name="remoteUrl"></param>
		/// <param name="fileName"></param>
		/// <param name="localDir"></param>
		/// <param name="sslRequired"></param>
		public bool FtpUploadFile(String remoteUrl, FileInfo fileNameAndPath, string remoteDirectory, bool sslRequired)
		{
			bool success = false;
			FileStream fileStream = null;
			Stream requestStream = null;

			ftpRequest = (FtpWebRequest)WebRequest.Create(remoteUrl + remoteDirectory + fileNameAndPath.Name);
			ftpRequest.Credentials = new NetworkCredential(ftpUserName, ftpPassword);
			ftpRequest.EnableSsl = sslRequired;
			ftpRequest.UsePassive = true;
			ftpRequest.Timeout = -1;
			ftpRequest.Proxy = null;
			ftpRequest.AuthenticationLevel = AuthenticationLevel.None;
			ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;

			if (sslRequired)
			{
				ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(MyCertValidationCb);
			}
			try
			{
				// This function will upload a file only if no other file with that name exists...
				requestStream = ftpRequest.GetRequestStream();
				fileStream = File.Open(fileNameAndPath.FullName, FileMode.Open);

				byte[] buffer = new byte[1024];
				int bytesRead;
				while (true)
				{
					bytesRead = fileStream.Read(buffer, 0, buffer.Length);
					if (bytesRead == 0)
						break;
					requestStream.Write(buffer, 0, bytesRead);
				}
				requestStream.Close();
				success = true;
			}
			catch (WebException e)
			{
				success = false;
                string status = String.IsNullOrEmpty(((FtpWebResponse)e.Response).StatusDescription) ? string.Empty : ((FtpWebResponse)e.Response).StatusDescription;
				if (status.Length == 0)
				{
					status = e.Message;
                    status += e.StackTrace;
				}
				throw new Exception(status, e);
			}
			catch (Exception e)
			{
				success = false;
				throw new Exception(e.Message, e);
			}
			finally
			{
				if (fileStream != null)
				{
					fileStream.Close();
				}
				if (requestStream != null)
				{
					requestStream.Close();
				}
				ftpRequest.Abort();
			}
			return success;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="remoteUrl"></param>
		/// <param name="localFile"></param>
		/// <param name="remoteFileNameAndPath"></param>
		/// <param name="sslRequired"></param>
		/// <returns></returns>
		public bool FtpUploadFileNoBuffer(String remoteUrl, FileInfo localFile, string remoteFileNameAndPath, bool sslRequired)
		{
			bool success = false;

			WebClient client = new WebClient();
			client.BaseAddress = ServerURL;
			client.Credentials = new NetworkCredential(FtpUserName, FtpPassword);

			if (sslRequired)
			{
				ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(MyCertValidationCb);
			}
			try
			{
				client.UploadFile(remoteUrl + remoteFileNameAndPath, localFile.FullName);
				success = true;
			}
			catch (WebException e)
			{
				success = false;
				FtpWebResponse ftpWebResponse = ((FtpWebResponse)e.Response);
				string status = ftpWebResponse.StatusDescription;
				string stackTrace = ftpWebResponse.BannerMessage;
				if (status.Length == 0)
				{
					status = e.Message;
				}
				throw new Exception(status, new Exception(stackTrace));
			}
			catch (Exception e)
			{
				success = false;
				throw e;
			}
			return success;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="remoteUrl"></param>
		/// <param name="localFile"></param>
		/// <param name="remoteFileNameAndPath"></param>
		/// <param name="sslRequired"></param>
		/// <returns></returns>
		public FileInfo FtpDownloadFile(String remoteUrl, DirectoryInfo localDirectory, string remoteFileNameAndPath, bool sslRequired)
		{
			FileInfo downloadedFile = null;

			WebClient client = new WebClient();
			client.BaseAddress = ServerURL;
			client.Credentials = new NetworkCredential(FtpUserName, FtpPassword);

			if (sslRequired)
			{
				ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(MyCertValidationCb);
			}
			downloadedFile = new FileInfo(localDirectory.FullName + Path.AltDirectorySeparatorChar + remoteFileNameAndPath);
			if (!downloadedFile.Directory.Exists)
			{
				Directory.CreateDirectory(downloadedFile.Directory.FullName);
			}
			client.DownloadFile(remoteUrl + remoteFileNameAndPath, localDirectory.FullName + Path.AltDirectorySeparatorChar + remoteFileNameAndPath);
			return downloadedFile;
		}

        /// <summary>
        /// Download a file using an FtpWebRequest configured to work with SSL and non-SSL FTP servers
        /// </summary>
        /// <param name="remoteResourceUrl">The URL of the resource to get, including the filename:  
        /// Ex: ftp://38.118.82.145/tds_airws_org/sandlot/XMLArchive/AMTEST/test.zip</param>
        /// <param name="downloadToDirectory">The local directory to download the file to.</param>
        /// <param name="sslRequired">The FTP server requires SSL</param>
        public void FtpDownloadFile(string remoteResourceUrl, string downloadToDirectory, bool sslRequired)
        {
            FtpDownloadFile(remoteResourceUrl, downloadToDirectory, sslRequired, null);
        }

        /// <summary>
        /// Download a file using an FtpWebRequest configured to work with SSL and non-SSL FTP servers.
        /// </summary>
        /// <param name="remoteResourceUrl">The URL of the resource to get, including the filename:  
        /// Ex: ftp://38.118.82.145/tds_airws_org/sandlot/XMLArchive/AMTEST/test.zip</param>
        /// <param name="downloadToDirectory">The local directory to download the file to.</param>
        /// <param name="sslRequired">The FTP server requires SSL</param>
        /// <param name="nameOfLocalFile">If the file should be saved with a different name from the resource
        /// being downloaded, pass the new name.</param>
        public void FtpDownloadFile(string remoteResourceUrl, string downloadToDirectory, bool sslRequired, string nameOfLocalFile)
        {
            FtpWebRequest request = CreateFtpWebRequest(remoteResourceUrl, sslRequired, WebRequestMethods.Ftp.DownloadFile);
            string outputPath = downloadToDirectory;
            if (String.IsNullOrEmpty(nameOfLocalFile))
            {
                outputPath = Path.Combine(outputPath, Path.GetFileName(remoteResourceUrl));
            }
            else
            {
                outputPath = Path.Combine(outputPath, nameOfLocalFile);
            }

            if (sslRequired)
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(MyCertValidationCb);
            }

            using (WebResponse response = request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (FileStream outputFileStream = File.Create(outputPath))
                    {
                        int bytesRead;
                        int bufferSize = 1024;
                        byte[] buffer = new byte[bufferSize];
                        
                        while ((bytesRead = responseStream.Read(buffer, 0, bufferSize)) > 0)
                        {
                            outputFileStream.Write(buffer, 0, bytesRead);
                        }
                        outputFileStream.Flush();
                    }
                }
            }
        }

        /// <summary>
        /// Rename or move a file on an FTP server
        /// </summary>
        /// <param name="remoteResourceUrl">The file to rename or remove.  This file must
        /// be in the directory passed to the ctor or if must be a relative path from that directory</param>
        /// <param name="sslRequired">Does the FTP server require SSL</param>
        /// <param name="to">Either a new filename (ex: "newfilename.zip") or
        /// a relative path with the same filename or new filename.  Ex:
        /// "./Processed/test.zip" or "./Processed/newfilename.zip"</param>
        public void FtpRenameOrMoveFile(string filename, string to, bool sslRequired)
        {
            FtpWebRequest request = CreateFtpWebRequest(Path.Combine(ServerURL, filename), sslRequired, WebRequestMethods.Ftp.Rename);
            to = to.Replace("\\", "/"); // make sure that we're using forward slashes for the destination
            request.RenameTo = to;

            if (sslRequired)
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(MyCertValidationCb);
            }

            using (WebResponse response = request.GetResponse())
            {
                // sent the request.  There's never any response, so nothing to do.
            }
        }

        /// <summary>
        /// Returns a list of filenames on the remote FTP server matching the regex pattern.
        /// Note that if no regex pattern is provided, directories will also be returned.
        /// </summary>
        /// <param name="remoteUrl"></param>
        /// <param name="sslRequired"></param>
        /// <param name="fileNameRegExPattern"></param>
        /// <returns></returns>
        public List<string> GetFileList(string remoteUrl, bool sslRequired, string fileTypeWildcard, string fileNameRegExPattern)
        {
            List<string> fileNames = new List<string>();
            string requestUrl = remoteUrl;
            if (!String.IsNullOrEmpty(fileTypeWildcard))
                requestUrl = Path.Combine(requestUrl, fileTypeWildcard);
            FtpWebRequest request = CreateFtpWebRequest(requestUrl, sslRequired, WebRequestMethods.Ftp.ListDirectory); 

            if (sslRequired)
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(MyCertValidationCb);
            }

            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string line = null;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (String.IsNullOrEmpty(fileNameRegExPattern) || Regex.IsMatch(line.Trim(), fileNameRegExPattern, RegexOptions.Singleline))
                        {
                            fileNames.Add(line.Trim());
                        }
                    }
                }
            }
            return fileNames;
        } 

        /// <summary>
        /// Returns a list of RemoteFile objects describing files on the remote FTP server that
        /// match the regex pattern for the filename and that were created on or after the date
        /// provided.  Both of these params are optional.  Note that for this method, even if no
        /// filename regex is provided, directories are excluded.
        /// </summary>
        /// <param name="remoteUrl"></param>
        /// <param name="sslRequired"></param>
        /// <param name="fileNameRegExPattern"></param>
        /// <param name="createdOnOrAfter"></param>
        /// <returns></returns>
        public List<RemoteFile> GetFileListDetails(string remoteUrl, bool sslRequired, string fileTypeWildcard, string fileNameRegExPattern, DateTime? createdOnOrAfter)
        {
            List<RemoteFile> files = new List<RemoteFile>();
            string requestUrl = remoteUrl;
            if (!String.IsNullOrEmpty(fileTypeWildcard))
                requestUrl = Path.Combine(requestUrl, fileTypeWildcard);
            FtpWebRequest request = CreateFtpWebRequest(requestUrl, sslRequired, WebRequestMethods.Ftp.ListDirectoryDetails); 

            if (sslRequired)
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(MyCertValidationCb);
            }

            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string line = null;
                    while ((line = reader.ReadLine()) != null)
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
                }
            }
            return files;
        }

        /// <summary>
        /// Helper method to keep the settings we use consistent across methods
        /// </summary>
        /// <param name="url"></param>
        /// <param name="useSSL"></param>
        /// <param name="useBinary"></param>
        /// <param name="method">WebRequestMethods.Ftp.*</param>
        /// <returns></returns>
        private FtpWebRequest CreateFtpWebRequest(string resourceUrl, bool useSSL, string method)
        {
            FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(resourceUrl);
            request.UseBinary = true;
            request.EnableSsl = useSSL;
            if (!String.IsNullOrEmpty(FtpUserName))
            {
                request.Credentials = new NetworkCredential(FtpUserName, FtpPassword);
            }
            request.UsePassive = true;
            request.Timeout = -1;
            request.Proxy = null;
            request.AuthenticationLevel = AuthenticationLevel.None;
            request.Method = method;
            return request;
        }

        /// <summary>
		/// Validating the server certificate. We will have to add the server certificate to our machine.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="certificate"></param>
		/// <param name="chain"></param>
		/// <param name="sslPolicyErrors"></param>
		/// <returns></returns>
		public static bool MyCertValidationCb(
		object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{

			if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateChainErrors)
					  == SslPolicyErrors.RemoteCertificateChainErrors)
			{
				return true; // this should be false if we are actually validating the server certificate
			}
			else if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateNameMismatch)
							== SslPolicyErrors.RemoteCertificateNameMismatch)
			{
				Zone z;
				z = Zone.CreateFromUrl(((FtpWebRequest)sender).RequestUri.ToString());
				if (z.SecurityZone == System.Security.SecurityZone.Intranet
				  || z.SecurityZone == System.Security.SecurityZone.MyComputer)
				{
					return true;
				}
				return false;
			}
			return false;
		}
	}
}
