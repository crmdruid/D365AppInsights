using System;
using FakeXrmEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

namespace AppInsightsAction.Tests
{
    [TestClass]
    public class ActionTraceTests
    {
        [TestMethod]
        public void ActionTraceTest()
        {
            AiSecureConfig aiSecureConfig =
                AppInsightsShared.Tests.Configs.GetAiSecureConfig(false, false, false, false, false, false);

            string secureConfig = SerializationHelper.SerializeObject<AiSecureConfig>(aiSecureConfig);

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

            xrmFakedPluginExecution.InputParameters = new ParameterCollection {
                new System.Collections.Generic.KeyValuePair<string, object>("message", "Hello from TraceTest - 1"),
                new System.Collections.Generic.KeyValuePair<string, object>("severity", "Warning")
            };

            xrmFakedPluginExecution.OutputParameters = new ParameterCollection();

            context.ExecutePluginWithConfigurations<LogTrace>(xrmFakedPluginExecution, null, secureConfig);
        }
    }
}