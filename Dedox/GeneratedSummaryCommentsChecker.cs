using System;
using System.Collections.Generic;

using Roslyn.Compilers.CSharp;

namespace Dedox
{
    abstract class GeneratedSummaryCommentsChecker<T> : GeneratedCommentsChecker<T> where T : SyntaxNode
    {
        private readonly Dictionary<string, List<Func<XmlElementStartTagSyntax, string>>> _map =
            new Dictionary<string, List<Func<XmlElementStartTagSyntax, string>>>();

        protected GeneratedSummaryCommentsChecker(T it, IDedoxConfig config, IDedoxMetrics metrics)
            : base(it, config, metrics)
        {
        }

        protected override string GetExpectedCommentForTag(XmlElementStartTagSyntax startTag, Func<string, string> nameTransform)
        {
            var tag = startTag.Name.LocalName.ValueText;
            if ("summary".Equals(tag))
            {
                // Pattern: The $(decomposed-name).
                // Pattern: The $(name).
                // Pattern: The $(name) $(decomposed-element-type-name).
                // Pattern: The $(decomposed-name) $(decomposed-element-type-name).
                return string.Format("The {0}.", nameTransform(Name));
            }

            Info("Unexpected tag {0} in comment.", tag);
            
            return null;
        }

    }
}