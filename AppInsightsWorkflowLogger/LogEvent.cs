using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;

namespace AppInsightsWorkflowLogger
{
    public sealed class LogEvent : WorkFlowActivityBase
    {
        [RequiredArgument]
        [Input("Name")]
        public InArgument<string> Name { get; set; }

        [RequiredArgument]
        [Input("Measurement Name")]
        public InArgument<string> MeasurementName { get; set; }

        [RequiredArgument]
        [Input("Measurement Value")]
        public InArgument<double> MeasurementValue { get; set; }

        [Output("Log Success")]
        public OutArgument<bool> LogSuccess { get; set; }

        public LogEvent() : base(typeof(LogEvent)) { }
        protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (localContext == null)
                throw new ArgumentNullException(nameof(localContext));

            string name = Name.Get(context);
            string measurementName = MeasurementName.Get(context);
            double measurementValue = MeasurementValue.Get(context);

            string measurementNameValidationResult = AiEvent.ValidateMeasurementName(measurementName);
            if (!string.IsNullOrEmpty(measurementNameValidationResult))
            {
                localContext.TracingService.Trace(measurementNameValidationResult);
                LogSuccess.Set(context, false);
                return;
            }

            OrganizationRequest request = new OrganizationRequest
            {
                RequestName = "lat_ApplicationInsightsLogEvent",
                Parameters = new ParameterCollection
                {
                    new KeyValuePair<string, object>("name", name),
                    new KeyValuePair<string, object>("measurementname", measurementName),
                    new KeyValuePair<string, object>("measurementvalue", measurementValue)
                }
            };

            OrganizationResponse response = localContext.OrganizationService.Execute(request);

            bool hasLogSuccess = response.Results.TryGetValue("logsuccess", out object objLogSuccess);
            if (hasLogSuccess)
            {
                LogSuccess.Set(context, (bool)objLogSuccess);
                return;
            }

            LogSuccess.Set(context, false);
        }
    }
}