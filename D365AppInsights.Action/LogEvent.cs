using JLattimer.D365AppInsights;
using System;
using System.Collections.Generic;

namespace D365AppInsights.Action
{
    public class LogEvent : PluginBase
    {
        #region Constructor/Configuration
        private readonly string _secureConfig;
        private string _unsecureConfig;

        public LogEvent(string unsecure, string secureConfig)
            : base(typeof(LogEvent))
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
                string measurementName = ActionHelpers.GetInputValue<string>("measurementname", localContext.PluginExecutionContext, localContext.TracingService);
                float? measurementValue = ActionHelpers.GetFloatInput("measurementvalue", localContext.PluginExecutionContext, localContext.TracingService);

                if (string.IsNullOrEmpty(name))
                {
                    ActionHelpers.SetOutputParameters(localContext.PluginExecutionContext.OutputParameters, false, "Name must be populated");
                    return;
                }

                string measurementNameValidationResult = AiEvent.ValidateMeasurementName(measurementName);
                if (!string.IsNullOrEmpty(measurementNameValidationResult))
                {
                    ActionHelpers.SetOutputParameters(localContext.PluginExecutionContext.OutputParameters, false, measurementNameValidationResult);
                    return;
                }

                Dictionary<string, double?> measurements = new Dictionary<string, double?>();
                if (measurementValue != null)
                    measurements.Add(measurementName, Convert.ToDouble(measurementValue));
                else
                    measurements.Add(measurementName, null);

                bool result = aiLogger.WriteEvent(DateTime.UtcNow, name, measurements);

                ActionHelpers.SetOutputParameters(localContext.PluginExecutionContext.OutputParameters, result, null);
            }
            catch (Exception e)
            {
                ActionHelpers.SetOutputParameters(localContext.PluginExecutionContext.OutputParameters, false, e.Message);
            }
        }

        // This would have been used to convert a JSON string into the Dictionary<string, double> objects
        // Maybe this will get implemented at some point
        //private static Dictionary<string, double> GetMeasurementsFromInput(string measurementsInput, ITracingService tracer)
        //{
        //    try
        //    {
        //        JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
        //        object dictionary = jsSerializer.DeserializeObject(measurementsInput);
        //        Dictionary<string, object> unconvertedMeasurements = (Dictionary<string, object>)dictionary;

        //        Dictionary<String, double> measurements = new Dictionary<string, double>();
        //        foreach (KeyValuePair<string, object> keyValuePair in unconvertedMeasurements)
        //        {
        //            var type = keyValuePair.Value.GetType();

        //            if (type == typeof(Int32))
        //                measurements.Add(keyValuePair.Key, Convert.ToDouble(keyValuePair.Value));
        //            else if (type == typeof(double))
        //                measurements.Add(keyValuePair.Key, (double)keyValuePair.Value);
        //        }

        //        return measurements;
        //    }
        //    catch (Exception e)
        //    {
        //        tracer.Trace($"Error deserializing measurements: {e.Message}");
        //        throw;
        //    }
        //}
    }
}