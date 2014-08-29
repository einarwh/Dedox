using Roslyn.Compilers.CSharp;

namespace Dedox
{
    class GeneratedClassCommentsChecker : GeneratedSummaryCommentsChecker<ClassDeclarationSyntax>
    {
        public GeneratedClassCommentsChecker(ClassDeclarationSyntax classDeclaration)
            : base(classDeclaration)
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