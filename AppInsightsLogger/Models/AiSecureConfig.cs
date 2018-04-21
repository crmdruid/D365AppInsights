using System.Runtime.Serialization;

/*
 {
    "InstrumentationKey": "Your Key",
    "AiEndpoint": "https://dc.services.visualstudio.com/v2/track",
    "DisableTraceTracking": false,
    "DisableMetricTracking": false,
    "DisableEventTracking": false,
    "DisableExceptionTracking": false,
    "DisableDependencyTracking": false,
    "DisableContextParameterTracking": false
}
*/

[DataContract]
public class AiSecureConfig
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
}