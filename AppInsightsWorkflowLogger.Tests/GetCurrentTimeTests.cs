using FakeXrmEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AppInsightsWorkflowLogger.Tests
{
    [TestClass]
    public class GetCurrentTimeTests
    {
        [TestMethod]
        public void GetCurrentTimeTest()
        {
            XrmFakedWorkflowContext workflowContext = new XrmFakedWorkflowContext();

            XrmFakedContext xrmFakedContext = new XrmFakedContext();

            var result = xrmFakedContext.ExecuteCodeActivity<GetCurrentTime>(workflowContext);

            bool isValidDateTime = DateTime.TryParse(result["CurrentTime"].ToString(), out DateTime date);

            Assert.IsTrue(isValidDateTime);
        }
    }
}