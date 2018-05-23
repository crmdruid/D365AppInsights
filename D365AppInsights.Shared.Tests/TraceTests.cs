using FakeXrmEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;

namespace D365AppInsights.Shared.Tests
{
    [TestClass]
    public class TraceTests
    {
        [TestMethod]
        public void TraceTest()
        {
            AiSetup aiSetup =
                AppInsightsShared.Tests.Configs.GetAiSetup(false, false, false, false, false, false, true);

            string secureConfig = SerializationHelper.SerializeObject<AiSetup>(aiSetup);

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
            xrmFakedPluginExecution.OperationCreatedOn = DateTime.Now;

            xrmFakedPluginExecution.InputParameters = new ParameterCollection {
                new System.Collections.Generic.KeyValuePair<string, object>("InputParam1", "test"),
                new System.Collections.Generic.KeyValuePair<string, object>("InputParam2", 34)
            };

            xrmFakedPluginExecution.OutputParameters = new ParameterCollection {
                new System.Collections.Generic.KeyValuePair<string, object>("OutputParam1", "test"),
                new System.Collections.Generic.KeyValuePair<string, object>("OutputParam2", 34)
            };

            AiLogger aiLogger = new AiLogger(secureConfig, fakedService, fakedTracingService, xrmFakedPluginExecution);
            bool result = aiLogger.WriteTrace(DateTime.UtcNow, "Hello from TraceTest - 0", AiTraceSeverity.Information);

            Assert.IsTrue(result);
        }
    }
}