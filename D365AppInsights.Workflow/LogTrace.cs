using JLattimer.D365AppInsights;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;

namespace D365AppInsights.Workflow
{
    public sealed class LogTrace : WorkFlowActivityBase
    {
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

            string message = Message.Get(context);
            string severity = Severity.Get(context);

            string severityValidationResult = AiTrace.ValidateSeverityValue(severity);
            if (!string.IsNullOrEmpty(severityValidationResult))
            {
                localContext.TracingService.Trace(severityValidationResult);
                LogSuccess.Set(context, false);
                return;
            }

            OrganizationRequest request = new OrganizationRequest
            {
                RequestName = "lat_ApplicationInsightsLogTrace",
                Parameters = new ParameterCollection
                {
                    new KeyValuePair<string, object>("message", message),
                    new KeyValuePair<string, object>("severity", severity)
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