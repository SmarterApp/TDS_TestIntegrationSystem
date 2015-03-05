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
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using AIR.Configuration.Security;
using OSS.TIS.Security;

namespace TISService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            // initialize secure setting manager
            SecureConfigManager.Initialize(new NoEncryptionManager());

            ////TODO: uncomment to step through a file...
            //Test();
            //return;

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new TISService() 
            };
            ServiceBase.Run(ServicesToRun);
        }

        private static void Test()
        {
            //TODO: set to the ID of the file you want to step through
            //  Note: oppId and testeeKey can be pulled from the file below if you're so inclined.
            //  If using the deserializer, a ref to the test scoring engine will be required.
            //  Easy enough just to set them here.
            long fileID = 23;
            long oppId = 5000003;
            long testeeKey = 85;

            // init the main thread
            OSS.TIS.TISMainThread mainQASystemThread = new OSS.TIS.TISMainThread();

            // get the file to step through
            TDSQASystemAPI.BL.XmlRepository xmlRepo = new TDSQASystemAPI.BL.XmlRepository();
            System.Xml.XmlDocument doc = xmlRepo.GetXmlContent(fileID);

            // init the QA system
            TDSQASystemAPI.QASystem qaSystem = new TDSQASystemAPI.QASystem("TDSQC", "TDSQC");

            // submit the file
            TDSQASystemAPI.QASystem.QAResult result = qaSystem.ReceiveTestResult(doc, new TDSQASystemAPI.Data.XmlRepositoryItem(fileID, oppId.ToString(), testeeKey, null, null));
        }
    }
}
