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
    public bool EnableDebug { get; set; }

    public AiConfig(string secureConfig)
    {
        AiSetup aiSetup = SerializationHelper.DeserializeObject<AiSetup>(secureConfig);

        var aiEndpoint = aiSetup.AiEndpoint;
        if (string.IsNullOrEmpty(aiEndpoint))
            throw new InvalidPluginExecutionException("Missing Application Insights logging endpoint url");

        var instrumentationKey = aiSetup.InstrumentationKey;
        if (string.IsNullOrEmpty(instrumentationKey))
            throw new InvalidPluginExecutionException("Missing Application Insights instrumentation key");

        InstrumentationKey = instrumentationKey;
        AiEndpoint = aiEndpoint;
        DisableTraceTracking = aiSetup.DisableTraceTracking;
        DisableEventTracking = aiSetup.DisableEventTracking;
        DisableDependencyTracking = aiSetup.DisableDependencyTracking;
        DisableExceptionTracking = aiSetup.DisableExceptionTracking;
        DisableMetricTracking = aiSetup.DisableMetricTracking;
        DisableContextParameterTracking = aiSetup.DisableContextParameterTracking;
        EnableDebug = aiSetup.EnableDebug;
    }
}