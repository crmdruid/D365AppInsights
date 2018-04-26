namespace AppInsightsShared.Tests
{
    public class Configs
    {
        public static AiSetup GetAiSetup(bool disableDependencyTracking, bool disableEventTracking,
            bool disableExceptionTracking, bool disableMetricTracking, bool disableTraceTracking, 
            bool disableContextParameterTracking, bool enableDebug)
        {
            return new AiSetup
            {
                InstrumentationKey = "YOUR AI INSTRUMENTATION KEY",
                AiEndpoint = "https://dc.services.visualstudio.com/v2/track",
                DisableDependencyTracking = disableDependencyTracking,
                DisableEventTracking = disableEventTracking,
                DisableExceptionTracking = disableExceptionTracking,
                DisableMetricTracking = disableMetricTracking,
                DisableTraceTracking = disableTraceTracking,
                DisableContextParameterTracking = disableContextParameterTracking,
                EnableDebug = enableDebug
            };
        }
    }
}