using System.IO;

using Roslyn.Compilers.CSharp;

namespace Dedox
{
    class GeneratedClassCommentsChecker : GeneratedSummaryCommentsChecker<ClassDeclarationSyntax>
    {
        public GeneratedClassCommentsChecker(ClassDeclarationSyntax classDeclaration, TextWriter writer)
            : base(classDeclaration, writer)
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