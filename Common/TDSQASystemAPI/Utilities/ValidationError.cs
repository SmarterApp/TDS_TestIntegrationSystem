using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TDSQASystemAPI.Utilities
{
    public class ValidationResponseError
    {
        [JsonProperty("errors")]        
        public IList<ValidationError> Errors { get; private set; }

        public ValidationResponseError(IList<ValidationError> errors)
        {
            Errors = errors ?? throw new ArgumentNullException(nameof(errors));
        }

    }

    /// <summary>
    /// A generic error object, similar in structure to the <code>ValidationError</code> in TDS.
    /// </summary>
    public class ValidationError
    {
        /// <summary>
        /// The error code for the error type.
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; private set; }

        /// <summary>
        /// The error message with details of the error that occurred.
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; private set; }

        public ValidationError(string code, string message)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }
    }
}
