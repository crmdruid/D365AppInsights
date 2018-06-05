using JLattimer.D365AppInsights;
using System;

namespace D365AppInsights.Action
{
    public class LogDependency : PluginBase
    {
        #region Constructor/Configuration
        private readonly string _secureConfig;
        private string _unsecureConfig;

        public LogDependency(string unsecure, string secureConfig)
            : base(typeof(LogDependency))
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
                string method = ActionHelpers.GetInputValue<string>("method", localContext.PluginExecutionContext, localContext.TracingService);
                string typeInput = ActionHelpers.GetInputValue<string>("type", localContext.PluginExecutionContext, localContext.TracingService);
                int? duration = ActionHelpers.GetInputValue<int?>("duration", localContext.PluginExecutionContext, localContext.TracingService);
                int? resultcode = ActionHelpers.GetInputValue<int?>("resultcode", localContext.PluginExecutionContext, localContext.TracingService);
                bool? success = ActionHelpers.GetInputValue<bool?>("success", localContext.PluginExecutionContext, localContext.TracingService);
                string data = ActionHelpers.GetInputValue<string>("data", localContext.PluginExecutionContext, localContext.TracingService);

                if (string.IsNullOrEmpty(name) || duration == null || string.IsNullOrEmpty(typeInput) || success == null)
                {
                    string errorMessage;
                    if (string.IsNullOrEmpty(name))
                        errorMessage = "Name must be populated";
                    else if (duration == null)
                        errorMessage = "Duration must be populated";
                    else if (string.IsNullOrEmpty(typeInput))
                        errorMessage = "Type must be populated";
                    else
                        errorMessage = "Success must be populated";

                    ActionHelpers.SetOutputParameters(localContext.PluginExecutionContext.OutputParameters, false, errorMessage);
                    return;
                }

                bool result = aiLogger.WriteDependency(name, method, typeInput, (int)duration, resultcode, (bool)success, data);

                ActionHelpers.SetOutputParameters(localContext.PluginExecutionContext.OutputParameters, result, null);
            }
            catch (Exception e)
            {
                ActionHelpers.SetOutputParameters(localContext.PluginExecutionContext.OutputParameters, false, e.Message);
            }
        }
    }
}