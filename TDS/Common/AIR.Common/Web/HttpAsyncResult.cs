/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Threading;
using System.Web;

namespace AIR.Common.Web
{
    public enum RequestNotificationStatus
    {
        Continue,
        Pending,
        FinishRequest
    }

    public class HttpAsyncResult : IAsyncResult
    {
        private object _asyncState;
        private AsyncCallback _callback;
        private bool _completed;
        private bool _completedSynchronously;
        private Exception _error;
        private object _result;
        private RequestNotificationStatus _status;

        public HttpAsyncResult(AsyncCallback cb, object state)
        {
            this._callback = cb;
            this._asyncState = state;
            this._status = RequestNotificationStatus.Continue;
        }

        public HttpAsyncResult(AsyncCallback cb, object state, bool completed, object result, Exception error)
        {
            this._callback = cb;
            this._asyncState = state;
            this._completed = completed;
            this._completedSynchronously = completed;
            this._result = result;
            this._error = error;
            this._status = RequestNotificationStatus.Continue;
            if (this._completed && (this._callback != null))
            {
                this._callback(this);
            }
        }

        public void Complete(bool synchronous, object result, Exception error)
        {
            this.Complete(synchronous, result, error, RequestNotificationStatus.Continue);
        }

        public void Complete(bool synchronous, object result, Exception error, RequestNotificationStatus status)
        {
            this._completed = true;
            this._completedSynchronously = synchronous;
            this._result = result;
            this._error = error;
            this._status = status;
            if (this._callback != null)
            {
                this._callback(this);
            }
        }

        public object End()
        {
            if (this._error != null)
            {
                throw new HttpException(null, this._error);
            }
            return this._result;
        }

        public void SetComplete()
        {
            this._completed = true;
        }

        public object AsyncState
        {
            get
            {
                return this._asyncState;
            }
        }

        public WaitHandle AsyncWaitHandle
        {
            get
            {
                return null;
            }
        }

        public bool CompletedSynchronously
        {
            get
            {
                return this._completedSynchronously;
            }
        }

        public Exception Error
        {
            get
            {
                return this._error;
            }
        }

        public bool IsCompleted
        {
            get
            {
                return this._completed;
            }
        }

        public RequestNotificationStatus Status
        {
            get
            {
                return this._status;
            }
        }
    }
}