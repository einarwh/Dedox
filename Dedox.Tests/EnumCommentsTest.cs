using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dedox.Tests
{
    [TestClass]
    public class EnumCommentsTest : CommentsTest
    {
        [TestMethod]
        public void StripsBasicPattern()
        {
            const string text = @"
/// <summary>
/// The stranger than paradise.
/// </summary>
public enum StrangerThanParadise 
{
   Lurie, Edson, Balint
}
";
            const string expected = @"
public enum StrangerThanParadise 
{
   Lurie, Edson, Balint
}
";
            VerifyStrip(text, expected);
        }

        [TestMethod]
        public void RetainsUsefulDocumentation()
        {
            const string text = @"
/// <summary>
/// Choice of actors in the Jarmusch movie.
/// </summary>
public enum StrangerThanParadise 
{
   Lurie, Edson, Balint
}
";
            VerifyRetain(text);
        }
    }
}