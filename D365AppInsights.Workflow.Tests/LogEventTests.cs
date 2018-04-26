using FakeXrmEasy;
using FakeXrmEasy.FakeMessageExecutors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;

namespace D365AppInsights.Workflow.Tests
{
    [TestClass]
    public class LogEventTests
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
        public void EventTest()
        {
            XrmFakedWorkflowContext workflowContext = new XrmFakedWorkflowContext();

            var inputs = new Dictionary<string, object>
            {
                { "Name", "Hello from EventTest - 2" },
                { "MeasurementName", "TestMeasurement" },
                { "MeasurementValue", 4622d }
            };

            XrmFakedContext xrmFakedContext = new XrmFakedContext();
            var fakeLogActionExecutor = new FakeLogActionExecutor("lat_ApplicationInsightsLogEvent");
            xrmFakedContext.AddFakeMessageExecutor<OrganizationRequest>(fakeLogActionExecutor);

            var result = xrmFakedContext.ExecuteCodeActivity<LogEvent>(workflowContext, inputs);

            Assert.IsTrue(bool.Parse(result["LogSuccess"].ToString()));
        }

        [TestMethod]
        public void Event_Float_Value_Test()
        {
            XrmFakedWorkflowContext workflowContext = new XrmFakedWorkflowContext();

            var inputs = new Dictionary<string, object>
            {
                { "Name", "Test Log Event" },
                { "MeasurementName", "test" },
                { "MeasurementValue", 1.00000f }
            };

            XrmFakedContext xrmFakedContext = new XrmFakedContext();
            var fakeLogActionExecutor = new FakeLogActionExecutor("lat_ApplicationInsightsLogEvent");
            xrmFakedContext.AddFakeMessageExecutor<OrganizationRequest>(fakeLogActionExecutor);

            var result = xrmFakedContext.ExecuteCodeActivity<LogEvent>(workflowContext, inputs);

            Assert.IsTrue(bool.Parse(result["LogSuccess"].ToString()));
        }

        [TestMethod]
        public void Event_Missing_Name_Test()
        {
            XrmFakedWorkflowContext workflowContext = new XrmFakedWorkflowContext();

            var inputs = new Dictionary<string, object>
            {
                { "Name", null },
                { "MeasurementName", "TestMeasurement" },
                { "MeasurementValue", 4622d }
            };

            XrmFakedContext xrmFakedContext = new XrmFakedContext();
            var fakeLogActionExecutor = new FakeLogActionExecutor("lat_ApplicationInsightsLogEvent");
            xrmFakedContext.AddFakeMessageExecutor<OrganizationRequest>(fakeLogActionExecutor);

            var result = xrmFakedContext.ExecuteCodeActivity<LogEvent>(workflowContext, inputs);

            Assert.IsFalse(bool.Parse(result["LogSuccess"].ToString()));
        }

        [TestMethod]
        public void Event_Invalid_Measurement_Name_Test()
        {
            XrmFakedWorkflowContext workflowContext = new XrmFakedWorkflowContext();

            var inputs = new Dictionary<string, object>
            {
                { "Name", null},
                { "MeasurementName", "sisznxevfkzibsdtsvfwijucumzedrzauyzzyzqmrrmdwwdqugtiprgvgkmpokcoldnxcmlwywcuernvoobnfogzgjkbnsteycrvafpharlnylyvyigsnskjuwwqjeiudwkibztwzwwotfbaijxcqwwk" },
                { "MeasurementValue", 4622d }
            };

            XrmFakedContext xrmFakedContext = new XrmFakedContext();
            var fakeLogActionExecutor = new FakeLogActionExecutor("lat_ApplicationInsightsLogEvent");
            xrmFakedContext.AddFakeMessageExecutor<OrganizationRequest>(fakeLogActionExecutor);

            var result = xrmFakedContext.ExecuteCodeActivity<LogEvent>(workflowContext, inputs);

            Assert.IsFalse(bool.Parse(result["LogSuccess"].ToString()));
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