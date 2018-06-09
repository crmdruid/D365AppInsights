using JLattimer.D365AppInsights;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;

namespace D365AppInsights.Workflow
{
    public sealed class LogTrace : WorkFlowActivityBase
    {
        [RequiredArgument]
        [Input("AI Setup JSON")]
        public InArgument<string> AiSetupJson { get; set; }

        [RequiredArgument]
        [Input("Message")]
        public InArgument<string> Message { get; set; }

        [RequiredArgument]
        [Input("Severity (Valid values: Verbose, Information, Warning, Error, Critical)")]
        public InArgument<string> Severity { get; set; }

        [Output("Log Success")]
        public OutArgument<bool> LogSuccess { get; set; }

        public LogTrace() : base(typeof(LogTrace)) { }
        protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (localContext == null)
                throw new ArgumentNullException(nameof(localContext));

            string aiSetupJson = AiSetupJson.Get(context);
            AiLogger aiLogger = new AiLogger(aiSetupJson, localContext.OrganizationService, localContext.TracingService,
                localContext.WorkflowExecutionContext, null, localContext.WorkflowExecutionContext.WorkflowCategory);

            string message = Message.Get(context);
            string severity = Severity.Get(context);

            string severityValidationResult = AiTrace.ValidateSeverityValue(severity);
            if (!string.IsNullOrEmpty(severityValidationResult))
            {
                localContext.TracingService.Trace(severityValidationResult);
                LogSuccess.Set(context, false);
                return;
            }

            Enum.TryParse(severity, out AiTraceSeverity traceSeverity);

            bool logSuccess = aiLogger.WriteTrace(message, traceSeverity);

            LogSuccess.Set(context, logSuccess);
        }
    }
}