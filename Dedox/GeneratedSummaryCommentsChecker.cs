using System;

using Roslyn.Compilers.CSharp;

namespace Dedox
{
    abstract class GeneratedSummaryCommentsChecker<T> : GeneratedCommentsChecker<T> where T : SyntaxNode
    {
        protected GeneratedSummaryCommentsChecker(T it, IDedoxConfig config, IDedoxMetrics metrics)
            : base(it, config, metrics)
        {
        }

        protected override string GetExpectedCommentForTag(XmlElementStartTagSyntax startTag, Func<string, string> nameTransform)
        {
            var tag = startTag.Name.LocalName.ValueText;
            if ("summary".Equals(tag))
            {
                return string.Format("The {0}.", nameTransform(Name));
            }

            Info("Unexpected tag {0} in comment.", tag);
            
            return null;
        }

    }
}