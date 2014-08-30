using System.IO;

using Roslyn.Compilers.CSharp;

namespace Dedox
{
    class GeneratedClassCommentsChecker : GeneratedSummaryCommentsChecker<ClassDeclarationSyntax>
    {
        public GeneratedClassCommentsChecker(ClassDeclarationSyntax classDeclaration, IDedoxConfig config)
            : base(classDeclaration, config)
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