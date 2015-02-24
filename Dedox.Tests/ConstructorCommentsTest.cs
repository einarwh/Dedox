using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace Dedox.Tests
{
    [TestClass]
    public class ConstructorCommentsTest : CommentsTest
    {
        [TestMethod]
        public void StripsStyleCopConstructorPattern()
        {
            const string text = @"
public class Hubblescope 
{
   /// <summary>
   /// Initializes a new instance of the <see cref=""Hubblescope""/> class.
   /// </summary>
   public Hubblescope() {}
}
";
            const string expected = @"
public class Hubblescope 
{
   public Hubblescope() {}
}
";
            VerifyStrip(text, expected);
        }

        [TestMethod]
        public void StripsStyleCopConstructorPatternWithReturn()
        {
            const string text = @"
public class Hubblescope 
{
   /// <summary>
   /// Initializes a new instance of the <see cref=""Hubblescope""/> class.
   /// </summary>
   /// <returns>
   /// The <see cref=""Hubblescope""/>.
   /// </returns>
   public Hubblescope() {}
}
";
            const string expected = @"
public class Hubblescope 
{
   public Hubblescope() {}
}
";
            VerifyStrip(text, expected);
        }

        [TestMethod]
        public void RetainsCommentThatHasRemarks()
        {
            const string text = @"
public class Hubblescope 
{
   /// <summary>
   /// Initializes a new instance of the <see cref=""Hubblescope""/> class.
   /// </summary>
   /// <remarks>
   /// This object allocates unmanaged resources and should be disposed after use.
   /// </remarks>
   public Hubblescope() {}
}
";

            VerifyRetain(text);
        }

        [TestMethod]
        public void RetainsTextThatDoesntMatchConstructorPattern()
        {
            const string text = @"
public class Hubblescope 
{
   /// <summary>
   /// This is a rare useful constructor comment.
   /// </summary>
   public Hubblescope() {}
}
";
            VerifyRetain(text);
        }
    }
}
