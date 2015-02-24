using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace Dedox.Tests
{
    public abstract class CommentsTest
    {
        protected void VerifyRetain(string input)
        {
            var actual = RunTest(input);
            Assert.IsNull(actual);
        }

        protected void VerifyStrip(string input, string output)
        {
            var actual = RunTest(input);
            Assert.AreEqual(output, actual);
        }

        private static string RunTest(string input)
        {
            var writerMock = new Mock<IConsoleWriter>();
            var writer = writerMock.Object;

            var configMock = new Mock<IDedoxConfig>();
            configMock.SetupGet(it => it.Writer).Returns(writer);

            var metricsMock = new Mock<IDedoxMetrics>();

            var config = configMock.Object;
            var metrics = metricsMock.Object;

            var sut = new Dedoxifier(config, metrics);
            return sut.Run(input);
        }
    }
}
