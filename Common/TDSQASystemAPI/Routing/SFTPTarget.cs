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
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Xsl;
using System.IO;
using TDSQASystemAPI.TestResults;
using TDSQASystemAPI.ExceptionHandling;
using TDSQASystemAPI.Extensions;
using CommonUtilities;
using CommonUtilities.Configuration;

namespace TDSQASystemAPI.Routing
{
    public class SFTPTarget : Target
    {
        public SFTPTarget(string targetName, TargetClass targetClass, TargetType type, XMLAdapter.AdapterType xmlVersion, FileTransformSpec transformSpec, IFileTransformArgs transformArgs)
            : base(targetName, targetClass, type, xmlVersion, transformSpec, transformArgs) { }

        public SFTPTarget(string targetName, TargetClass targetClass, TargetType type, XMLAdapter.AdapterType xmlVersion, FileTransformSpec transformSpec)
            : base(targetName, targetClass, type, xmlVersion, transformSpec)
        {
        }

        /// <summary>
        /// Sent the test result to an SFTP server using base setup
        /// </summary>
        /// <param name="testResult"></param>
        /// <returns></returns>
        public override ITargetResult Send(TestResult testResult, Action<object> outputProcessor, params object[] inputArgs)
        {
            // the target's name will be the FTPSite's name
            FTPSite ftpSettings = Settings.FTP[base.Name];

            if (ftpSettings == null)
                throw new QAException(String.Format("There is no FTPSite configured with name: {0}", base.Name), QAException.ExceptionType.ConfigurationError);

            if (String.IsNullOrEmpty(ftpSettings.TempDirectoryPath))
                throw new QAException(String.Format("There is no tempDirectoryPath configured for FTPSite: {0}", base.Name), QAException.ExceptionType.ConfigurationError);

            if (!(new System.IO.DirectoryInfo(ftpSettings.TempDirectoryPath).Exists))
                throw new QAException(String.Format("Path not accessible: {0}", ftpSettings.TempDirectoryPath), QAException.ExceptionType.ConfigurationError);

            string fileName = MakeFileName(testResult);
            string pathToFile = Path.Combine(ftpSettings.TempDirectoryPath, fileName);

            try
            {
                // write the file out to a temp dir
                string xml = base.GetPayloadAsString(testResult);
                File.WriteAllText(pathToFile, xml, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new QAException(String.Format("Error preparing file for Handscoring. OppID: {0}, Message: {1}", testResult.Opportunity.OpportunityID, ex.Message), QAException.ExceptionType.General, ex);
            }

            // FTP the file using WinSCPLibWrapper
            try
            {
                new WinSCPLibWrapper().PutFiles(ftpSettings.URL, ftpSettings.Username, ftpSettings.Password, fileName,
                    ftpSettings.TempDirectoryPath, null, ftpSettings.WinSCPExecutablePath, ftpSettings.HostKey, ftpSettings.SSLRequired,
                    ftpSettings.TransferMode, ftpSettings.TimeoutInSeconds, true);
            }
            catch (Exception ex)
            {
                throw new QAException(String.Format("Exception uploading file: {0} for oppId: {1} to client site: {2}.  Message: {3}", pathToFile, testResult.Opportunity.OpportunityID, ftpSettings.URL, ex.Message), QAException.ExceptionType.General, ex);
            }

            return new TargetResult() { Sent = true, ID = ftpSettings.URL };
        }

        //File naming convention: <[OP|DEMO]>_<TestName>_<OpportunityID>_<Status>_<DateTimeStamp>.xml
        //Example: OP_(Minnesota)GRAD-Paper-Writing-10-Fall-2012-2013_1232123_invalidated_20120925T121612469.xml
        //
        //TODO: allow this to be plugged in so that a custom target isn't required just to change the filename?
        //  Filename is needed for REST too if using multipart
        protected virtual string MakeFileName(TestResult testResult)
        {
            return String.Format("{0}_{1}_{2}_{3}_{4}.xml", testResult.Testee.TesteeDistrictType,
                testResult.Name.Replace("_", "-"), testResult.Opportunity.OpportunityID, testResult.Opportunity.Status,
                base.CreateDate.ToString("yyyyMMddTHHmmssfff"));
        }
    }
}
