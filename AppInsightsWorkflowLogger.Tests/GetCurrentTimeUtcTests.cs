using FakeXrmEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AppInsightsWorkflowLogger.Tests
{
    [TestClass]
    public class GetCurrentTimeUtcTests
    {
        [TestMethod]
        public void GetCurrentTimeUtcTest()
        {
            XrmFakedWorkflowContext workflowContext = new XrmFakedWorkflowContext();

            XrmFakedContext xrmFakedContext = new XrmFakedContext();
            
            var result = xrmFakedContext.ExecuteCodeActivity<GetCurrentTimeUtc>(workflowContext);

            bool isValidDateTime = DateTime.TryParse(result["CurrentTimeUtc"].ToString(), out DateTime date);

            Assert.IsTrue(isValidDateTime);
        }
    }
}