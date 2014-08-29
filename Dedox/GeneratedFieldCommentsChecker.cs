using System;
using System.Linq;

using Roslyn.Compilers.CSharp;

namespace Dedox
{
    class GeneratedFieldCommentsChecker : GeneratedCommentsChecker<FieldDeclarationSyntax>
    {
        public GeneratedFieldCommentsChecker(FieldDeclarationSyntax fieldDeclaration)
            : base(fieldDeclaration)
        {
        }

        protected override string Name
        {
            get
            {
                return It.Declaration.Variables.First().Identifier.ValueText;
            }
        }

        protected override string GetExpectedCommentForTag(XmlElementStartTagSyntax startTag)
        {
            var tag = startTag.Name.LocalName.ValueText;
            if ("summary".Equals(tag))
            {
                var expectedComment = string.Format("The {0}.", NaiveNameFixer(Name));
                Console.WriteLine("Expected field comment: '{0}'", expectedComment);
                return expectedComment;
            }

            Console.WriteLine("Unexpected tag {0} in field comment.", tag);
            return null;
        }
    }
}