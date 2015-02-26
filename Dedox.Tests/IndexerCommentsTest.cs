using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dedox.Tests
{
    [TestClass]
    public class IndexerCommentsTest : CommentsTest
    {
        [TestMethod]
        public void StripsBasicPattern()
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

        [TestMethod]
        public void StripsGhostDocPattern()
        {
            const string text = @"
public class Zomg
{
   /// <summary>
   /// Gets or sets the <see cref=""System.Int32""/> with the specified lul.
   /// </summary>
   /// <value>
   /// The <see cref=""System.Int32""/>.
   /// </value>
   /// <param name=""lul"">The lul.</param>
   /// <param name=""wat"">The wat.</param>
   /// <returns></returns>
   int this[string lul, double wat] { get; set; }
}
";
            const string expected = @"
public class Zomg
{
   int this[string lul, double wat] { get; set; }
}
";
            VerifyStrip(text, expected);
        }

    }
}