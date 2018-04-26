using FakeXrmEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;

namespace D365AppInsights.Action.Tests
{
    [TestClass]
    public class HelpersTests
    {
        [TestMethod]
        public void GetFloatInputValue()
        {
            float? expected = -195;
            const string measurementName = "name";

            XrmFakedPluginExecutionContext xrmFakedPluginExecution = new XrmFakedPluginExecutionContext
            {
                InputParameters = new ParameterCollection {
                    new KeyValuePair<string, object>(measurementName, expected)
                }
            };

            XrmFakedTracingService xrmFakedTracingService = new XrmFakedTracingService();

            float? measurementValue = ActionHelpers.GetFloatInput(measurementName, xrmFakedPluginExecution, xrmFakedTracingService);

            Assert.IsTrue(measurementValue.Equals(expected));
        }

        [TestMethod]
        public void GetNullFloatInputValue()
        {
            const string measurementName = "name";

            XrmFakedPluginExecutionContext xrmFakedPluginExecution = new XrmFakedPluginExecutionContext
            {
                InputParameters = new ParameterCollection {
                    new KeyValuePair<string, object>(measurementName, null)
                }
            };

            XrmFakedTracingService xrmFakedTracingService = new XrmFakedTracingService();

            float? measurementValue = ActionHelpers.GetFloatInput(measurementName, xrmFakedPluginExecution, xrmFakedTracingService);

            Assert.IsNull(measurementValue);
        }

        [TestMethod]
        public void GetMissingFloatInputValue()
        {
            const string measurementName = "name";

            XrmFakedPluginExecutionContext xrmFakedPluginExecution = new XrmFakedPluginExecutionContext
            {
                InputParameters = new ParameterCollection {
                    new KeyValuePair<string, object>(measurementName, null)
                }
            };

            XrmFakedTracingService xrmFakedTracingService = new XrmFakedTracingService();

            float? measurementValue = ActionHelpers.GetFloatInput("???", xrmFakedPluginExecution, xrmFakedTracingService);

            Assert.IsNull(measurementValue);
        }

        [TestMethod]
        public void GetStringInputValue()
        {
            const string measurementName = "name";
            const string expected = "test";

            XrmFakedPluginExecutionContext xrmFakedPluginExecution = new XrmFakedPluginExecutionContext
            {
                InputParameters = new ParameterCollection {
                    new KeyValuePair<string, object>(measurementName, expected)
                }
            };

            XrmFakedTracingService xrmFakedTracingService = new XrmFakedTracingService();

            string measurementValue = ActionHelpers.GetInputValue<string>(measurementName, xrmFakedPluginExecution, xrmFakedTracingService);

            Assert.IsTrue(measurementValue.Equals(expected));
        }

        [TestMethod]
        public void GetNullStringInputValue()
        {
            const string measurementName = "name";

            XrmFakedPluginExecutionContext xrmFakedPluginExecution = new XrmFakedPluginExecutionContext
            {
                InputParameters = new ParameterCollection {
                    new KeyValuePair<string, object>(measurementName, null)
                }
            };

            XrmFakedTracingService xrmFakedTracingService = new XrmFakedTracingService();

            string measurementValue = ActionHelpers.GetInputValue<string>(measurementName, xrmFakedPluginExecution, xrmFakedTracingService);

            Assert.IsNull(measurementValue);
        }

        [TestMethod]
        public void GetMissingStringInputValue()
        {
            const string measurementName = "name";

            XrmFakedPluginExecutionContext xrmFakedPluginExecution = new XrmFakedPluginExecutionContext
            {
                InputParameters = new ParameterCollection {
                    new KeyValuePair<string, object>(measurementName, null)
                }
            };

            XrmFakedTracingService xrmFakedTracingService = new XrmFakedTracingService();

            string measurementValue = ActionHelpers.GetInputValue<string>("???", xrmFakedPluginExecution, xrmFakedTracingService);

            Assert.IsNull(measurementValue);
        }

        [TestMethod]
        public void GetIntInputValue()
        {
            const string measurementName = "name";
            int? expected = 7;

            XrmFakedPluginExecutionContext xrmFakedPluginExecution = new XrmFakedPluginExecutionContext
            {
                InputParameters = new ParameterCollection {
                    new KeyValuePair<string, object>(measurementName, expected)
                }
            };

            XrmFakedTracingService xrmFakedTracingService = new XrmFakedTracingService();

            int? measurementValue = ActionHelpers.GetInputValue<int?>(measurementName, xrmFakedPluginExecution, xrmFakedTracingService);

            Assert.IsTrue(measurementValue.Equals(expected));
        }

        [TestMethod]
        public void GetNullIntInputValue()
        {
            const string measurementName = "name";

            XrmFakedPluginExecutionContext xrmFakedPluginExecution = new XrmFakedPluginExecutionContext
            {
                InputParameters = new ParameterCollection {
                    new KeyValuePair<string, object>(measurementName, null)
                }
            };

            XrmFakedTracingService xrmFakedTracingService = new XrmFakedTracingService();

            int? measurementValue = ActionHelpers.GetInputValue<int?>(measurementName, xrmFakedPluginExecution, xrmFakedTracingService);

            Assert.IsNull(measurementValue);
        }

        [TestMethod]
        public void GetMissingIntInputValue()
        {
            const string measurementName = "name";

            XrmFakedPluginExecutionContext xrmFakedPluginExecution = new XrmFakedPluginExecutionContext
            {
                InputParameters = new ParameterCollection {
                    new KeyValuePair<string, object>(measurementName, null)
                }
            };

            XrmFakedTracingService xrmFakedTracingService = new XrmFakedTracingService();

            int? measurementValue = ActionHelpers.GetInputValue<int?>("???", xrmFakedPluginExecution, xrmFakedTracingService);

            Assert.IsNull(measurementValue);
        }

        [TestMethod]
        public void GetBoolInputValue()
        {
            const string measurementName = "name";
            const bool expected = true;

            XrmFakedPluginExecutionContext xrmFakedPluginExecution = new XrmFakedPluginExecutionContext
            {
                InputParameters = new ParameterCollection {
                    new KeyValuePair<string, object>(measurementName, expected)
                }
            };

            XrmFakedTracingService xrmFakedTracingService = new XrmFakedTracingService();

            bool? measurementValue = ActionHelpers.GetInputValue<bool?>(measurementName, xrmFakedPluginExecution, xrmFakedTracingService);

            Assert.IsTrue(measurementValue == true);
        }

        [TestMethod]
        public void GetNullBoolInputValue()
        {
            const string measurementName = "name";

            XrmFakedPluginExecutionContext xrmFakedPluginExecution = new XrmFakedPluginExecutionContext
            {
                InputParameters = new ParameterCollection {
                    new KeyValuePair<string, object>(measurementName, null)
                }
            };

            XrmFakedTracingService xrmFakedTracingService = new XrmFakedTracingService();

            bool? measurementValue = ActionHelpers.GetInputValue<bool?>(measurementName, xrmFakedPluginExecution, xrmFakedTracingService);

            Assert.IsNull(measurementValue);
        }

        [TestMethod]
        public void GetMissingBoolInputValue()
        {
            const string measurementName = "name";

            XrmFakedPluginExecutionContext xrmFakedPluginExecution = new XrmFakedPluginExecutionContext
            {
                InputParameters = new ParameterCollection {
                    new KeyValuePair<string, object>(measurementName, null)
                }
            };

            XrmFakedTracingService xrmFakedTracingService = new XrmFakedTracingService();

            bool? measurementValue = ActionHelpers.GetInputValue<bool?>("???", xrmFakedPluginExecution, xrmFakedTracingService);

            Assert.IsNull(measurementValue);
        }
    }
}