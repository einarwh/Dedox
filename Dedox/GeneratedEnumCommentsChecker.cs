using Roslyn.Compilers.CSharp;

namespace Dedox
{
    class GeneratedEnumCommentsChecker : GeneratedSummaryCommentsChecker<EnumDeclarationSyntax>
    {
        public GeneratedEnumCommentsChecker(EnumDeclarationSyntax it)
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