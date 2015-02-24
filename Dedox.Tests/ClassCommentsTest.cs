using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace Dedox.Tests
{
    [TestClass]
    public class ClassCommentsTest
    {
        [TestMethod]
        public void StripsPattern1CapsRetained()
        {
            const string text = @"
/// <summary>
/// The Foo Bar Baz.
/// </summary>
public class FooBarBaz {}
";
            const string expected = @"
public class FooBarBaz {}
";
            VerifyStrip(text, expected);
        }

        [TestMethod]
        public void StripsPattern1Lowercased()
        {
            const string text = @"
/// <summary>
/// The foo bar baz.
/// </summary>
public class FooBarBaz {}
";
            const string expected = @"
public class FooBarBaz {}
";
            VerifyStrip(text, expected);
        }
        
        [TestMethod]
        public void StripsPattern2()
        {
            const string text = @"
/// <summary>
/// The FooBarBaz.
/// </summary>
public class FooBarBaz {}
";
            const string expected = @"
public class FooBarBaz {}
";
            VerifyStrip(text, expected);
        }

        [TestMethod]
        public void StripsPattern3CaseRetained()
        {
            const string text = @"
/// <summary>
/// The Foo Bar Baz class.
/// </summary>
public class FooBarBaz {}
";
            const string expected = @"
public class FooBarBaz {}
";
            VerifyStrip(text, expected);
        }

        [TestMethod]
        public void StripsPattern3Lowercased()
        {
            const string text = @"
/// <summary>
/// The foo bar baz class.
/// </summary>
public class FooBarBaz {}
";
            const string expected = @"
public class FooBarBaz {}
";
            VerifyStrip(text, expected);
        }

        [TestMethod]
        public void StripsPattern4()
        {
            const string text = @"
/// <summary>
/// The FooBarBaz class.
/// </summary>
public class FooBarBaz {}
";
            const string expected = @"
public class FooBarBaz {}
";
            VerifyStrip(text, expected);
        }


        [TestMethod]
        public void RetainsTextThatDoesntMatchPatterns()
        {
            const string text = @"
/// <summary>
/// This class handles coordination between domain objects X and Y.
/// </summary>
public class FooBarBaz {}
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
