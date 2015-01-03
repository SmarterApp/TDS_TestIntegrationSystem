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
using System.Net;
using System.Web;

namespace AIR.Common.Web
{
    /// <summary>
    /// A base class to help create ASP.NET HTTP commands.
    /// </summary>
    /// <remarks>
    /// Inspiration:
    /// http://msdn.microsoft.com/en-us/library/ff647590.aspx
    /// </remarks>
    public abstract class HttpCommandHandler : IHttpHandler
    {
        private readonly Dictionary<string, Func<HttpCommand>> _controllers = new Dictionary<string, Func<HttpCommand>>(StringComparer.CurrentCultureIgnoreCase);
        private readonly List<APIExceptionHandlerBase> _exceptionHandlers = new List<APIExceptionHandlerBase>();

        protected HttpRequestBase Request;
        protected HttpResponseBase Response;
        
        protected HttpCommandHandler()
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
            RegisterExceptionHandler(handler, false);
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

        protected void AddCommand<T>(string action) where T : HttpCommand, new()
        {
            Func<HttpCommand> controllerBuilder = () => new T();
            _controllers.Add(action, controllerBuilder);
        }

        public void ProcessRequest(HttpContext context)
        {
            ProcessRequest(new HttpContextWrapper(context));
        }

        public void ProcessRequest(HttpContextBase context)
        {
            Request = context.Request;
            Response = context.Response;

            if (Request.PathInfo.Length == 0)
            {
                SetStatus(HttpStatusCode.NotFound, "No method name was provided.");
                return;
            }

            string methodName = Request.PathInfo.Remove(0, 1);

            if (string.IsNullOrEmpty(methodName))
            {
                SetStatus(HttpStatusCode.NotFound, "Empty method name was provided.");
                return;
            }

            // check if action matches a mapping
            Func<HttpCommand> controllerBuilder;
            if (_controllers.TryGetValue(methodName, out controllerBuilder))
            {
                // run method
                try
                {
                    HttpCommand controller = controllerBuilder();
                    controller.SetHttpContext(context);
                    controller.Execute();
                }
                catch (Exception exception)
                {
                    // clear response
                    context.Response.Clear();

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

        protected void SetStatus(HttpStatusCode statusCode, string statusDescription, params object[] values)
        {
            Response.SetStatus(statusCode, statusDescription, values);
        }

        protected void SetStatus(HttpStatusCode statusCode)
        {
            SetStatus(statusCode, null);
        }
        
        /// <summary>
        /// Determines if this httphandler will be kept around for use by all requests (meaning you shouldn't keep state in internal fields)
        /// </summary>
        public bool IsReusable
        {
            get { return false; }
        }

    }

}