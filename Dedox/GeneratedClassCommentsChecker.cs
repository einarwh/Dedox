using Roslyn.Compilers.CSharp;

namespace Dedox
{
    class GeneratedClassCommentsChecker : GeneratedSummaryCommentsChecker<ClassDeclarationSyntax>
    {
        public GeneratedClassCommentsChecker(ClassDeclarationSyntax classDeclaration, IDedoxConfig config, IDedoxMetrics metrics)
            : base(classDeclaration, config, metrics)
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