using FakeXrmEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;

namespace AppInsightsAction.Tests
{
    [TestClass]
    public class ActionMetricTests
    {
        [TestMethod]
        public void ActionMetricTest()
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
                new System.Collections.Generic.KeyValuePair<string, object>("name", "Hello from MetricTest - 1"),
                new System.Collections.Generic.KeyValuePair<string, object>("kind", 0),
                new System.Collections.Generic.KeyValuePair<string, object>("value", 34)
                //,new System.Collections.Generic.KeyValuePair<string, object>("count", 0),
                //new System.Collections.Generic.KeyValuePair<string, object>("min", 0),
                //new System.Collections.Generic.KeyValuePair<string, object>("max", 0)
            };

            xrmFakedPluginExecution.OutputParameters = new ParameterCollection();

            context.ExecutePluginWithConfigurations<LogMetric>(xrmFakedPluginExecution, null, secureConfig);
        }
    }
}