using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dedox.Tests
{
    [TestClass]
    public class MethodCommentsTest : CommentsTest
    {
        [TestMethod]
        public void StripsBasicPattern()
        {
            const string text = @"
public class Launchpad 
{
   /// <summary>
   /// The fire rocket.
   /// </summary>
   public void FireRocket() {}
}
";
            const string expected = @"
public class Launchpad 
{
   public void FireRocket() {}
}
";
            VerifyStrip(text, expected);
        }

        [TestMethod]
        public void StripsBasicPatternWithReturn()
        {
            const string text = @"
public class Factory 
{
   /// <summary>
   /// The create gizmo.
   /// </summary>
   /// <returns>
   /// The <see cref=""Gizmo""/>.
   /// </returns>
   public Gizmo CreateGizmo() {}
}
";
            const string expected = @"
public class Factory 
{
   public Gizmo CreateGizmo() {}
}
";
            VerifyStrip(text, expected);
        }
    }
}
