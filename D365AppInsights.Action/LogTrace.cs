using JLattimer.D365AppInsights;
using System;

namespace D365AppInsights.Action
{
    public class LogTrace : PluginBase
    {
        #region Constructor/Configuration
        private readonly string _unsecureConfig;
        private readonly string _secureConfig;

        public LogTrace(string unsecure, string secure)
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
                AiLogger aiLogger = new AiLogger(_unsecureConfig, localContext.OrganizationService,
                    localContext.TracingService,
                    localContext.PluginExecutionContext, localContext.PluginExecutionContext.Stage, null);

                string message = ActionHelpers.GetInputValue<string>("message", localContext.PluginExecutionContext,
                    localContext.TracingService);
                string severity = ActionHelpers.GetInputValue<string>("severity", localContext.PluginExecutionContext,
                    localContext.TracingService);

                string severityValidationResult = AiTrace.ValidateSeverityValue(severity);
                if (!string.IsNullOrEmpty(severityValidationResult))
                {
                    localContext.TracingService.Trace(severityValidationResult);
                    ActionHelpers.SetOutputParameters(localContext.PluginExecutionContext.OutputParameters, false, severityValidationResult);
                    return;
                }

                bool isValid = Enum.TryParse(severity, out AiTraceSeverity traceSeverity);

                bool result = aiLogger.WriteTrace(message, isValid
                    ? traceSeverity
                    : AiTraceSeverity.Information);

                ActionHelpers.SetOutputParameters(localContext.PluginExecutionContext.OutputParameters, result, null);
            }
            catch (Exception e)
            {
                localContext.TracingService.Trace($"Unhandled Exception: {e.Message}");
                ActionHelpers.SetOutputParameters(localContext.PluginExecutionContext.OutputParameters, false, e.Message);
            }
        }
    }
}