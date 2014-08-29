using System.IO;

using Roslyn.Compilers.CSharp;

namespace Dedox
{
    class GeneratedEnumCommentsChecker : GeneratedSummaryCommentsChecker<EnumDeclarationSyntax>
    {
        public GeneratedEnumCommentsChecker(EnumDeclarationSyntax it, TextWriter writer)
            : base(it, writer)
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