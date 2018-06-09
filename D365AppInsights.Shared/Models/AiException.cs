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

        /// <summary>
        /// Initializes a new instance of the <see cref="AiException"/> class.
        /// </summary>
        /// <param name="exception">The exception being logged.</param>
        /// <param name="severity">The severity level <see cref="AiExceptionSeverity"/>.</param>
        public AiException(Exception exception, AiExceptionSeverity severity)
        {
            TypeName = exception.GetType().Name.Length > 1024
                ? exception.GetType().Name.Substring(0, 1023)
                : exception.GetType().Name;
            Message = exception.Message;
            HasFullStack = !string.IsNullOrEmpty(exception.StackTrace);
            Stack = HasFullStack ? exception.StackTrace : null;
            ParsedStacks = ExceptionHelper.GetParsedStacked(exception);
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