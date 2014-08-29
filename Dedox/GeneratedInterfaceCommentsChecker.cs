using System;

using Roslyn.Compilers.CSharp;

namespace Dedox
{
    class GeneratedInterfaceCommentsChecker : GeneratedCommentsChecker<InterfaceDeclarationSyntax>
    {
        public GeneratedInterfaceCommentsChecker(InterfaceDeclarationSyntax it)
            : base(it)
        {
        }

        protected override string Name
        {
            get
            {
                return It.Identifier.ValueText;
            }
        }

        protected override string GetExpectedCommentForTag(XmlElementStartTagSyntax startTag)
        {
            var tag = startTag.Name.LocalName.ValueText;
            if ("summary".Equals(tag))
            {
                var n = Name;
                var name = n[0] == 'I' ? n.Substring(1) : n;
                var expectedComment = string.Format("The {0} interface.", name);
                Console.WriteLine("Expected interface comment: '{0}'", expectedComment);
                return expectedComment;
            }

            Console.WriteLine("Unexpected tag {0} in interface comment.", tag);
            return null;
        }
    }
}
