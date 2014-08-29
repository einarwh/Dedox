using System.IO;

using Roslyn.Compilers.CSharp;

namespace Dedox
{
    class GeneratedEnumMemberCommentsChecker : GeneratedSummaryCommentsChecker<EnumMemberDeclarationSyntax>
    {
        public GeneratedEnumMemberCommentsChecker(EnumMemberDeclarationSyntax it, TextWriter writer)
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