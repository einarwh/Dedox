using System;
using System.IO;

using Roslyn.Compilers.CSharp;

namespace Dedox
{
    abstract class GeneratedSummaryCommentsChecker<T> : GeneratedCommentsChecker<T> where T : SyntaxNode
    {
        protected GeneratedSummaryCommentsChecker(T it, TextWriter writer)
            : base(it, writer)
        {
        }

        protected override string GetExpectedCommentForTag(XmlElementStartTagSyntax startTag)
        {
            var tag = startTag.Name.LocalName.ValueText;
            if ("summary".Equals(tag))
            {
                var expectedComment = string.Format("The {0}.", NaiveNameFixer(Name));
                WriteLine("Expected comment: '{0}'", expectedComment);
                return expectedComment;
            }

            WriteLine("Unexpected tag {0} in comment.", tag);
            return null;
        }
    }
}