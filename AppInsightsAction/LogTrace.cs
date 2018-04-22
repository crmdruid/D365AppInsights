using System;

namespace AppInsightsAction
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

                string message = Helpers.GetInputValue<string>("message", localContext.PluginExecutionContext, localContext.TracingService);
                string severity = Helpers.GetInputValue<string>("severity", localContext.PluginExecutionContext, localContext.TracingService);

                string severityValidationResult = AiTrace.ValidateSeverityValue(severity);
                if (!string.IsNullOrEmpty(severityValidationResult))
                {
                    Helpers.SetOutputParameters(localContext.PluginExecutionContext.OutputParameters, false, severityValidationResult);
                    return;
                }

                bool isValid = Enum.TryParse(severity, out AiTraceSeverity traceSeverity);

                bool result = aiLogger.WriteTrace(message, isValid
                    ? traceSeverity
                    : AiTraceSeverity.Information);

                Helpers.SetOutputParameters(localContext.PluginExecutionContext.OutputParameters, result, null);
            }
            catch (Exception e)
            {
                Helpers.SetOutputParameters(localContext.PluginExecutionContext.OutputParameters, false, e.Message);
            }
        }
    }
}