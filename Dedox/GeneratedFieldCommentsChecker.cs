using Roslyn.Compilers.CSharp;

namespace Dedox
{
    class GeneratedFieldCommentsChecker : GeneratedSummaryCommentsChecker<FieldDeclarationSyntax>
    {
        public GeneratedFieldCommentsChecker(FieldDeclarationSyntax fieldDeclaration, IDedoxConfig config)
            : base(fieldDeclaration, config)
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