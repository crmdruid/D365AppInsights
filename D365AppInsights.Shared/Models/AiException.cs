using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JLattimer.D365AppInsights
{
    [DataContract]
    public class AiException
    {
        [DataMember(Name = "typeName")]
        public string TypeName { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }

        [DataMember(Name = "hasFullStack")]
        public bool HasFullStack { get; set; }

        [DataMember(Name = "stack")]
        public string Stack { get; set; }

        [DataMember(Name = "parsedStack")]
        public List<AiParsedStack> ParsedStacks { get; set; }

        [DataMember(Name = "severityLevel")]
        public AiExceptionSeverity SeverityLevel { get; set; }

        public AiException(Exception e, AiExceptionSeverity severity)
        {
            TypeName = e.GetType().Name.Length > 1024
                ? e.GetType().Name.Substring(0, 1023)
                : e.GetType().Name;
            Message = e.Message;
            HasFullStack = !string.IsNullOrEmpty(e.StackTrace);
            Stack = HasFullStack ? e.StackTrace : null;
            ParsedStacks = ExceptionHelper.GetParsedStacked(e);
            SeverityLevel = severity;
        }

        public static string ValidateSeverityValue(string severity)
        {
            if (string.IsNullOrEmpty(severity))
                return "Severity cannot be null";

            if (!Enum.IsDefined(typeof(AiExceptionSeverity), severity))
                return "Severity valid values: Verbose, Information, Warning, Error, Critical";

            return null;
        }
    }
}