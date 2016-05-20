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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Compilation;

namespace AIR.Common.Utilities
{
    public static class AssemblyLocator
    {
        private static readonly ReadOnlyCollection<Assembly> _allAssemblies;
        private static readonly ReadOnlyCollection<Assembly> _binAssemblies;

        static AssemblyLocator()
        {
            _allAssemblies = new ReadOnlyCollection<Assembly>(BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToList());

            IList<Assembly> binAssemblies = new List<Assembly>();

            string binFolder = HttpRuntime.AppDomainAppPath + "bin\\";
            IList<string> dllFiles = Directory.GetFiles(binFolder, "*.dll",
                SearchOption.TopDirectoryOnly).ToList();

            foreach (string dllFile in dllFiles)
            {
                AssemblyName assemblyName = AssemblyName.GetAssemblyName(dllFile);

                Assembly locatedAssembly = _allAssemblies.FirstOrDefault(a =>
                    AssemblyName.ReferenceMatchesDefinition(a.GetName(), assemblyName));

                if (locatedAssembly != null)
                {
                    binAssemblies.Add(locatedAssembly);
                }
            }

            _binAssemblies = new ReadOnlyCollection<Assembly>(binAssemblies);
        }

        public static ReadOnlyCollection<Assembly> GetAssemblies()
        {
            return _allAssemblies;
        }

        public static ReadOnlyCollection<Assembly> GetBinFolderAssemblies()
        {
            return _binAssemblies;
        }
    }
}
