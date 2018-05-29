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

        public AiTrace(string message, AiTraceSeverity aiTraceSeverity)
        {
            Message = message.Length > 32768
                ? message.Substring(0, 32767)
                : message;
            SeverityLevel = aiTraceSeverity;
        }

        public static string ValidateSeverityValue(string severity)
        {
            if (string.IsNullOrEmpty(severity))
                return "Severity cannot be null";

            if (!Enum.IsDefined(typeof(AiTraceSeverity), severity))
                return "Severity valid values: Verbose, Information, Warning, Error, Critical";

            return null;
        }
    }
}