using System.IO;

using Roslyn.Compilers.CSharp;

namespace Dedox
{
    class GeneratedEnumMemberCommentsChecker : GeneratedSummaryCommentsChecker<EnumMemberDeclarationSyntax>
    {
        public GeneratedEnumMemberCommentsChecker(EnumMemberDeclarationSyntax it, IDedoxConfig config)
            : base(it, config)
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