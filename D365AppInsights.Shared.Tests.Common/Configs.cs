using JLattimer.D365AppInsights;
using System;

namespace D365AppInsights.Shared.Tests.Common
{
    public class Configs
    {
        public static AiSetup GetAiSetup(bool disableDependencyTracking, bool disableEventTracking,
            bool disableExceptionTracking, bool disableMetricTracking, bool disableTraceTracking,
            bool disableContextParameterTracking, bool enableDebug, int percentLoggedTrace = 100, int percentLoggedMetric = 100,
            int percentLoggedEvent = 100, int percentLoggedException = 100, int percentLoggedDependency = 100)
        {
            const string aiInstrumentationKey = "YOUR AI INSTRUMENTATION KEY";

            if (aiInstrumentationKey == "YOUR AI INSTRUMENTATION KEY")
                throw new Exception(
                    "Enter your Application Insights instrumention key in D365AppInsights.Shared.Tests.Common.Configs");

            return new AiSetup
            {
                InstrumentationKey = aiInstrumentationKey,
                AiEndpoint = "https://dc.services.visualstudio.com/v2/track",
                DisableDependencyTracking = disableDependencyTracking,
                DisableEventTracking = disableEventTracking,
                DisableExceptionTracking = disableExceptionTracking,
                DisableMetricTracking = disableMetricTracking,
                DisableTraceTracking = disableTraceTracking,
                DisableContextParameterTracking = disableContextParameterTracking,
                EnableDebug = enableDebug,
                PercentLoggedTrace = percentLoggedTrace,
                PercentLoggedMetric = percentLoggedMetric,
                PercentLoggedEvent = percentLoggedEvent,
                PercentLoggedException = percentLoggedException,
                PercentLoggedDependency = percentLoggedDependency
            };
        }
    }
}