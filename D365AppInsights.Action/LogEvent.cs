using JLattimer.D365AppInsights;
using System;
using System.Collections.Generic;

namespace D365AppInsights.Action
{
    public class LogEvent : PluginBase
    {
        #region Constructor/Configuration
        private readonly string _unsecureConfig;
        private readonly string _secureConfig;

        public LogEvent(string unsecure, string secure)
            : base(typeof(LogEvent))
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
                string measurementName = ActionHelpers.GetInputValue<string>("measurementname", localContext.PluginExecutionContext, localContext.TracingService);
                float? measurementValue = ActionHelpers.GetFloatInput("measurementvalue", localContext.PluginExecutionContext, localContext.TracingService);

                if (string.IsNullOrEmpty(name))
                {
                    localContext.TracingService.Trace("Name must be populated");
                    ActionHelpers.SetOutputParameters(localContext.PluginExecutionContext.OutputParameters, false, "Name must be populated");
                    return;
                }

                string measurementNameValidationResult = AiEvent.ValidateMeasurementName(measurementName);
                if (!string.IsNullOrEmpty(measurementNameValidationResult))
                {
                    localContext.TracingService.Trace(measurementNameValidationResult);
                    ActionHelpers.SetOutputParameters(localContext.PluginExecutionContext.OutputParameters, false, measurementNameValidationResult);
                    return;
                }

                Dictionary<string, double?> measurements = new Dictionary<string, double?>();
                if (measurementValue != null)
                    measurements.Add(measurementName, Convert.ToDouble(measurementValue));
                else
                    measurements.Add(measurementName, null);

                bool result = aiLogger.WriteEvent(name, measurements);

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