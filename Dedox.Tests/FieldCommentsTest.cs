using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dedox.Tests
{
    [TestClass]
    public class FieldCommentsTest : CommentsTest
    {
        [TestMethod]
        public void StripsBasicPattern()
        {
            const string text = @"
public class Cat 
{
   /// <summary>
   /// The lol wut.
   /// </summary>
   public string LolWut; 
}
";
            const string expected = @"
public class Cat 
{
   public string LolWut; 
}
";
            VerifyStrip(text, expected);
        }
    }
}