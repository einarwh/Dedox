using Roslyn.Compilers.CSharp;

namespace Dedox
{
    class GeneratedEnumMemberCommentsChecker : GeneratedSummaryCommentsChecker<EnumMemberDeclarationSyntax>
    {
        public GeneratedEnumMemberCommentsChecker(EnumMemberDeclarationSyntax it)
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
    }
}