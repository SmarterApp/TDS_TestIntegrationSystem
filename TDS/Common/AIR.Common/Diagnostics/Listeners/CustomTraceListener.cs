/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Remoting.Messaging;

namespace AIR.Common.Diagnostics
{
    /// <summary>
    /// An abstract class designed to make creating a <see cref="TraceListener"/> easier with just two 
    /// methods to override (<see cref="TraceEventCore"/> and <see cref="TraceDataCore"/>).
    /// </summary>
    public abstract class CustomTraceListener : TraceListener
    {
        protected CustomTraceListener(string name) : base(name)
        {
        }

        /// <summary>
        /// Determines whether a filter is attached to this listener and, if so, asks whether it ShouldTrace applies to this data.
        /// </summary>
        protected virtual bool ShouldTrace(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string formatOrMessage, object[] args, object data1, object[] data)
        {
            return !(Filter != null && !Filter.ShouldTrace(eventCache, source, eventType, id, formatOrMessage, args, data1, data));
        }

        /// <summary>
        /// Called before the main TraceEventCore method and applies any filter by calling ShouldTrace.
        /// </summary>
        protected virtual void FilterTraceEventCore(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            try
            {
                if (ShouldTrace(eventCache, source, eventType, id, message, null, null, null))
                {
                    TraceEventCore(eventCache, source, eventType, id, message);
                }
            }
            catch (Exception exc)
            {
                InternalLogger.LogException(exc);
            }
        }

        /// <summary>
        /// Called before the main TraceDataCore method and applies any filter by calling ShouldTrace.
        /// </summary>
        protected virtual void FilterTraceDataCore(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
        {
            try
            {
                if (ShouldTrace(eventCache, source, eventType, id, null, null, null, data))
                {
                    TraceDataCore(eventCache, source, eventType, id, data);
                }
            }
            catch (Exception exc)
            {
                InternalLogger.LogException(exc);
            }
        }

        /// <summary>
        /// Logs a transfer to a new ActivityId (and uses the <see cref="RelatedActivityIdStore"/> to store the 
        /// relatedActivityId in <see cref="CallContext"/> for the duration of the call)
        /// </summary>
        /// <param name="relatedActivityId">The activityId to which we are transferring</param>
        public override void TraceTransfer(TraceEventCache eventCache, string source, int id, string message, Guid relatedActivityId)
        {
            using (RelatedActivityIdStore.SetRelatedActivityId(relatedActivityId))
            {
                base.TraceTransfer(eventCache, source, id, message, relatedActivityId);
            }
        }

        /// <summary>
        /// This method must be overriden and forms the core logging method called by all other TraceEvent methods.
        /// </summary>
        protected abstract void TraceEventCore(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message);

        /// <summary>
        /// This method must be overriden and forms the core logging method called by all otherTraceData methods.
        /// </summary>
        protected abstract void TraceDataCore(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data);

        public override void Write(string message)
        {
            FilterTraceEventCore(null, string.Empty, TraceEventType.Information, 0, message);
        }

        public override void WriteLine(string message)
        {
            Write(message);
        }

        public override sealed void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            FilterTraceEventCore(eventCache, source, eventType, id, message);
        }

        public override sealed void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            string message = format;
            
            if (args != null)
            {
                message = string.Format(CultureInfo.CurrentCulture, format, args);
            }
            
            FilterTraceEventCore(eventCache, source, eventType, id, message);
        }

        public override sealed void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
        {
            FilterTraceEventCore(eventCache, source, eventType, id, null);
        }

        public override sealed void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            FilterTraceDataCore(eventCache, source, eventType, id, data);
        }

        public override sealed void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
        {
            FilterTraceDataCore(eventCache, source, eventType, id, data);
        }
    }
}