using System;

using Roslyn.Compilers.CSharp;

namespace Dedox
{
    class GeneratedClassCommentsChecker : GeneratedCommentsChecker<ClassDeclarationSyntax>
    {
        public GeneratedClassCommentsChecker(ClassDeclarationSyntax classDeclaration)
            : base(classDeclaration)
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
                var expectedComment = string.Format("The {0}.", NaiveNameFixer(Name));
                Console.WriteLine("Expected class comment: '{0}'", expectedComment);
                return expectedComment;
            }

            Console.WriteLine("Unexpected tag {0} in comment.", tag);
            return null;
        }
    }
}