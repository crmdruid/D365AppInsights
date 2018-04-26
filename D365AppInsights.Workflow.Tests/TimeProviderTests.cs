using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace D365AppInsights.Workflow.Tests
{
    [TestClass]
    public class TimeProviderTests
    {
        [TestMethod]
        public void ReturnFixedTime()
        {
            var timeProvider = A.Fake<ITimeProvider>();

            A.CallTo(() => timeProvider.UtcNow).Returns(new DateTime(2017, 9, 10, 10, 0, 0));

            Assert.IsTrue(new DateTime(2017, 9, 10, 10, 0, 0) == timeProvider.UtcNow);
        }
    }
}