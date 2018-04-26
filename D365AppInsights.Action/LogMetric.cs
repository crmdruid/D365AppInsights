using System;

namespace D365AppInsights.Action
{
    public class LogMetric : PluginBase
    {
        #region Constructor/Configuration
        private readonly string _secureConfig;
        private string _unsecureConfig;

        public LogMetric(string unsecure, string secureConfig)
            : base(typeof(LogMetric))
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

                string name = ActionHelpers.GetInputValue<string>("name", localContext.PluginExecutionContext, localContext.TracingService);
                int? value = ActionHelpers.GetInputValue<int?>("value", localContext.PluginExecutionContext, localContext.TracingService);
                int? count = ActionHelpers.GetInputValue<int?>("count", localContext.PluginExecutionContext, localContext.TracingService);
                int? min = ActionHelpers.GetInputValue<int?>("min", localContext.PluginExecutionContext, localContext.TracingService);
                int? max = ActionHelpers.GetInputValue<int?>("max", localContext.PluginExecutionContext, localContext.TracingService);
                int? stdDev = ActionHelpers.GetInputValue<int?>("stdDev", localContext.PluginExecutionContext, localContext.TracingService);

                if (string.IsNullOrEmpty(name) || value == null)
                {
                    var errorMessage = string.IsNullOrEmpty(name)
                        ? "Name must be populated"
                        : "Value must be populated";

                    ActionHelpers.SetOutputParameters(localContext.PluginExecutionContext.OutputParameters, false, errorMessage);
                    return;
                }

                bool result = aiLogger.WriteMetric(name, (int)value, count, min, max, stdDev);

                ActionHelpers.SetOutputParameters(localContext.PluginExecutionContext.OutputParameters, result, null);
            }
            catch (Exception e)
            {
                ActionHelpers.SetOutputParameters(localContext.PluginExecutionContext.OutputParameters, false, e.Message);
            }
        }
    }
}