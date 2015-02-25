using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dedox.Tests
{
    [TestClass]
    public class PropertyCommentsTest : CommentsTest
    {
        [TestMethod]
        public void StripsGetSetPattern()
        {
            const string text = @"
public interface IKidYouNot 
{
   /// <summary>
   /// Gets or sets the name.
   /// </summary>
   string Name { get; set; }
}
";
            const string expected = @"
public interface IKidYouNot 
{
   string Name { get; set; }
}
";
            VerifyStrip(text, expected);
        }

        [TestMethod]
        public void StripsGetOnlyPattern()
        {
            const string text = @"
public interface IKidYouNot 
{
   /// <summary>
   /// Gets the name.
   /// </summary>
   string Name { get; }
}
";
            const string expected = @"
public interface IKidYouNot 
{
   string Name { get; }
}
";
            VerifyStrip(text, expected);
        }

        [TestMethod]
        public void StripsSetOnlyPattern()
        {
            const string text = @"
public interface IKidYouNot 
{
   /// <summary>
   /// Sets the name.
   /// </summary>
   string Name { set; }
}
";
            const string expected = @"
public interface IKidYouNot 
{
   string Name { set; }
}
";
            VerifyStrip(text, expected);
        }

        [TestMethod]
        public void StripsGetSetPatternWithValue()
        {
            const string text = @"
public interface IHoldThings 
{
   /// <summary>
   /// Gets or sets the things.
   /// </summary>
   /// <value>
   /// The things.
   /// </value>
   List<Queue<int>> Things { get; set; }
}
";
            const string expected = @"
public interface IHoldThings 
{
   List<Queue<int>> Things { get; set; }
}
";
            VerifyStrip(text, expected);
        }
    }
}