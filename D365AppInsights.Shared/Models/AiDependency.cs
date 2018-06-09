using System;
using System.Runtime.Serialization;

namespace JLattimer.D365AppInsights
{
    [DataContract]
    public class AiDependency : AiBaseData
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "resultCode")]
        public int? ResultCode { get; set; }

        [DataMember(Name = "duration")]
        public int Duration { get; set; }

        [DataMember(Name = "success")]
        public bool Success { get; set; }

        [DataMember(Name = "data")]
        public string Data { get; set; }

        [DataMember(Name = "target")]
        public string Target { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AiDependency"/> class.
        /// </summary>
        /// <param name="properties">The D365 specific AI properties.</param>
        /// <param name="name">The dependency name or absolute URL.</param>
        /// <param name="method">The HTTP method (only logged with URL).</param>
        /// <param name="type">The type of dependency (Ajax, HTTP, SQL, etc.).</param>
        /// <param name="duration">he duration in ms of the dependent event.</param>
        /// <param name="resultCode">The result code, HTTP or otherwise.</param>
        /// <param name="success">Set to <c>true</c> if the dependent event was successful, <c>false</c> otherwise.</param>
        /// <param name="data">Any other data associated with the dependent event (ignored with URL).</param>
        public AiDependency(AiProperties properties, string name, string method,
            string type, int duration, int? resultCode, bool success, string data)
        {
            Uri uri = CreateUri(name);
            if (uri != null)
            {
                method = method.ToUpper();
                Name = !string.IsNullOrEmpty(method)
                    ? $"{method} {uri.AbsolutePath}"
                    : uri.AbsolutePath;
                Data = name;
                Target = uri.Host;
            }
            else
            {
                Name = name;
                Data = data;
                Target = null;
            }

            if (Name.Length > 512)
                Name = Name.Substring(0, 511);

            Id = LogHelper.GenerateNewId();
            ResultCode = resultCode;
            Duration = duration;
            Success = success;
            Type = type;
            Properties = properties;
        }

        private static Uri CreateUri(string name)
        {
            bool isUri = Uri.TryCreate(name, UriKind.Absolute, out Uri uri);
            return isUri ? uri : null;
        }
    }
}