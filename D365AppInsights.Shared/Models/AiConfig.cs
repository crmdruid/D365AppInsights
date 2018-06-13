using Microsoft.Xrm.Sdk;
using System;

namespace JLattimer.D365AppInsights
{
    public class AiConfig
    {
        private const string DefaultAiEndpoint = "https://dc.services.visualstudio.com/v2/track";
        public string InstrumentationKey { get; set; }
        public string AiEndpoint { get; set; }
        public bool DisableTraceTracking { get; set; }
        public bool DisableMetricTracking { get; set; }
        public bool DisableEventTracking { get; set; }
        public bool DisableExceptionTracking { get; set; }
        public bool DisableDependencyTracking { get; set; }
        public bool DisableContextParameterTracking { get; set; }
        public bool EnableDebug { get; set; }
        public int PercentLoggedTrace { get; set; }
        public int PercentLoggedMetric { get; set; }
        public int PercentLoggedEvent { get; set; }
        public int PercentLoggedException { get; set; }
        public int PercentLoggedDependency { get; set; }

        public AiConfig(Guid instrumentationKey, string aiEndpoint = DefaultAiEndpoint)
        {
            InstrumentationKey = instrumentationKey.ToString();
            AiEndpoint = aiEndpoint;
        }

        public AiConfig(string aiSetupJson)
        {
            AiSetup aiSetup = SerializationHelper.DeserializeObject<AiSetup>(aiSetupJson);

            var aiEndpoint = aiSetup.AiEndpoint;
            if (string.IsNullOrEmpty(aiEndpoint))
                aiEndpoint = DefaultAiEndpoint;

            var instrumentationKey = aiSetup.InstrumentationKey;
            if (string.IsNullOrEmpty(instrumentationKey))
                throw new InvalidPluginExecutionException("Missing Application Insights instrumentation key");

            if (instrumentationKey == "YOUR AI INSTRUMENTATION KEY")
                throw new InvalidPluginExecutionException("Application Insights instrumentation key not set");

            InstrumentationKey = instrumentationKey;
            AiEndpoint = aiEndpoint;
            DisableTraceTracking = aiSetup.DisableTraceTracking;
            DisableEventTracking = aiSetup.DisableEventTracking;
            DisableDependencyTracking = aiSetup.DisableDependencyTracking;
            DisableExceptionTracking = aiSetup.DisableExceptionTracking;
            DisableMetricTracking = aiSetup.DisableMetricTracking;
            DisableContextParameterTracking = aiSetup.DisableContextParameterTracking;
            EnableDebug = aiSetup.EnableDebug;
            PercentLoggedTrace = GetLogPercent(aiSetup.PercentLoggedTrace);
            PercentLoggedMetric = GetLogPercent(aiSetup.PercentLoggedMetric);
            PercentLoggedEvent = GetLogPercent(aiSetup.PercentLoggedEvent);
            PercentLoggedException = GetLogPercent(aiSetup.PercentLoggedException);
            PercentLoggedDependency = GetLogPercent(aiSetup.PercentLoggedDependency);
        }

        private static int GetLogPercent(int? input)
        {
            if (input == null)
                return 100;
            if (input > 100)
                return 100;
            if (input < 0)
                return 0;

            return input.Value;
        }
    }
}