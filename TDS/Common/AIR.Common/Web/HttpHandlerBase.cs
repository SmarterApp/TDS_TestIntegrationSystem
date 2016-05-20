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
using System.IO;
using System.Net;
using System.Web;
using System.Web.UI;

namespace AIR.Common.Web
{
    /// <summary>
    /// A base class to help create ASP.NET HTTP handlers.
    /// </summary>
    /// <remarks>
    /// Future work:
    /// * http://ayende.com/blog/72705/node-cs (IHttpAsyncHandler using TPL)
    /// 
    /// Routes:
    /// * http://forums.asp.net/post/3722712.aspx
    /// * http://stackoverflow.com/questions/3502633/use-routing-in-httphandler
    /// </remarks>
    public abstract class HttpHandlerBase : IHttpHandler
    {
        private HttpContext _currentContext;
        readonly List<APIExceptionHandlerBase> _exceptionHandlers = new List<APIExceptionHandlerBase>();
        private readonly Dictionary<string, Action> _methodMapper = new Dictionary<string, Action>(StringComparer.CurrentCultureIgnoreCase);
        
        protected HttpContext CurrentContext
        {
            get { return _currentContext; }
        }

        protected HttpHandlerBase()
        {
        }

        /// <summary>
        /// Set this to true to send the error description when exceptions occur.
        /// </summary>
        public bool SendErrorMessage { get; set; }

        protected void RegisterExceptionHandler<T>(Action<T> handler, bool global) where T : Exception
        {
            APIExceptionHandler<T> exceptionHandler = new APIExceptionHandler<T>(handler, global);
            _exceptionHandlers.Add(exceptionHandler);
        }

        protected void RegisterExceptionHandler<T>(Action<T> handler) where T : Exception
        {
            RegisterExceptionHandler<T>(handler, false);
        }

        private bool InvokeExceptionHandler(Exception exception)
        {
            bool handled = false;

            foreach (APIExceptionHandlerBase exceptionHandler in _exceptionHandlers)
            {
                if (exceptionHandler.Is(exception))
                {
                    // mark that we found a handler
                    handled = true;

                    // execute handler
                    exceptionHandler.Invoke(exception);
                    
                    // if the handler is not global then stop processing other handlers
                    if (!exceptionHandler.Global) break;
                }
            }

            return handled;
        }

        public void ProcessRequest(HttpContext context)
        {
            this._currentContext = context;

            if (_currentContext.Request.PathInfo.Length == 0)
            {
                SetStatus(HttpStatusCode.NotFound, "No method name was provided.");
                return;
            }

            string methodName = _currentContext.Request.PathInfo.Remove(0, 1);

            if (string.IsNullOrEmpty(methodName))
            {
                SetStatus(HttpStatusCode.NotFound, "Empty method name was provided.");
                return;
            }

            // call init
            Init();

            // check if action matches a mapping
            Action methodAction;
            if (_methodMapper.TryGetValue(methodName, out methodAction))
            {
                // run method
                try
                {
                    PreAction();
                    methodAction();
                    //PostAction();
                }
                catch (Exception exception)
                {
                    // clear response
                    CurrentContext.Response.Clear();

                    // check for error handler
                    bool exceptionHandled = InvokeExceptionHandler(exception);

                    if (!exceptionHandled)
                    {
                        if (SendErrorMessage)
                        {
                            SetStatus(HttpStatusCode.InternalServerError, exception.Message);
                        }
                        else
                        {
                            SetStatus(HttpStatusCode.InternalServerError);
                        }
                    }
                }
            }
            else
            {
                // method not found!
                SetStatus(HttpStatusCode.NotFound, "The method name is not mapped to a function."); // 404
            }
        }

        /// <summary>
        /// This is called every time a request comes in regardless if the action method is found.
        /// </summary>
        protected virtual void Init() {}

        /// <summary>
        /// This is called before the action method is invoked.
        /// </summary>
        protected virtual void PreAction() { }

        /// <summary>
        /// This is called after the action method is invoked.
        /// </summary>
        // protected virtual void PostAction() {}

        /// <summary>
        /// Maps path to a method
        /// </summary>
        /// <param name="name">PathInfo of the url 
        /// (e.x., you would register "methodName" if the url looked like: http://example.com/handler.ashx/methodName?param=test)</param>
        /// <param name="action">the function to call</param>
        protected void MapMethod(string name, Action action)
        {
            _methodMapper.Add(name, action);
        }

        protected void SetStatus(HttpStatusCode statusCode, string statusDescription, params object[] values)
        {
            _currentContext.Response.StatusCode = (int) statusCode;

            if (!string.IsNullOrEmpty(statusDescription))
            {
                // NOTE: You can't have a linebreak in the middle of an HTTP header.
                if (statusDescription.Contains(Environment.NewLine))
                {
                    statusDescription = statusDescription.Replace(Environment.NewLine, " ");
                }

                _currentContext.Response.StatusDescription = string.Format(statusDescription, values);
            }
        }

        protected void SetStatus(HttpStatusCode statusCode)
        {
            SetStatus(statusCode, null);
        }

        /// <summary>
        /// Render an ASCX user control to a text writer. 
        /// </summary>
        /// <remarks>
        /// Some helpsful links:
        /// - http://stackoverflow.com/questions/6924586/usercontrol-render-with-object-asp-net-c-sharp
        /// - http://stackoverflow.com/questions/976524/issues-rendering-usercontrol-using-server-execute-in-an-asmx-web-service
        /// - http://stackoverflow.com/questions/1325101/create-instance-aspx-page-of-ascx-control-in-a-back-end-class-without-loading-fi
        /// - http://stackoverflow.com/questions/58925/asp-net-how-to-render-a-control-to-html
        /// - http://haacked.com/archive/2011/08/01/text-templating-using-razor-the-easy-way.aspx
        /// </remarks>
        protected void RenderUserControl(string virtualPath, TextWriter textWriter)
        {
            // load user control
            Page pageHolder = new Page();

            UserControl viewControl = pageHolder.LoadControl(virtualPath) as UserControl;

            // execute user control and render output
            if (viewControl != null)
            {
                pageHolder.Controls.Add(viewControl);
                CurrentContext.Server.Execute(pageHolder, textWriter, true);
            }
        }

        /// <summary>
        /// Render an ASCX user control and return HTML as a string. 
        /// </summary>
        protected string RenderUserControl(string virtualPath)
        {
            StringWriter sw = new StringWriter();
            RenderUserControl(virtualPath, sw);
            return sw.ToString();
        }

        /// <summary>
        /// Determines if this httphandler will be kept around for use by all requests (meaning you shouldn't keep state in internal fields)
        /// </summary>
        public bool IsReusable
        {
            get { return false; }
        }

        #region Utilities

        public long CurrentTimeMillis()
        {
            return WebHelper.CurrentTimeMillis();
        }

        public void SetMIMEType(string contentType)
        {
            WebHelper.SetContentType(contentType);
        }

        public void SetMIMEType(ContentType contentType)
        {
            WebHelper.SetContentType(contentType);
        }

        public static string GetString(string name)
        {
            return WebHelper.GetString(name);
        }

        public static string GetQueryString(string name)
        {
            return WebHelper.GetQueryString(name);
        }

        public static T GetQueryValue<T>(string name)
        {
            return WebHelper.GetQueryValue<T>(name);
        }

        public static List<T> GetQueryValues<T>(string name)
        {
            return WebHelper.GetQueryValues<T>(name);
        }

        public static string GetFormString(string name)
        {
            return WebHelper.GetFormString(name);
        }

        public static T GetFormValue<T>(string name)
        {
            return WebHelper.GetFormValue<T>(name);
        }

        public static List<T> GetFormValues<T>(string name)
        {
            return WebHelper.GetFormValues<T>(name);
        }

        public static void WriteString(string value)
        {
            WebHelper.WriteString(value);
        }

        public static void WriteString(string value, params object[] values)
        {
            WebHelper.WriteString(value, values);
        }

        public static void WriteJsonString(string value)
        {
            WebHelper.WriteJsonString(value);
        }

        public static void WriteJsonObject<T>(T data)
        {
            WebHelper.WriteJsonObject<T>(data);
        }

        public static T GetJsonObject<T>()
        {
            return WebHelper.GetJsonObject<T>();
        }

        public static string ParseJsonString<T>(T o)
        {
            return WebHelper.ParseJsonString<T>(o);
        }

        #endregion
    }

    #region Exception Handling

    public abstract class APIExceptionHandlerBase
    {
        /// <summary>
        /// Is this handler global and should allow other handlers to execute. 
        /// </summary>
        public bool Global { get; set; }
        
        /// <summary>
        /// Is the exception passed in a subclass of what this handlers type is.
        /// </summary>
        public abstract bool Is(Exception exception);
        
        /// <summary>
        /// Execute the handler and pass in the current exception.
        /// </summary>
        public abstract void Invoke(Exception exception);
    }

    public class APIExceptionHandler<T> : APIExceptionHandlerBase where T : Exception
    {
        private readonly Action<T> _action;

        public APIExceptionHandler(Action<T> action, bool global)
        {
            _action = action;
            Global = global;
        }

        public override bool Is(Exception exception)
        {
            return (exception is T);
        }

        public override void Invoke(Exception exception)
        {
            _action((T)exception);
        }
    }

    #endregion

}