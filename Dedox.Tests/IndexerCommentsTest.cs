using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dedox.Tests
{
    [TestClass]
    public class IndexerCommentsTest : CommentsTest
    {
        [TestMethod]
        public void IndexerTest()
        {
            const string text = @"
public class Zomg
{
   /// <summary>
   /// The this.
   /// </summary>
   /// <param name=""that"">
   /// The that.
   /// </param>
   /// <returns>
   /// The <see cref=""int""/>.
   /// </returns>
   int this[int that] { get; set; }
}
";
            const string expected = @"
public class Zomg
{
   int this[int that] { get; set; }
}
";
            VerifyStrip(text, expected);
        }
    }
}