using JLattimer.D365AppInsights;
using System;

namespace D365AppInsights.Action
{
    public class LogTraceAsync : PluginBase
    {
        #region Constructor/Configuration
        private readonly string _unsecureConfig;
        private readonly string _secureConfig;

        public LogTraceAsync(string unsecure, string secure)
            : base(typeof(LogTrace))
        {
            _unsecureConfig = unsecure;
            _secureConfig = secure;
        }
        #endregion

        protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {
            if (localContext == null)
                throw new ArgumentNullException(nameof(localContext));

            try
            {
                AiLogger aiLogger = new AiLogger(_unsecureConfig, localContext.OrganizationService, localContext.TracingService,
                    localContext.PluginExecutionContext, localContext.PluginExecutionContext.Stage, null);

                string message = ActionHelpers.GetInputValue<string>("message", localContext.PluginExecutionContext, localContext.TracingService);
                string severity = ActionHelpers.GetInputValue<string>("severity", localContext.PluginExecutionContext, localContext.TracingService);

                string severityValidationResult = AiTrace.ValidateSeverityValue(severity);
                if (!string.IsNullOrEmpty(severityValidationResult))
                {
                    localContext.TracingService.Trace(severityValidationResult);
                    return;
                }

                bool isValid = Enum.TryParse(severity, out AiTraceSeverity traceSeverity);

                aiLogger.WriteTrace(message, isValid
                    ? traceSeverity
                    : AiTraceSeverity.Information);
            }
            catch (Exception e)
            {
                localContext.TracingService.Trace($"Unhandled Exception: {e.Message}");
            }
        }
    }
}