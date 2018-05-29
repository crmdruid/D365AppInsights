using JLattimer.D365AppInsights;
using System;

namespace D365AppInsights.Action
{
    public class LogTrace : PluginBase
    {
        #region Constructor/Configuration
        private readonly string _secureConfig;
        private string _unsecureConfig;

        public LogTrace(string unsecure, string secureConfig)
            : base(typeof(LogTrace))
        {
            _secureConfig = secureConfig;
            _unsecureConfig = unsecure;
        }
        #endregion

        protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {
            if (localContext == null)
                throw new ArgumentNullException(nameof(localContext));

            try
            {
                AiLogger aiLogger = new AiLogger(_secureConfig, localContext.OrganizationService, localContext.TracingService, localContext.PluginExecutionContext);

                string message = ActionHelpers.GetInputValue<string>("message", localContext.PluginExecutionContext, localContext.TracingService);
                string severity = ActionHelpers.GetInputValue<string>("severity", localContext.PluginExecutionContext, localContext.TracingService);

                string severityValidationResult = AiTrace.ValidateSeverityValue(severity);
                if (!string.IsNullOrEmpty(severityValidationResult))
                {
                    ActionHelpers.SetOutputParameters(localContext.PluginExecutionContext.OutputParameters, false, severityValidationResult);
                    return;
                }

                bool isValid = Enum.TryParse(severity, out AiTraceSeverity traceSeverity);

                bool result = aiLogger.WriteTrace(DateTime.UtcNow, message, isValid
                    ? traceSeverity
                    : AiTraceSeverity.Information);

                ActionHelpers.SetOutputParameters(localContext.PluginExecutionContext.OutputParameters, result, null);
            }
            catch (Exception e)
            {
                ActionHelpers.SetOutputParameters(localContext.PluginExecutionContext.OutputParameters, false, e.Message);
            }
        }
    }
}