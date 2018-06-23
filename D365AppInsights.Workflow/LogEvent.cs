using JLattimer.D365AppInsights;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;

namespace D365AppInsights.Workflow
{
    public sealed class LogEvent : WorkFlowActivityBase
    {
        [RequiredArgument]
        [Input("AI Setup JSON")]
        public InArgument<string> AiSetupJson { get; set; }

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

            string aiSetupJson = AiSetupJson.Get(context);
            AiLogger aiLogger = new AiLogger(aiSetupJson, localContext.OrganizationService, localContext.TracingService,
                localContext.WorkflowExecutionContext, null, localContext.WorkflowExecutionContext.WorkflowCategory);

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

            Dictionary<string, double> measurements =
                new Dictionary<string, double> { { measurementName, Convert.ToDouble(measurementValue) } };

            bool logSuccess = aiLogger.WriteEvent(name, measurements);

            LogSuccess.Set(context, logSuccess);
        }
    }
}