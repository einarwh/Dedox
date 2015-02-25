using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dedox.Tests
{
    [TestClass]
    public class EnumMemberCommentsTest : CommentsTest
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
    }
}
