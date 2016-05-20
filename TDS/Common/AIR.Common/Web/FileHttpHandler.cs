/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Configuration;
using System.Diagnostics;
using System.Web;
using System.Web.UI.MobileControls.Adapters;

namespace AIR.Common.Web
{
    /// <summary>
    /// This is the same as internal DefaultHttpHandler except it uses a custom StaticFileHandler
    /// </summary>
    public abstract class FileHttpHandler : System.Web.DefaultHttpHandler
    {
        private static TraceSource logger = new TraceSource("System.Web");

        protected static readonly string workerRequestName;
        protected static readonly bool isIIS6or7;

        private bool _useIIS = true;
        private bool _supportRanges = false;

        static FileHttpHandler()
        {
            // get worker request name and check if IIS is 6 or 7
            string workerRequestFullName = HttpWorkerHelper.GetWorkerRequest().GetType().FullName;
            workerRequestName = workerRequestFullName.Split('.')[workerRequestFullName.Split('.').Length - 1];
            isIIS6or7 = workerRequestName.Contains("IIS6") || workerRequestName.Contains("IIS7");
        }

        /// <summary>
        /// Determines if should use IIS for processing files if available.
        /// </summary>
        public bool UseIIS
        {
            get { return _useIIS; }
            set { _useIIS = value; }
        }

        /// <summary>
        /// If this is true then we will use StaticFileHandler3 which supports HTTP ranges.
        /// </summary>
        public bool SupportRanges
        {
            get { return _supportRanges; }
            set { _supportRanges = value; }
        }

        protected virtual bool LoggingEnabled
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Write log
        /// </summary>
        protected void Log(string message)
        {
            if (LoggingEnabled)
            {
                logger.TraceEvent(TraceEventType.Information, 0, message);
            }
        }

        public override IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback callback, object state)
        {
            // If server is running on IIS6/7 then you can return the request back to IIS for processing.
            if (isIIS6or7 && _useIIS)
            {
                // When BeginProcessRequest is entered it will call OverrideExecuteUrlPath()
                return base.BeginProcessRequest(context, callback, state);
            }

            //this.OnExecuteUrlPreconditionFailure();

            HttpRequest request = context.Request;

            if (HttpWorkerHelper.HttpVerb == HttpVerb.POST)
            {
                throw new HttpException(405, SR.GetString("Method_not_allowed", request.HttpMethod, request.Path));
            }

            if (IsClassicAspRequest(request.FilePath))
            {
                throw new HttpException(403, SR.GetString("Path_forbidden", request.Path));
            }

            StaticFileHandler(context);

            return new HttpAsyncResult(callback, state, true, null, null);
        }

        /// <summary>
        /// This is called when the request cannot be handed off to IIS
        /// </summary>
        public virtual void StaticFileHandler(HttpContext context)
        {
            string physicalPath = this.OverrideExecuteUrlPath();

            if (FileFtpHandler.AllowScheme(physicalPath))
            {
                FileFtpHandler.ProcessRequestInternal(context, physicalPath);
            }
            else
            {
                // In case the path is a file:// syntax, we need to convert to physical file path
                if (UrlHelper.IsFileProtocol(physicalPath))
                {
                    Uri pathUri;
                    if (Uri.TryCreate(physicalPath, UriKind.Absolute, out pathUri))
                    {
                        physicalPath = pathUri.LocalPath;
                    }                    
                }

                if (_supportRanges) StaticFileHandler3.ProcessRequestInternal(context, physicalPath);
                else StaticFileHandler2.ProcessRequestInternal(context, physicalPath);
            }
        }

        internal static bool IsClassicAspRequest(string filePath)
        {
            return filePath.EndsWith(".asp", StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Called when IIS6 or IIS7 is not used or preconditions are not met. However the BeginProcessRequest should realize this
        /// and redirect to our custom static file handler.
        /// </summary>
        public override void OnExecuteUrlPreconditionFailure()
        {
            throw new Exception("OnExecuteUrlPreconditionFailure: For this handler to be called you must use IIS6 and meet precondition requirements.");
        }
    }
}
