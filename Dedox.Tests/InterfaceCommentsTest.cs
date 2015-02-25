using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dedox.Tests
{
    [TestClass]
    public class InterfaceCommentsTest : CommentsTest
    {
        [TestMethod]
        public void StripsPattern1CapsRetained()
        {
            const string text = @"
/// <summary>
/// The Am Track.
/// </summary>
public interface IAmTrack {}
";
            const string expected = @"
public interface IAmTrack {}
";
            VerifyStrip(text, expected);
        }

        [TestMethod]
        public void StripsPattern1Lowercased()
        {
            const string text = @"
/// <summary>
/// The am track.
/// </summary>
public interface IAmTrack {}
";
            const string expected = @"
public interface IAmTrack {}
";
            VerifyStrip(text, expected);
        }

        [TestMethod]
        public void StripsPattern2()
        {
            const string text = @"
/// <summary>
/// The AmTrack.
/// </summary>
public interface IAmTrack {}
";
            const string expected = @"
public interface IAmTrack {}
";
            VerifyStrip(text, expected);
        }

        [TestMethod]
        public void StripsPattern3CaseRetained()
        {
            const string text = @"
/// <summary>
/// The Am Track interface.
/// </summary>
public interface IAmTrack {}
";
            const string expected = @"
public interface IAmTrack {}
";
            VerifyStrip(text, expected);
        }

        [TestMethod]
        public void StripsPattern3Lowercased()
        {
            const string text = @"
/// <summary>
/// The am track interface.
/// </summary>
public interface IAmTrack {}
";
            const string expected = @"
public interface IAmTrack {}
";
            VerifyStrip(text, expected);
        }

        [TestMethod]
        public void StripsPattern4()
        {
            const string text = @"
/// <summary>
/// The AmTrack interface.
/// </summary>
public interface IAmTrack {}
";
            const string expected = @"
public interface IAmTrack {}
";
            VerifyStrip(text, expected);
        }

        [TestMethod]
        public void StripsBlindChopPattern()
        {
            const string text = @"
/// <summary>
/// The eels interface.
/// </summary>
public interface Heels {}
";
            const string expected = @"
public interface Heels {}
";
            VerifyStrip(text, expected);
        }

        [TestMethod]
        public void RetainsTextThatDoesntMatchPatterns()
        {
            const string text = @"
/// <summary>
/// A marker interface for frobnitz-instances that may be edited in the thing.
/// </summary>
public interface IEditable {}
";
            VerifyRetain(text);
        }
    }
}
