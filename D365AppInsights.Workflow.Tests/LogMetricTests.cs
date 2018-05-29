using FakeXrmEasy;
using FakeXrmEasy.FakeMessageExecutors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;

namespace D365AppInsights.Workflow.Tests
{
    [TestClass]
    public class LogMetricTests
    {
        #region Test Initialization and Cleanup

        // Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void ClassInitialize(TestContext testContext) { }

        // Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void ClassCleanup() { }

        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void TestMethodInitialize() { }

        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void TestMethodCleanup() { }

        #endregion

        [TestMethod]
        public void Metric_Measurement_Test()
        {
            XrmFakedWorkflowContext workflowContext = new XrmFakedWorkflowContext();

            var inputs = new Dictionary<string, object>
            {
                { "Name", "Hello from TraceTest - 2"},
                { "MetricValue", 456 }
            };

            XrmFakedContext xrmFakedContext = new XrmFakedContext();
            var fakeLogActionExecutor = new FakeLogActionExecutor("lat_ApplicationInsightsLogMetric");
            xrmFakedContext.AddFakeMessageExecutor<OrganizationRequest>(fakeLogActionExecutor);

            var result = xrmFakedContext.ExecuteCodeActivity<LogMetric>(workflowContext, inputs);

            Assert.IsTrue(bool.Parse(result["LogSuccess"].ToString()));
        }

        [TestMethod]
        public void Metric_Measurement_Missing_Name_Test()
        {
            XrmFakedWorkflowContext workflowContext = new XrmFakedWorkflowContext();

            var inputs = new Dictionary<string, object>
            {
                { "Name", null},
                { "MetricValue", 456 }
            };

            XrmFakedContext xrmFakedContext = new XrmFakedContext();
            var fakeLogActionExecutor = new FakeLogActionExecutor("lat_ApplicationInsightsLogMetric");
            xrmFakedContext.AddFakeMessageExecutor<OrganizationRequest>(fakeLogActionExecutor);

            var result = xrmFakedContext.ExecuteCodeActivity<LogMetric>(workflowContext, inputs);

            Assert.IsFalse(bool.Parse(result["LogSuccess"].ToString()));
        }

        [TestMethod]
        public void Metric_Aggregation_Test()
        {
            XrmFakedWorkflowContext workflowContext = new XrmFakedWorkflowContext();

            var inputs = new Dictionary<string, object>
            {
                { "Name", "Hello from TraceTest - 2"},
                { "MetricValue", 456 },
                { "Count", 1 },
                { "Min", 456 },
                { "Max", 456 },
                { "StdDev", 0 }
            };

            XrmFakedContext xrmFakedContext = new XrmFakedContext();
            var fakeLogActionExecutor = new FakeLogActionExecutor("lat_ApplicationInsightsLogMetric");
            xrmFakedContext.AddFakeMessageExecutor<OrganizationRequest>(fakeLogActionExecutor);

            var result = xrmFakedContext.ExecuteCodeActivity<LogMetric>(workflowContext, inputs);

            Assert.IsTrue(bool.Parse(result["LogSuccess"].ToString()));
        }

        private class FakeLogActionExecutor : IFakeMessageExecutor
        {
            private readonly string _actionName;

            public FakeLogActionExecutor(string actionName)
            {
                _actionName = actionName;
            }

            public bool CanExecute(OrganizationRequest request)
            {
                return request.RequestName == _actionName;
            }

            public Type GetResponsibleRequestType()
            {
                return typeof(OrganizationResponse);
            }

            public OrganizationResponse Execute(OrganizationRequest request, XrmFakedContext ctx)
            {
                OrganizationResponse response = new OrganizationResponse();
                response.Results.Add(new KeyValuePair<string, object>("logsuccess", true));
                response.Results.Add(new KeyValuePair<string, object>("errormessage", null));

                return response;
            }
        }
    }
}