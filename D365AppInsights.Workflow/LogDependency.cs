using JLattimer.D365AppInsights;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;

namespace D365AppInsights.Workflow
{
    public sealed class LogDependency : WorkFlowActivityBase
    {
        [RequiredArgument]
        [Input("AI Setup JSON")]
        public InArgument<string> AiSetupJson { get; set; }

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
        [Default("true")]
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

            string aiSetupJson = AiSetupJson.Get(context);
            AiLogger aiLogger = new AiLogger(aiSetupJson, localContext.OrganizationService, localContext.TracingService,
                localContext.WorkflowExecutionContext, null, localContext.WorkflowExecutionContext.WorkflowCategory);

            string name = Name.Get(context);
            string method = Method.Get(context);
            string type = Type.Get(context);
            int duration = Duration.Get(context);
            int? resultCode = ResultCode.Get(context);
            bool success = Success.Get(context);
            string data = Data.Get(context);

            bool logSuccess = aiLogger.WriteDependency(name, method, type, duration, resultCode, success, data);

            LogSuccess.Set(context, logSuccess);
        }
    }
}