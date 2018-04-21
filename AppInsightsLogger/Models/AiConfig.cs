using Microsoft.Xrm.Sdk;

public class AiConfig
{
    public string InstrumentationKey { get; set; }
    public string AiEndpoint { get; set; }
    public bool DisableTraceTracking { get; set; }
    public bool DisableMetricTracking { get; set; }
    public bool DisableEventTracking { get; set; }
    public bool DisableExceptionTracking { get; set; }
    public bool DisableDependencyTracking { get; set; }
    public bool DisableContextParameterTracking { get; set; }

    public AiConfig(string secureConfig)
    {
        AiSecureConfig aiSecureConfig = SerializationHelper.DeserializeObject<AiSecureConfig>(secureConfig);

        var aiEndpoint = aiSecureConfig.AiEndpoint;
        if (string.IsNullOrEmpty(aiEndpoint))
            throw new InvalidPluginExecutionException("Missing Application Insights logging endpoint url");

        var instrumentationKey = aiSecureConfig.InstrumentationKey;
        if (string.IsNullOrEmpty(instrumentationKey))
            throw new InvalidPluginExecutionException("Missing Application Insights instrumentation key");

        InstrumentationKey = instrumentationKey;
        AiEndpoint = aiEndpoint;
        DisableTraceTracking = aiSecureConfig.DisableTraceTracking;
        DisableEventTracking = aiSecureConfig.DisableEventTracking;
        DisableDependencyTracking = aiSecureConfig.DisableDependencyTracking;
        DisableExceptionTracking = aiSecureConfig.DisableExceptionTracking;
        DisableMetricTracking = aiSecureConfig.DisableMetricTracking;
        DisableContextParameterTracking = aiSecureConfig.DisableContextParameterTracking;
    }
}