using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;

namespace D365AppInsights.Workflow
{
    public sealed class GetCurrentTimeUtc : WorkFlowActivityBase
    {
        [Output("Current Time (UTC)")]
        public OutArgument<DateTime> CurrentTimeUtc { get; set; }

        public GetCurrentTimeUtc() : base(typeof(GetCurrentTimeUtc)) { }
        protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (localContext == null)
                throw new ArgumentNullException(nameof(localContext));

            DateTime x = TimeProvider.UtcNow;

            CurrentTimeUtc.Set(context, TimeProvider.UtcNow);
        }
    }
}