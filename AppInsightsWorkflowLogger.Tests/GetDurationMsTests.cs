using FakeXrmEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace AppInsightsWorkflowLogger.Tests
{
    [TestClass]
    public class GetDurationMsTests
    {
        [TestMethod]
        public void GetCurrentTimeTest()
        {
            XrmFakedWorkflowContext workflowContext = new XrmFakedWorkflowContext();

            var inputs = new Dictionary<string, object>
            {
                { "StartTime", new DateTime(2018, 2, 2, 10, 0, 0, 0, 0) }
            };

            XrmFakedContext xrmFakedContext = new XrmFakedContext();

            var result = xrmFakedContext.ExecuteCodeActivity<GetDurationMs>(workflowContext, inputs);

            bool isValidDateTime = DateTime.TryParse(result["CurrentTime"].ToString(), out DateTime date);

            Assert.IsTrue(isValidDateTime);
        }
    }
}