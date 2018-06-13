using System.Runtime.Serialization;

/*
{
    "InstrumentationKey": "YOUR AI INSTRUMENTATION KEY",
    "AiEndpoint": "https://dc.services.visualstudio.com/v2/track",
    "DisableTraceTracking": false,
    "DisableMetricTracking": false,
    "DisableEventTracking": false,
    "DisableExceptionTracking": false,
    "DisableDependencyTracking": false,
    "DisableContextParameterTracking": false,
    "EnableDebug": true,
    "PercentLoggedTrace": 100,
    "PercentLoggedMetric": 100,
    "PercentLoggedEvent": 100,
    "PercentLoggedException": 100,
    "PercentLoggedDependency": 100
}
*/

namespace JLattimer.D365AppInsights
{
    [DataContract]
    public class AiSetup
    {
        [DataMember]
        public string InstrumentationKey { get; set; }

        [DataMember]
        public string AiEndpoint { get; set; }

        [DataMember]
        public bool DisableTraceTracking { get; set; }

        [DataMember]
        public bool DisableMetricTracking { get; set; }

        [DataMember]
        public bool DisableEventTracking { get; set; }

        [DataMember]
        public bool DisableExceptionTracking { get; set; }

        [DataMember]
        public bool DisableDependencyTracking { get; set; }

        [DataMember]
        public bool DisableContextParameterTracking { get; set; }

        [DataMember]
        public bool EnableDebug { get; set; }

        [DataMember]
        public int? PercentLoggedTrace { get; set; }

        [DataMember]
        public int? PercentLoggedMetric { get; set; }

        [DataMember]
        public int? PercentLoggedEvent { get; set; }

        [DataMember]
        public int? PercentLoggedException { get; set; }

        [DataMember]
        public int? PercentLoggedDependency { get; set; }
    }
}