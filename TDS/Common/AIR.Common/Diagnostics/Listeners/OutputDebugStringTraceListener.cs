using System;
using System.Diagnostics;

namespace AIR.Common.Diagnostics
{
    /// <summary>
    /// A TraceListener that supports formatted output and writes using OutputDebugString
    /// </summary>
    public class OutputDebugStringTraceListener : CustomTraceListener
    {
        /// <summary>
        /// Creates a new <see cref="OutputDebugStringTraceListener"/> />
        /// </summary>
        public OutputDebugStringTraceListener() : base("OutputDebugStringTraceListener")
        {
        }

        protected override void TraceEventCore(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            SafeNativeMethods.OutputDebugString(String.Format("{0} {1}({2}) {3} ", eventType, source, id, message));
        }

        protected override void TraceDataCore(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
        {
            SafeNativeMethods.OutputDebugString(String.Format("{0} {1}({2}) {3} ", eventType, source, id, StringFormatter.FormatData(data)));
        }
    }
}