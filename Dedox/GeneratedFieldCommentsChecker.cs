using Roslyn.Compilers.CSharp;

namespace Dedox
{
    class GeneratedFieldCommentsChecker : GeneratedSummaryCommentsChecker<FieldDeclarationSyntax>
    {
        public GeneratedFieldCommentsChecker(FieldDeclarationSyntax it, IDedoxConfig config, IDedoxMetrics metrics)
            : base(it, config, metrics)
        {
        }

        protected override string Name
        {
            get
            {
                return It.Declaration.Variables.First().Identifier.ValueText;
            }
        }
    }
}