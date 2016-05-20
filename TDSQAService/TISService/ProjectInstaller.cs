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
using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceProcess;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.Configuration;

namespace TISService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
            this.Installers.Add(GetServiceInstaller());
            this.Installers.Add(GetServiceProcessInstaller());
        }

        private ServiceInstaller GetServiceInstaller()
        {
            ServiceInstaller installer = new ServiceInstaller();
            installer.ServiceName = GetConfigurationValue("ServiceName");
            return installer;
        }

        private ServiceProcessInstaller GetServiceProcessInstaller()
        {
            ServiceProcessInstaller installer = new ServiceProcessInstaller();
            installer.Account = ServiceAccount.LocalSystem;
            return installer;
        }

        private string GetConfigurationValue(string key)
        {
            Assembly service = Assembly.GetAssembly(typeof(ProjectInstaller));
            Configuration config = ConfigurationManager.OpenExeConfiguration(service.Location);
            if (config.AppSettings.Settings[key] != null)
            {
                return config.AppSettings.Settings[key].Value;
            }
            else
            {
                throw new IndexOutOfRangeException("Settings collection does not contain the requested key:" + key);
            }
        }
    }
}
