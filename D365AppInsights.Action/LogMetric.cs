using JLattimer.D365AppInsights;
using System;

namespace D365AppInsights.Action
{
    public class LogMetric : PluginBase
    {
        #region Constructor/Configuration
        private readonly string _unsecureConfig;
        private readonly string _secureConfig;

        public LogMetric(string unsecure, string secure)
            : base(typeof(LogMetric))
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

                string name = ActionHelpers.GetInputValue<string>("name", localContext.PluginExecutionContext, localContext.TracingService);
                int? value = ActionHelpers.GetInputValue<int?>("value", localContext.PluginExecutionContext, localContext.TracingService);
                int? count = ActionHelpers.GetInputValue<int?>("count", localContext.PluginExecutionContext, localContext.TracingService);
                int? min = ActionHelpers.GetInputValue<int?>("min", localContext.PluginExecutionContext, localContext.TracingService);
                int? max = ActionHelpers.GetInputValue<int?>("max", localContext.PluginExecutionContext, localContext.TracingService);
                int? stdDev = ActionHelpers.GetInputValue<int?>("stddev", localContext.PluginExecutionContext, localContext.TracingService);

                if (string.IsNullOrEmpty(name) || value == null)
                {
                    var errorMessage = string.IsNullOrEmpty(name)
                        ? "Name must be populated"
                        : "Value must be populated";

                    localContext.TracingService.Trace(errorMessage);
                    ActionHelpers.SetOutputParameters(localContext.PluginExecutionContext.OutputParameters, false, errorMessage);
                    return;
                }

                bool result = aiLogger.WriteMetric(name, (int)value, count, min, max, stdDev);

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