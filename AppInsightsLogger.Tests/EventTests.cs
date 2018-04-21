using FakeXrmEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;

namespace AppInsightsLogger.Tests
{
    [TestClass]
    public class EventTests
    {
        [TestMethod]
        public void EventTest()
        {
            AiSecureConfig aiSecureConfig =
                AppInsightsShared.Tests.Configs.GetAiSecureConfig(false, false, false, false, false, false);

            string secureConfig = SerializationHelper.SerializeObject<AiSecureConfig>(aiSecureConfig);

            XrmFakedTracingService fakedTracingService = new XrmFakedTracingService();
            XrmFakedContext context = new XrmFakedContext();
            IOrganizationService fakedService = context.GetOrganizationService();

            XrmFakedPluginExecutionContext xrmFakedPluginExecution = new XrmFakedPluginExecutionContext();
            Guid userId = Guid.Parse("9e7ec57b-3a08-4a41-a4d4-354d66f19b65");
            xrmFakedPluginExecution.InitiatingUserId = userId;
            xrmFakedPluginExecution.UserId = userId;
            xrmFakedPluginExecution.CorrelationId = Guid.Parse("15cc775b-9ebc-48d1-93a6-b0ce9c920b66");
            xrmFakedPluginExecution.PrimaryEntityName = "account";
            xrmFakedPluginExecution.PrimaryEntityId = Guid.Parse("f14c4d40-96e9-40a5-95b7-4028af9605de");
            xrmFakedPluginExecution.MessageName = "update";
            xrmFakedPluginExecution.Mode = 1;
            xrmFakedPluginExecution.Depth = 1;
            xrmFakedPluginExecution.OrganizationName = "test.crm.dynamics.com";
            xrmFakedPluginExecution.Stage = 40;

            xrmFakedPluginExecution.InputParameters = new ParameterCollection {
                new KeyValuePair<string, object>("Param1", "test"),
                new KeyValuePair<string, object>("Param2", 34)
            };

            AiLogger aiLogger = new AiLogger(secureConfig, fakedService, fakedTracingService, xrmFakedPluginExecution);

            Dictionary<string, double?> measurements = new Dictionary<string, double?> {
                { "Click1", 33 }
            };

            bool result = aiLogger.WriteEvent("Hello from EventTest - 0", measurements);

            Assert.IsTrue(result);
        }
    }
}