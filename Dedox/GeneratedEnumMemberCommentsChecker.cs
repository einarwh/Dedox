using Roslyn.Compilers.CSharp;

namespace Dedox
{
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