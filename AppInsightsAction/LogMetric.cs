using System;

namespace AppInsightsAction
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

                string name = Helpers.GetInputValue<string>("name", localContext.PluginExecutionContext, localContext.TracingService);
                int? value = Helpers.GetInputValue<int?>("value", localContext.PluginExecutionContext, localContext.TracingService);
                int? count = Helpers.GetInputValue<int?>("count", localContext.PluginExecutionContext, localContext.TracingService);
                int? min = Helpers.GetInputValue<int?>("min", localContext.PluginExecutionContext, localContext.TracingService);
                int? max = Helpers.GetInputValue<int?>("max", localContext.PluginExecutionContext, localContext.TracingService);
                int? stdDev = Helpers.GetInputValue<int?>("stdDev", localContext.PluginExecutionContext, localContext.TracingService);

                if (string.IsNullOrEmpty(name) || value == null)
                {
                    string errorMessage;
                    if (string.IsNullOrEmpty(name))
                        errorMessage = "Name must be populated";
                    else
                        errorMessage = "Value must be populated";

                    Helpers.SetOutputParameters(localContext.PluginExecutionContext.OutputParameters, false, errorMessage);
                    return;
                }

                bool result = aiLogger.WriteMetric(name, (int)value, count, min, max, stdDev);

                Helpers.SetOutputParameters(localContext.PluginExecutionContext.OutputParameters, result, null);
            }
            catch (Exception e)
            {
                Helpers.SetOutputParameters(localContext.PluginExecutionContext.OutputParameters, false, e.Message);
            }
        }
    }
}