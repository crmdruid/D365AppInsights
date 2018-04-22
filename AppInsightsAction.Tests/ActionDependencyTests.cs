using FakeXrmEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;

namespace AppInsightsAction.Tests
{
    [TestClass]
    public class ActionDependencyTests
    {
        [TestMethod]
        public void ActionDependencyTest()
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

            xrmFakedPluginExecution.InputParameters = GetInputParameters();

            xrmFakedPluginExecution.OutputParameters = new ParameterCollection();

            context.ExecutePluginWithConfigurations<LogDependency>(xrmFakedPluginExecution, null, secureConfig);

            Assert.IsTrue((bool)xrmFakedPluginExecution.OutputParameters["logsuccess"]);
        }

        [TestMethod]
        public void ActionDependency_Missing_Name_Test()
        {
            AiSecureConfig aiSecureConfig =
                AppInsightsShared.Tests.Configs.GetAiSecureConfig(false, false, false, false, false, false);

            string secureConfig = SerializationHelper.SerializeObject<AiSecureConfig>(aiSecureConfig);

            XrmFakedContext context = new XrmFakedContext();

            XrmFakedPluginExecutionContext xrmFakedPluginExecution = new XrmFakedPluginExecutionContext();

            xrmFakedPluginExecution.InputParameters = GetInputParameters();
            xrmFakedPluginExecution.InputParameters.Remove("name");

            xrmFakedPluginExecution.OutputParameters = new ParameterCollection();

            context.ExecutePluginWithConfigurations<LogDependency>(xrmFakedPluginExecution, null, secureConfig);

            Assert.IsFalse((bool)xrmFakedPluginExecution.OutputParameters["logsuccess"]);
            Assert.IsTrue(xrmFakedPluginExecution.OutputParameters["errormessage"].ToString() == "Name must be populated");
        }

        [TestMethod]
        public void ActionDependency_Missing_Duration_Test()
        {
            AiSecureConfig aiSecureConfig =
                AppInsightsShared.Tests.Configs.GetAiSecureConfig(false, false, false, false, false, false);

            string secureConfig = SerializationHelper.SerializeObject<AiSecureConfig>(aiSecureConfig);

            XrmFakedContext context = new XrmFakedContext();

            XrmFakedPluginExecutionContext xrmFakedPluginExecution = new XrmFakedPluginExecutionContext();

            xrmFakedPluginExecution.InputParameters = GetInputParameters();
            xrmFakedPluginExecution.InputParameters.Remove("duration");

            xrmFakedPluginExecution.OutputParameters = new ParameterCollection();

            context.ExecutePluginWithConfigurations<LogDependency>(xrmFakedPluginExecution, null, secureConfig);

            Assert.IsFalse((bool)xrmFakedPluginExecution.OutputParameters["logsuccess"]);
            Assert.IsTrue(xrmFakedPluginExecution.OutputParameters["errormessage"].ToString() == "Duration must be populated");
        }

        [TestMethod]
        public void ActionDependency_Missing_Type_Test()
        {
            AiSecureConfig aiSecureConfig =
                AppInsightsShared.Tests.Configs.GetAiSecureConfig(false, false, false, false, false, false);

            string secureConfig = SerializationHelper.SerializeObject<AiSecureConfig>(aiSecureConfig);

            XrmFakedContext context = new XrmFakedContext();

            XrmFakedPluginExecutionContext xrmFakedPluginExecution = new XrmFakedPluginExecutionContext();

            xrmFakedPluginExecution.InputParameters = GetInputParameters();
            xrmFakedPluginExecution.InputParameters.Remove("type");

            xrmFakedPluginExecution.OutputParameters = new ParameterCollection();

            context.ExecutePluginWithConfigurations<LogDependency>(xrmFakedPluginExecution, null, secureConfig);

            Assert.IsFalse((bool)xrmFakedPluginExecution.OutputParameters["logsuccess"]);
            Assert.IsTrue(xrmFakedPluginExecution.OutputParameters["errormessage"].ToString() == "Type must be populated");
        }

        [TestMethod]
        public void ActionDependency_Missing_Success_Test()
        {
            AiSecureConfig aiSecureConfig =
                AppInsightsShared.Tests.Configs.GetAiSecureConfig(false, false, false, false, false, false);

            string secureConfig = SerializationHelper.SerializeObject<AiSecureConfig>(aiSecureConfig);

            XrmFakedContext context = new XrmFakedContext();

            XrmFakedPluginExecutionContext xrmFakedPluginExecution = new XrmFakedPluginExecutionContext();

            xrmFakedPluginExecution.InputParameters = GetInputParameters();
            xrmFakedPluginExecution.InputParameters.Remove("success");

            xrmFakedPluginExecution.OutputParameters = new ParameterCollection();

            context.ExecutePluginWithConfigurations<LogDependency>(xrmFakedPluginExecution, null, secureConfig);

            Assert.IsFalse((bool)xrmFakedPluginExecution.OutputParameters["logsuccess"]);
            Assert.IsTrue(xrmFakedPluginExecution.OutputParameters["errormessage"].ToString() == "Success must be populated");
        }

        private static ParameterCollection GetInputParameters()
        {
            return new ParameterCollection {
                new System.Collections.Generic.KeyValuePair<string, object>("name", "https://www.testapi.com/user/7"),
                new System.Collections.Generic.KeyValuePair<string, object>("method", "GET"),
                new System.Collections.Generic.KeyValuePair<string, object>("type", "HTTP"),
                new System.Collections.Generic.KeyValuePair<string, object>("duration", 213),
                new System.Collections.Generic.KeyValuePair<string, object>("resultcode", 200),
                new System.Collections.Generic.KeyValuePair<string, object>("success", true),
                new System.Collections.Generic.KeyValuePair<string, object>("data", "Hello from DependencyTest - 1")
            };
        }
    }
}