using FakeXrmEasy;
using JLattimer.D365AppInsights;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;

namespace D365AppInsights.Action.Tests
{
    [TestClass]
    public class ActionTraceTests
    {
        [TestMethod]
        public void ActionTraceTest()
        {
            AiSetup aiSetup =
                AppInsightsShared.Tests.Configs.GetAiSetup(false, false, false, false, false, false, true);

            string secureConfig = SerializationHelper.SerializeObject<AiSetup>(aiSetup);

            XrmFakedContext context = new XrmFakedContext();

            XrmFakedPluginExecutionContext xrmFakedPluginExecution = new XrmFakedPluginExecutionContext();
            Guid userId = Guid.Parse("9e7ec57b-3a08-4a41-a4d4-354d66f19b65");
            xrmFakedPluginExecution.InitiatingUserId = userId;
            xrmFakedPluginExecution.UserId = userId;
            xrmFakedPluginExecution.CorrelationId = Guid.Parse("15cc775b-9ebc-48d1-93a6-b0ce9c920b66");
            xrmFakedPluginExecution.MessageName = "update";
            xrmFakedPluginExecution.Mode = 1;
            xrmFakedPluginExecution.Depth = 1;
            xrmFakedPluginExecution.OrganizationName = "test.crm.dynamics.com";
            xrmFakedPluginExecution.Stage = 40;
            xrmFakedPluginExecution.OperationCreatedOn = DateTime.Now;

            xrmFakedPluginExecution.InputParameters = GetInputParameters();

            xrmFakedPluginExecution.OutputParameters = new ParameterCollection();

            context.ExecutePluginWithConfigurations<LogTrace>(xrmFakedPluginExecution, null, secureConfig);

            Assert.IsTrue((bool)xrmFakedPluginExecution.OutputParameters["logsuccess"]);
        }

        [TestMethod]
        public void ActionTrace_Null_Severity_Test()
        {
            AiSetup aiSetup =
                AppInsightsShared.Tests.Configs.GetAiSetup(false, false, false, false, false, false, true);

            string secureConfig = SerializationHelper.SerializeObject<AiSetup>(aiSetup);

            XrmFakedContext context = new XrmFakedContext();

            XrmFakedPluginExecutionContext xrmFakedPluginExecution =
                new XrmFakedPluginExecutionContext { InputParameters = GetInputParameters() };

            xrmFakedPluginExecution.InputParameters["severity"] = null;

            xrmFakedPluginExecution.OutputParameters = new ParameterCollection();

            context.ExecutePluginWithConfigurations<LogTrace>(xrmFakedPluginExecution, null, secureConfig);

            Assert.IsFalse((bool)xrmFakedPluginExecution.OutputParameters["logsuccess"]);
            Assert.IsTrue(xrmFakedPluginExecution.OutputParameters["errormessage"].ToString() == "Severity cannot be null");
        }

        [TestMethod]
        public void ActionTrace_Invalid_Severity_Test()
        {
            AiSetup aiSetup =
                AppInsightsShared.Tests.Configs.GetAiSetup(false, false, false, false, false, false, true);

            string secureConfig = SerializationHelper.SerializeObject<AiSetup>(aiSetup);

            XrmFakedContext context = new XrmFakedContext();

            XrmFakedPluginExecutionContext xrmFakedPluginExecution =
                new XrmFakedPluginExecutionContext { InputParameters = GetInputParameters() };

            xrmFakedPluginExecution.InputParameters["severity"] = "test";

            xrmFakedPluginExecution.OutputParameters = new ParameterCollection();

            context.ExecutePluginWithConfigurations<LogTrace>(xrmFakedPluginExecution, null, secureConfig);

            Assert.IsFalse((bool)xrmFakedPluginExecution.OutputParameters["logsuccess"]);
            Assert.IsTrue(xrmFakedPluginExecution.OutputParameters["errormessage"].ToString() == "Severity valid values: Verbose, Information, Warning, Error, Critical");
        }

        private static ParameterCollection GetInputParameters()
        {
            return new ParameterCollection {
                new KeyValuePair<string, object>("message", "Hello from TraceTest - 1"),
                new KeyValuePair<string, object>("severity", "Warning")
            };
        }
    }
}