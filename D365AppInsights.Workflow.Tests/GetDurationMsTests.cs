using FakeXrmEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace D365AppInsights.Workflow.Tests
{
    [TestClass]
    public class GetDurationMsTests
    {
        [TestMethod]
        public void GetCurrentTime_Greater_1_Minute_No_StartTime_Test()
        {
            XrmFakedWorkflowContext workflowContext =
                new XrmFakedWorkflowContext { OperationCreatedOn = DateTime.UtcNow.AddMinutes(-1) };

            XrmFakedContext xrmFakedContext = new XrmFakedContext();


            var result = xrmFakedContext.ExecuteCodeActivity<GetDurationMs>(workflowContext);

            Assert.IsTrue((int)result["DurationMs"] > 60000);
        }

        [TestMethod]
        public void GetCurrentTime_Greater_1_Minute_Set_StartTime_Test()
        {
            XrmFakedWorkflowContext workflowContext =
                new XrmFakedWorkflowContext { OperationCreatedOn = DateTime.UtcNow.AddMinutes(-1) };

            var inputs = new Dictionary<string, object>
            {
                { "StartTime", DateTime.UtcNow.AddMinutes(-1) }
            };

            XrmFakedContext xrmFakedContext = new XrmFakedContext();

            var result = xrmFakedContext.ExecuteCodeActivity<GetDurationMs>(workflowContext, inputs);

            Assert.IsTrue((int)result["DurationMs"] > 60000);
        }
    }
}