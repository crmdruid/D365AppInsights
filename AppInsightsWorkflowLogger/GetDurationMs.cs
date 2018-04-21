using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;

namespace AppInsightsWorkflowLogger
{
    public sealed class GetDurationMs : WorkFlowActivityBase
    {
        [Input("Start Time (UTC)")]
        [RequiredArgument]
        public InArgument<DateTime> StartTime { get; set; }

        [Output("Current Time")]
        public OutArgument<int> DurationMs { get; set; }

        public GetDurationMs() : base(typeof(GetDurationMs)) { }
        protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (localContext == null)
                throw new ArgumentNullException(nameof(localContext));

            DateTime startTime = StartTime.Get(context);

            TimeSpan difference = startTime - DateTime.UtcNow;

            DurationMs.Set(context, difference.TotalMilliseconds);
        }
    }
}