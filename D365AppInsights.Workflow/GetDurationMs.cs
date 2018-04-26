using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;

namespace D365AppInsights.Workflow
{
    public sealed class GetDurationMs : WorkFlowActivityBase
    {
        [Input("Start Time (UTC)")]
        public InArgument<DateTime> StartTime { get; set; }

        [Output("Duration")]
        public OutArgument<int> DurationMs { get; set; }

        public GetDurationMs() : base(typeof(GetDurationMs)) { }
        protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (localContext == null)
                throw new ArgumentNullException(nameof(localContext));

            DateTime startTimeInput = StartTime.Get(context);

            localContext.TracingService.Trace("Input: " + startTimeInput.ToString("yyyy-MM-dd HH:mm:ss.fff"));

            DateTime startTime = startTimeInput == DateTime.MinValue
                ? localContext.WorkflowExecutionContext.OperationCreatedOn
                : startTimeInput;

            localContext.TracingService.Trace("StartTime: " + startTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));

            DateTime endTime = TimeProvider.UtcNow;

            localContext.TracingService.Trace("EndTime: " + endTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));

            TimeSpan difference = endTime - startTime;

            DurationMs.Set(context, Convert.ToInt32(difference.TotalMilliseconds));
        }
    }
}