using Microsoft.Xrm.Sdk;
using System;

namespace D365AppInsights.Action
{
    public class ActionHelpers
    {
        public static T GetInputValue<T>(string name, IPluginExecutionContext context, ITracingService tracer)
        {
            bool hasValue = context.InputParameters.TryGetValue(name, out var oValue);
            if (hasValue)
                return (T)oValue;

            tracer.Trace($"Input parameters did not contain key: {name}");
            return default(T);
        }

        public static float? GetFloatInput(string name, IPluginExecutionContext context, ITracingService tracer)
        {
            bool hasValue = context.InputParameters.TryGetValue(name, out var oValue);
            if (hasValue)
            {
                if (oValue == null)
                    return null;

                return Convert.ToSingle(oValue);
            }

            tracer.Trace($"Input parameters did not contain key: {name}");
            return null;
        }

        public static void SetOutputParameters(ParameterCollection outputParameters, bool success, string errorMessage)
        {
            outputParameters["logsuccess"] = success;
            outputParameters["errormessage"] = errorMessage;
        }
    }
}