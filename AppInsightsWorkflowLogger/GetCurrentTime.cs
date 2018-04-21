using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;

namespace AppInsightsWorkflowLogger
{
    public sealed class GetCurrentTime : WorkFlowActivityBase
    {
        [Output("Current Time")]
        public OutArgument<DateTime> CurrentTime { get; set; }

        public GetCurrentTime() : base(typeof(GetCurrentTime)) { }
        protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (localContext == null)
                throw new ArgumentNullException(nameof(localContext));

            CurrentTime.Set(context, DateTime.UtcNow);
        }
    }
}