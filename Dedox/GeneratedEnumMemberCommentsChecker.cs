using Roslyn.Compilers.CSharp;

namespace Dedox
{
    /// <summary>
    /// Enum members have a simple summary tag.
    /// By default a generated summary tag is assumed to follow one of these patterns:
    ///  - Pattern #1: The $(decomposed-name).
    ///  - Pattern #2: The $(name).
    ///  - Pattern #3: The $(decomposed-name) $(decomposed-element-type-name).
    ///  - Pattern #4: The $(name) $(decomposed-element-type-name).
    /// </summary>
    class GeneratedEnumMemberCommentsChecker : GeneratedSummaryCommentsChecker<EnumMemberDeclarationSyntax>
    {
        public GeneratedEnumMemberCommentsChecker(EnumMemberDeclarationSyntax it, IDedoxConfig config, IDedoxMetrics metrics)
            : base(it, config, metrics)
        {
        }

        protected override string Name
        {
            get
            {
                return It.Identifier.ValueText;
            }
        }
    }
}