using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;

namespace D365AppInsights.Workflow
{
    public sealed class LogDependency : WorkFlowActivityBase
    {
        [RequiredArgument]
        [Input("Name")]
        public InArgument<string> Name { get; set; }

        [Input("Method")]
        public InArgument<string> Method { get; set; }

        [RequiredArgument]
        [Input("Type")]
        public InArgument<string> Type { get; set; }

        [RequiredArgument]
        [Input("Duration (ms)")]
        public InArgument<int> Duration { get; set; }

        [Input("Result Code")]
        public InArgument<int> ResultCode { get; set; }

        [RequiredArgument]
        [Input("Success")]
        public InArgument<bool> Success { get; set; }

        [Input("Data")]
        public InArgument<string> Data { get; set; }

        [Output("Log Success")]
        public OutArgument<bool> LogSuccess { get; set; }

        public LogDependency() : base(typeof(LogDependency)) { }
        protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (localContext == null)
                throw new ArgumentNullException(nameof(localContext));

            string name = Name.Get(context);
            string method = Method.Get(context);
            string type = Type.Get(context);
            int? duration = Duration.Get(context);
            int? resultCode = ResultCode.Get(context);
            bool success = Success.Get(context);
            string data = Data.Get(context);

            if (string.IsNullOrEmpty(name))
            {
                localContext.TracingService.Trace("Name must be populated");
                LogSuccess.Set(context, false);
                return;
            }

            OrganizationRequest request = new OrganizationRequest
            {
                RequestName = "lat_ApplicationInsightsLogDependency",
                Parameters = new ParameterCollection
                {
                    new KeyValuePair<string, object>("name", name),
                    new KeyValuePair<string, object>("method", method),
                    new KeyValuePair<string, object>("type", type),
                    new KeyValuePair<string, object>("duration", duration),
                    new KeyValuePair<string, object>("resultcode", resultCode),
                    new KeyValuePair<string, object>("success", success),
                    new KeyValuePair<string, object>("data", data)
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