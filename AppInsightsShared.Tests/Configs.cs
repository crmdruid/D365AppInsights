namespace AppInsightsShared.Tests
{
    public class Configs
    {
        public static AiSecureConfig GetAiSecureConfig(bool disableDependencyTracking, bool disableEventTracking,
            bool disableExceptionTracking, bool disableMetricTracking, bool disableTraceTracking, bool disableContextParameterTracking)
        {
            return new AiSecureConfig
            {
                InstrumentationKey = "YOUR AI INSTRUMENTATION KEY",
                AiEndpoint = "https://dc.services.visualstudio.com/v2/track",
                DisableDependencyTracking = disableDependencyTracking,
                DisableEventTracking = disableEventTracking,
                DisableExceptionTracking = disableExceptionTracking,
                DisableMetricTracking = disableMetricTracking,
                DisableTraceTracking = disableTraceTracking,
                DisableContextParameterTracking = disableContextParameterTracking
            };
        }
    }
}