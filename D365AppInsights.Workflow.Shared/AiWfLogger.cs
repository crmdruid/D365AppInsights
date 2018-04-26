using Microsoft.Xrm.Sdk.Workflow;
using System.Collections.Generic;
using System;

namespace AppInsightsLogger.Workflow
{
    public class AiWfLogger
    {
        public static Dictionary<string, object> GetWorkflowExecutionContextDetails(IWorkflowContext workflowContext)
        {
            Dictionary<string, object> workflowContextDetails = new Dictionary<string, object>
            {
                {"WorkflowCategory", GetWorkflowCategoryName(workflowContext.WorkflowCategory)}
            };

            return workflowContextDetails;
        }

        private static string GetWorkflowCategoryName(int category)
        {
            switch (category)
            {
                case 0:
                    return "Workflow";
                case 1:
                    return "Dialog";
                case 2:
                    return "Business Rule";
                case 3:
                    return "Action";
                case 4:
                    return "Business Process Flow";
                default:
                    return "Unknown";
            }
        }
    }
}