using JLattimer.D365AppInsights;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;

namespace D365AppInsights.Workflow
{
    public sealed class LogMetric : WorkFlowActivityBase
    {
        [RequiredArgument]
        [Input("AI Setup JSON")]
        public InArgument<string> AiSetupJson { get; set; }

        [RequiredArgument]
        [Input("Name")]
        public InArgument<string> Name { get; set; }

        [RequiredArgument]
        [Input("Metric Value")]
        public InArgument<int> MetricValue { get; set; }

        [Input("Count")]
        public InArgument<int> Count { get; set; }

        [Input("Min")]
        public InArgument<int> Min { get; set; }

        [Input("Max")]
        public InArgument<int> Max { get; set; }

        [Input("StdDev")]
        [Default("0")]
        public InArgument<int> StdDev { get; set; }

        [Output("Log Success")]
        public OutArgument<bool> LogSuccess { get; set; }

        public LogMetric() : base(typeof(LogMetric)) { }
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
            int value = MetricValue.Get(context);
            int? count = Count.Get(context);
            int? min = Min.Get(context);
            int? max = Max.Get(context);
            int stdDev = StdDev.Get(context);

            bool logSuccess = aiLogger.WriteMetric(name, value, count, min, max, stdDev);

            LogSuccess.Set(context, logSuccess);
        }
    }
}