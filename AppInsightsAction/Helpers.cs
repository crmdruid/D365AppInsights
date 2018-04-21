using Microsoft.Xrm.Sdk;

namespace AppInsightsAction
{
    public class Helpers
    {
        public static T GetInputValue<T>(string name, IPluginExecutionContext context, ITracingService tracer)
        {
            bool hasValue = context.InputParameters.TryGetValue(name, out var oValue);
            if (hasValue)
                return (T)oValue;

            tracer.Trace($"Input parameters did not contain key: {name}");
            return default(T);
        }

        public static ParameterCollection SetOutputParameters(bool logsuccess, string errormessage)
        {
            ParameterCollection outputParameters = new ParameterCollection();
            outputParameters.Add("logsuccess", logsuccess);
            outputParameters.Add("errormessage", errormessage);

            return outputParameters;
        }
    }
}