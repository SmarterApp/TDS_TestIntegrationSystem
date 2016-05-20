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
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using AIR.Common;
using AIR.Common.Security;
using AIR.Common.Threading;
using AIR.Common.Web;
using TDS.ScoringDaemon.Abstractions;
using TDS.ScoringDeamon.Web;
using TDS.Shared.Logging;

namespace TDS.ScoringDaemon.Web
{
    public class Application : HttpApplication
    {
       
        /// <summary>
        /// Handles the Start event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Application_Start(object sender, EventArgs e)
        {
            // Connection strings are secure (encrypted). Setup the Config Manager to take care of decryption
            SecureConfigManager.Initialize();

            RegisterServices();

            // Ignore any SSL cert errors (in case our servers are running self signed)
            ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertificate;
        
            // Run all the startup Tasks
            ExecuteStartupTasks();

            // Kick off the timer 
            Setup();
        }

        /// <summary>
        /// Register any services needed by this application.
        /// </summary>
        protected virtual void RegisterServices()
        {
            ServiceLocator.Register<ItemScoreRequestFactory>(() => new ItemScoreRequestFactory());
        }

        /// <summary>
        /// Handles the Start event of the Session control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Session_Start(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Handles the BeginRequest event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            //http://mvolo.com/iis7-integrated-mode-request-is-not-available-in-this-context-exception-in-applicationstart/
            // We need to cache our URL from the very first request made
            HttpApplication app = (HttpApplication)sender;
            HttpContext context = app.Context;
            // Attempt to peform first request initialization
            FirstRequestInitialization.Initialize(context);

        }

        /// <summary>
        /// Handles the AuthenticateRequest event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
           
        }

        /// <summary>
        /// Handles the Error event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Application_Error(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Handles the End event of the Session control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Session_End(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Handles the End event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Application_End(object sender, EventArgs e)
        {
            AdminMonitor.GetInstance().Dispose();
        }

        /// <summary>
        /// Overrides the remote certificate status essentially bypassing any SSL validation errors
        /// </summary>
        /// <remarks>
        /// http://blog.jameshiggs.com/2008/05/01/c-how-to-accept-an-invalid-ssl-certificate-programmatically/
        /// </remarks>
        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        private void Setup()
        {
            AdminMonitor.GetInstance().Start(WindowsIdentity.GetCurrent(), CallbackInitAction);            
        }

        /// <summary>
        /// Allow a callback init action to be plugged-in by the Application.
        /// If null, the ItemScoringCallbackHandler's WorkerPool will be initialized in the AdminMonitor's Start method
        /// </summary>
        protected virtual Action CallbackInitAction { get { return null; } }

        /// <summary>
        /// Runs through and executes all the startup tasks sorted by their "priority" levels
        /// </summary>
        private static void ExecuteStartupTasks()
        {
            bool startupTasksFound = false;
            foreach (Type startupTaskType in GetStartupTasks())
            {
                startupTasksFound = true;
                StartupTask startupTask = Activator.CreateInstance(startupTaskType) as StartupTask;
                if (startupTask != null)
                {
                    TDSLogger.Application.Info(String.Format("ScoringDaemon STARTUP TASK: {0}", startupTaskType.Name));
                    startupTask.Run();
                }
            }
            if (!startupTasksFound)
                throw new ApplicationException("No StartupTask was found in the system.  Missing a project reference?");
        }

        /// <summary>
        /// Gets a list of all loaded assemblies not in the GAC
        /// Note that that this will also return assemblies referenced by the main Web.UI project.
        /// </summary>
        [Obsolete("Not reliable.  Use GetAssemblies.  The classes we're attempting to load in a DLL that's referenced by Web.UI, not this DLL.  Apparently they're not always loaded.", true)]
        public static IEnumerable<Assembly> GetReferencedAssemblies()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly assembly in assemblies)
            {
                // ignore DLL's in the GAC or created in temp
                if (!assembly.GlobalAssemblyCache)
                {
                    yield return assembly;
                }
            }
        }

        /// <summary>
        /// Gets a list of all assemblies in the bin dir that are affiliated with the ScoringDaemon and loads them
        /// </summary>
        public static IEnumerable<Assembly> GetAssemblies()
        {
            string binPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "bin");
            DirectoryInfo directory = new DirectoryInfo(binPath);
            FileInfo[] files = directory.GetFiles("*ScoringDaemon*.dll", SearchOption.TopDirectoryOnly);
            foreach (FileInfo file in files)
            {
                yield return Assembly.Load(AssemblyName.GetAssemblyName(file.FullName));
            }
        }

        /// <summary>
        /// Find all the starup task classes.
        /// </summary>
        private static IEnumerable<Type> GetStartupTasks()
        {
            foreach (Assembly assembly in GetAssemblies())
            {
                // make sure assembly is TDS related (e.x., "TDS.Student.Web")
                AssemblyName assemblyName = assembly.GetName();
                if (!assemblyName.Name.Contains("ScoringDaemon")) continue;

                Type handlerInterface = typeof(StartupTask);
                Type[] types;

                try
                {
                    // this can throw ReflectionTypeLoadException
                    types = assembly.GetTypes();
                }
                catch (Exception ex)
                {
                    string errorMsg = String.Format("Error when looking for startup tasks in the assembly \"{0}\".", assemblyName.Name);
                    TDSLogger.Application.Error(new TraceLog("Application: " + errorMsg, ex));
                    continue; // skip this assembly
                }

                var handlers = types.Where(t => t.IsClass && !t.IsAbstract && handlerInterface.IsAssignableFrom(t));

                foreach (Type handlerType in handlers)
                {
                    yield return handlerType;
                }
            }
        }
    }

    internal class FirstRequestInitialization
    {
        private static bool _initializedAlready = false;
        private static readonly Object Lock = new Object();
        // Initialize only on the first request
        public static void Initialize(HttpContext context)
        {
            if (_initializedAlready)
            {
                return;
            }
            lock (Lock)
            {
                if (_initializedAlready)
                {
                    return;
                }
                // Perform first-request initialization here ...
                _initializedAlready = true;
                ScoringDaemonSettings.ItemScoringCallBackHostUrl = UrlHelper.GetApplication();
                TDSLogger.Application.Info(String.Format("Call back host URL: {0}", ScoringDaemonSettings.ItemScoringCallBackHostUrl));
            }
        }
    }
}
