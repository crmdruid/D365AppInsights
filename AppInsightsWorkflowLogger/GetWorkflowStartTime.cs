using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Globalization;

namespace AppInsightsWorkflowLogger
{
    public sealed class GetWorkflowStartTime : WorkFlowActivityBase
    {
        [Output("Workflow Start Time")]
        public OutArgument<string> WorkflowStartTime { get; set; }

        public GetWorkflowStartTime() : base(typeof(GetWorkflowStartTime)) { }
        protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (localContext == null)
                throw new ArgumentNullException(nameof(localContext));

            WorkflowStartTime.Set(context, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture));
        }
    }
}