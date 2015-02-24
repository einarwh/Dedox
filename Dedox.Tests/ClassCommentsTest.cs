using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dedox.Tests
{
    [TestClass]
    public class ClassCommentsTest : CommentsTest
    {
        [TestMethod]
        public void StripsPattern1CapsRetained()
        {
            const string text = @"
/// <summary>
/// The Stupid It Burns.
/// </summary>
public class StupidItBurns {}
";
            const string expected = @"
public class StupidItBurns {}
";
            VerifyStrip(text, expected);
        }

        [TestMethod]
        public void StripsPattern1Lowercased()
        {
            const string text = @"
/// <summary>
/// The stupid it burns.
/// </summary>
public class StupidItBurns {}
";
            const string expected = @"
public class StupidItBurns {}
";
            VerifyStrip(text, expected);
        }
        
        [TestMethod]
        public void StripsPattern2()
        {
            const string text = @"
/// <summary>
/// The StupidItBurns.
/// </summary>
public class StupidItBurns {}
";
            const string expected = @"
public class StupidItBurns {}
";
            VerifyStrip(text, expected);
        }

        [TestMethod]
        public void StripsPattern3CaseRetained()
        {
            const string text = @"
/// <summary>
/// The Stupid It Burns class.
/// </summary>
public class StupidItBurns {}
";
            const string expected = @"
public class StupidItBurns {}
";
            VerifyStrip(text, expected);
        }

        [TestMethod]
        public void StripsPattern3Lowercased()
        {
            const string text = @"
/// <summary>
/// The stupid it burns class.
/// </summary>
public class StupidItBurns {}
";
            const string expected = @"
public class StupidItBurns {}
";
            VerifyStrip(text, expected);
        }

        [TestMethod]
        public void StripsPattern4()
        {
            const string text = @"
/// <summary>
/// The StupidItBurns class.
/// </summary>
public class StupidItBurns {}
";
            const string expected = @"
public class StupidItBurns {}
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
public class StupidItBurns {}
";
            VerifyRetain(text);
        }
    }
}
