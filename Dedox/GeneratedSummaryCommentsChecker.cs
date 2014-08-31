using System;
using System.Collections.Generic;

using Roslyn.Compilers.CSharp;

namespace Dedox
{
    abstract class GeneratedSummaryCommentsChecker<T> : GeneratedCommentsChecker<T> where T : SyntaxNode
    {
        protected GeneratedSummaryCommentsChecker(T it, IDedoxConfig config, IDedoxMetrics metrics)
            : base(it, config, metrics)
        {
        }

        protected override List<Func<string>> GetExpectedCommentForTag(XmlElementStartTagSyntax startTag)
        {
            var tag = startTag.Name.LocalName.ValueText;
            if ("summary".Equals(tag))
            {
                // Pattern: The $(decomposed-name).
                // Pattern: The $(name).
                // Pattern: The $(name) $(decomposed-element-type-name).
                // Pattern: The $(decomposed-name) $(decomposed-element-type-name).
                var decomposed = StyleCopDecompose(Name);
                var decomposedElementType = StyleCopDecompose(GetCodeElementType());
                var list = new List<Func<string>>
                               {
                                   () => string.Format("The {0}.", decomposed),
                                   () => string.Format("The {0}.", Name),
                                   () =>
                                   string.Format("The {0} {1}.", decomposed, decomposedElementType),
                                   () => string.Format("The {0} {1}.", Name, decomposedElementType)
                               };
                return list;
            }

            Info("Unexpected tag {0} in comment.", tag);
            
            return new List<Func<string>>();
        }
    }
}