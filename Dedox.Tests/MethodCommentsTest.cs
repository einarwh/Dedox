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

        [TestMethod]
        public void StripsGhostDocPattern1()
        {
            const string text = @"
public class Mouse 
{
   /// <summary>
   /// Nibbles the exquisite cheese.
   /// </summary>
   public void NibbleExquisiteCheese() {}
}
";
            const string expected = @"
public class Mouse 
{
   public void NibbleExquisiteCheese() {}
}
";
            VerifyStrip(text, expected);
        }

        [TestMethod]
        public void StripsGhostDocPattern2()
        {
            const string text = @"
public class FireFighter
{
   /// <summary>
   /// Fights the fire with.
   /// </summary>
   /// <param name=""fire"">The fire.</param>
   public void FightFireWith(Fire fire) {}
}
";
            const string expected = @"
public class FireFighter
{
   public void FightFireWith(Fire fire) {}
}
";
            VerifyStrip(text, expected);
        }

        [TestMethod]
        public void StripsGhostDocPattern3()
        {
            const string text = @"
public class Food
{
   /// <summary>
   /// Eats this instance.
   /// </summary>
   public void Eat() {}
}
";
            const string expected = @"
public class Food
{
   public void Eat() {}
}
";
            VerifyStrip(text, expected);
        }

        [TestMethod]
        public void StripsGhostDocPattern3StaticMethod()
        {
            const string text = @"
public class Food
{
   /// <summary>
   /// Eats this instance.
   /// </summary>
   public static void Eat() {}
}
";
            const string expected = @"
public class Food
{
   public static void Eat() {}
}
";
            VerifyStrip(text, expected);
        }


    }

    public class StupidItBurns
    {
        /// <summary>
        /// Fooes this instance.
        /// </summary>
        public void Foo() { }

        /// <summary>
        /// Bars this instance.
        /// </summary>
        public static void Bar() { }
    }
}
