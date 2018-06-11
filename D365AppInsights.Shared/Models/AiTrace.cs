using System;
using System.Runtime.Serialization;

namespace JLattimer.D365AppInsights
{
    [DataContract]
    public class AiTrace : AiBaseData
    {
        [DataMember(Name = "message")]
        public string Message { get; set; }

        [DataMember(Name = "severityLevel")]
        public AiTraceSeverity SeverityLevel { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AiTrace"/> class.
        /// </summary>
        /// <param name="properties">The D365 specific AI properties.</param>
        /// <param name="message">The trace message.</param>
        /// <param name="aiTraceSeverity">The severity level <see cref="AiTraceSeverity"/>.</param>
        public AiTrace(AiProperties properties, string message, AiTraceSeverity aiTraceSeverity)
        {
            Message = message.Length > 32768
                ? message.Substring(0, 32767)
                : message;
            SeverityLevel = aiTraceSeverity;
            Properties = properties;
        }

        public static string ValidateSeverityValue(string severity)
        {
            if (string.IsNullOrEmpty(severity))
                return "Invalid Severity: cannot be null";

            if (!Enum.IsDefined(typeof(AiTraceSeverity), severity))
                return "Invalid Severity: valid values: Verbose, Information, Warning, Error, Critical";

            return null;
        }
    }
}