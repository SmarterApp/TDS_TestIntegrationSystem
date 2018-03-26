using System;

namespace TDSQASystemAPI.Utilities
{
    /// <summary>
    /// A generic error object, similar in structure to the <code>ValidationError</code> in TDS.
    /// </summary>
    public class ValidationError
    {
        /// <summary>
        /// The error code for the error type.
        /// </summary>
        public string Code { get; private set; }

        /// <summary>
        /// Describe the severity of the error.
        /// </summary>
        public string Severity { get; private set; }

        /// <summary>
        /// The error message with details of the error that occurred.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// The error message translated into another langauge.
        /// </summary>
        /// <remarks>
        /// Can be <code>null</code>.
        /// </remarks>
        public string TranslatedMessage { get; private set; }

        public ValidationError(string code, string severity, string message) : this(code, severity, message, null) { }

        public ValidationError(string code, string severity, string message, string translatedMessage)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Severity = severity ?? throw new ArgumentNullException(nameof(severity));
            Message = message ?? throw new ArgumentNullException(nameof(message));
            TranslatedMessage = translatedMessage;
        }
    }
}
