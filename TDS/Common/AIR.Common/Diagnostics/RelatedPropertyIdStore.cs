/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Remoting.Messaging;

namespace AIR.Common.Diagnostics
{
    /// <summary>
    /// Static class used to inject the RelatedActivityId into Thread Local Storage (<see cref="CallContext" />)
    /// during a TraceTransfer
    /// </summary>
    public static class RelatedActivityIdStore
    {
        internal const string RELATED_ACTIVITY_ID_KEY = "TDS.Common.Logging.RelatedActivityId";

        /// <summary>
        /// Injects the RelatedActivityId into Thread Local Storage (<see cref="CallContext" />). 
        /// Can be used with a using block to remove from TLS on disposal.
        /// </summary>
        /// <param name="relatedActivityId">The related activity id of the trace transfer</param>
        /// <returns>An <see cref="IDisposable"/> type that removes the RelatedActivityId from <see cref="CallContext" />
        /// when Dispose is called.</returns>
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public static IDisposable SetRelatedActivityId(Guid relatedActivityId)
        {
            CallContext.SetData(RELATED_ACTIVITY_ID_KEY, relatedActivityId);
            return new RemoveOnDispose();
        }

        /// <summary>
        /// Removeds the RelatedActivityId from Thread Local Storage (<see cref="CallContext" />).
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public static void RemoveRelatedActivityId()
        {
            CallContext.FreeNamedDataSlot(RELATED_ACTIVITY_ID_KEY);
        }

        /// <summary>
        /// Tries to retrieve the RelatedActivityId from Thread Local Storage (<see cref="CallContext"/>)
        /// </summary>
        /// <param name="relatedActivityId">The related activity id retrieved</param>
        /// <returns>Indicates whether the RelatedActivityId exists in <see cref="CallContext"/>.</returns>
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public static bool TryGetRelatedActivityId(out Guid relatedActivityId)
        {
            object data = CallContext.GetData(RELATED_ACTIVITY_ID_KEY);
            if (data != null)
            {
                relatedActivityId = (Guid) data;
                return true;
            }
            relatedActivityId = Guid.Empty;
            return false;
        }

        /// <summary>
        /// Private implementation of <see cref="IDisposable" /> returned when <see cref="RelatedActivityIdStore.SetRelatedActivityId"/>
        /// is called.
        /// </summary>
        private class RemoveOnDispose : IDisposable
        {
            void IDisposable.Dispose()
            {
                RemoveRelatedActivityId();
            }
        }
    }
}