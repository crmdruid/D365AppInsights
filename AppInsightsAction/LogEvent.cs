using System;
using System.Collections.Generic;

namespace AppInsightsAction
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

                string name = Helpers.GetInputValue<string>("name", localContext.PluginExecutionContext, localContext.TracingService);
                string measurementName = Helpers.GetInputValue<string>("measurementname", localContext.PluginExecutionContext, localContext.TracingService);
                double? measurementValue = Helpers.GetInputValue<double?>("measurementvalue", localContext.PluginExecutionContext, localContext.TracingService);

                if (string.IsNullOrEmpty(name))
                {
                    localContext.PluginExecutionContext.OutputParameters.AddRange(Helpers.SetOutputParameters(false, "Name must be populated"));
                    return;
                }

                string measurementNameValidationResult = AiEvent.ValidateMeasurementName(measurementName);
                if (!string.IsNullOrEmpty(measurementNameValidationResult))
                {
                    localContext.PluginExecutionContext.OutputParameters.AddRange(Helpers.SetOutputParameters(false, measurementNameValidationResult));
                    return;
                }

                Dictionary<string, double?> measurements = new Dictionary<string, double?>();
                if (measurementValue != null)
                    measurements.Add(measurementName, (double)measurementValue);
                else
                    measurements.Add(measurementName, null);

                bool result = aiLogger.WriteEvent(name, measurements);

                localContext.PluginExecutionContext.OutputParameters.AddRange(Helpers.SetOutputParameters(result, null));
            }
            catch (Exception e)
            {
                localContext.PluginExecutionContext.OutputParameters.AddRange(Helpers.SetOutputParameters(false, e.Message));
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