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

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new TISService() 
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
