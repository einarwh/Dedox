using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace Dedox.Tests
{
    [TestClass]
    public class EnumMemberCommentsTest
    {
        [TestMethod]
        public void StripsBasicPattern()
        {
            const string text = @"
public enum Stranger 
{
   /// <summary>
   /// The good.
   /// </summary>
   Good,

   /// <summary>
   /// The bad.
   /// </summary>
   Bad,

   /// <summary>
   /// The ugly.
   /// </summary>
   Ugly
}
";
            const string expected = @"
public enum Stranger 
{
   Good,

   Bad,

   Ugly
}
";
            VerifyStrip(text, expected);
        }

        [TestMethod]
        public void RetainsInformation()
        {
            const string text = @"
public enum Stranger 
{
   /// <summary>
   /// Clint Eastwood.
   /// </summary>
   Good,

   /// <summary>
   /// Lee Van Cleef.
   /// </summary>
   Bad,

   /// <summary>
   /// Eli Wallach.
   /// </summary>
   Ugly
}
";
            VerifyRetain(text);
        }

        private static void VerifyRetain(string input)
        {
            var actual = RunTest(input);
            Assert.IsNull(actual);
        }

        private static void VerifyStrip(string input, string output)
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
